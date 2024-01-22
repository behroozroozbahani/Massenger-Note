using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PortalCore.Application.Common.Extensions;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Common.Models;
using PortalCore.Domain.Common;
using PortalCore.Domain.Entities;
using PortalCore.Domain.Entities.Identity;
using PortalCore.Persistence.Configurations;
using PortalCore.Persistence.Configurations.Identity;
using PortalCore.Persistence.Extensions;
using PortalCore.Persistence.Toolkit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace PortalCore.Persistence.Context
{
    public class ApplicationDbContext :
        IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>,
        IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<MessengerGroup> MessengerGroups { get; set; } = null!;
        public DbSet<MessengerGroupMessage> MessengerGroupMessages { get; set; } = null!;
        public DbSet<MessengerGroupMessageSeenMessage> MessengerGroupMessageSeenMessages { get; set; } = null!;
        public DbSet<MessengerMessageFile> MessengerMessageFiles { get; set; } = null!;
        public DbSet<MessengerPrivateMessage> MessengerPrivateMessages { get; set; } = null!;
        public DbSet<MessengerGroupUser> MessengerGroupUsers { get; set; } = null!;
        public DbSet<Note> Notes { get; set; } = null!;
        public DbSet<NoteFile> NoteFiles { get; set; } = null!;

        #region BaseClass

        public virtual DbSet<AppLogItem> AppLogItems { get; set; } = null!;

        public void Migrate()
        {
            Database.Migrate();
        }

        public T GetShadowPropertyValue<T>(object entity, string propertyName) where T : IConvertible
        {
            var value = this.Entry(entity).Property(propertyName).CurrentValue;
            return value != null
                ? (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture)
                : default!;
        }

        public object GetShadowPropertyValue(object entity, string propertyName)
        {
            return this.Entry(entity).Property(propertyName).CurrentValue;
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ChangeTracker.DetectChanges();

            beforeSaveTriggers();

            ChangeTracker.AutoDetectChangesEnabled = false; // for performance reasons, to avoid calling DetectChanges() again.
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            ChangeTracker.AutoDetectChangesEnabled = true;

            return result;
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges(); //NOTE: changeTracker.Entries<T>() will call it automatically.

            beforeSaveTriggers();
            var auditEntries = setAuditEntries();
            setHash();
            var events = domainEventEntities();

            ChangeTracker.AutoDetectChangesEnabled = false; // for performance reasons, to avoid calling DetectChanges() again.
            var result = base.SaveChanges();
            ChangeTracker.AutoDetectChangesEnabled = true;

            saveAuditEntries(auditEntries);
            _ = DispatchEvents(events);

            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            ChangeTracker.DetectChanges();

            beforeSaveTriggers();
            var auditEntries = setAuditEntries();
            setHash();
            var events = domainEventEntities();

            ChangeTracker.AutoDetectChangesEnabled = false; // for performance reasons, to avoid calling DetectChanges() again.
            var result = await base.SaveChangesAsync(cancellationToken);
            ChangeTracker.AutoDetectChangesEnabled = true;

            saveAuditEntries(auditEntries);
            _ = DispatchEvents(events);

            return result;
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new())
        {
            ChangeTracker.DetectChanges();

            beforeSaveTriggers();

            ChangeTracker.AutoDetectChangesEnabled = false; // for performance reasons, to avoid calling DetectChanges() again.
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            ChangeTracker.AutoDetectChangesEnabled = true;

            return result;
        }

        private void beforeSaveTriggers()
        {
            validateEntities();
            setShadowProperties();
        }

        private void setShadowProperties()
        {
            // we can't use constructor injection anymore, because we are using the `AddDbContextPool<>`
            var props = this.GetService<IHttpContextAccessor>()?.GetShadowProperties();
            ChangeTracker.SetCreationTrackingEntityPropertyValues(props);
            ChangeTracker.SetModificationTrackingEntityPropertyValues(props);
            ChangeTracker.SetSoftDeleteEntityPropertyValues(props);
        }

        private void validateEntities()
        {
            var errors = this.GetValidationErrors();
            if (!string.IsNullOrWhiteSpace(errors))
            {
                // we can't use constructor injection anymore, because we are using the `AddDbContextPool<>`
                var loggerFactory = this.GetService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<ApplicationDbContext>();
                logger.LogError(errors);
                throw new InvalidOperationException(errors);
            }
        }

        private DomainEvent[] domainEventEntities()
        {
            return ChangeTracker.Entries<IHasDomainEvent>()
                .Select(x => x.Entity.DomainEvents)
                .SelectMany(x => x)
                .Where(domainEvent => !domainEvent.IsPublished)
                .ToArray();
        }

        private void afterSaveTriggers()
        {
            dispatchEvents();
        }

        private async void dispatchEvents()
        {
            while (true)
            {
                var domainEventEntity = ChangeTracker
                    .Entries<IHasDomainEvent>()
                    .Select(x => x.Entity.DomainEvents)
                    .SelectMany(x => x)
                    .FirstOrDefault(domainEvent => !domainEvent.IsPublished);
                if (domainEventEntity == null) break;

                var domainEventService = this.GetService<IDomainEventService>();
                domainEventEntity.IsPublished = true;
                await domainEventService.Publish(domainEventEntity);
            }
        }

        private IList<AuditEntry> setAuditEntries()
        {
            var auditEntries = new List<AuditEntry>();

            foreach (var entry in ChangeTracker.Entries<IHasRowIntegrity>())
            {
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                {
                    continue;
                }

                var auditEntry = new AuditEntry(entry);
                auditEntries.Add(auditEntry);

                //var now = DateTimeOffset.UtcNow;

                foreach (var property in entry.Properties)
                {
                    var propertyName = property.Metadata.Name;
                    if (propertyName == nameof(IHasRowIntegrity.Hash))
                    {
                        continue;
                    }
                    if (propertyName == nameof(IHasRowVersion.RowVersion))
                    {
                        continue;
                    }

                    if (property.IsTemporary)
                    {
                        // It's an auto-generated value and should be retrieved from the DB after calling the base.SaveChanges().
                        auditEntry.AuditProperties.Add(new AuditProperty(propertyName, null, true, property));
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            //entry.Property(AuditableShadowProperties.CreatedDateTime).CurrentValue = now;//Shadow property
                            auditEntry.AuditProperties.Add(new AuditProperty(propertyName, property.CurrentValue, false, property));
                            break;
                        case EntityState.Modified:
                            auditEntry.AuditProperties.Add(new AuditProperty(propertyName, property.CurrentValue, false, property));
                            //entry.Property(AuditableShadowProperties.ModifiedDateTime).CurrentValue = now;//Shadow property
                            break;
                    }
                }
            }

            return auditEntries;
        }

        private void setHash()
        {
            foreach (var entry in ChangeTracker.Entries<IHasRowIntegrity>())
            {
                var entityHash = entry.GenerateEntityEntryHash(propertyToIgnore: nameof(IHasRowIntegrity.Hash));
                entry.Property(nameof(IHasRowIntegrity.Hash)).CurrentValue = entityHash;
            }
        }

        private void saveAuditEntries(IList<AuditEntry> auditEntries)
        {
            if (auditEntries.Count > 0)
            {
                foreach (var auditEntry in auditEntries)
                {
                    foreach (var auditProperty in auditEntry.AuditProperties.Where(x => x.IsTemporary))
                    {
                        // Now we have the auto-generated value from the DB.
                        auditProperty.Value = auditProperty.PropertyEntry.CurrentValue;
                        auditProperty.IsTemporary = false;
                    }
                    auditEntry.EntityEntry.Property(nameof(IHasRowIntegrity.Hash)).CurrentValue =
                        auditEntry.AuditProperties.ToDictionary(x => x.Name, x => x.Value).GenerateObjectHash();
                }

                base.SaveChanges();
            }
        }

        private async Task DispatchEvents(DomainEvent[] events)
        {
            var domainEventService = this.GetService<IDomainEventService>();
            foreach (var @event in events)
            {
                @event.IsPublished = true;
                await domainEventService.Publish(@event);
            }
        }

        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // it should be placed here, otherwise it will rewrite the following settings!
            base.OnModelCreating(builder);

            // Adds all of the ASP.NET Core Identity related mappings at once.
            builder.AddCustomIdentityMappings();

            // Custom application mappings
            builder.AddCustomEntityMappings();

            builder.AddDateTimeUtcKindConverter();

            // This should be placed here, at the end.
            builder.AddCreationTrackingShadowProperties();
            builder.AddModificationTrackingShadowProperties();
            builder.AddSoftDeleteShadowProperties();
            builder.ApplySoftDeleteQueryFilters();
            builder.AddRowVersionField();
        }
    }
}
