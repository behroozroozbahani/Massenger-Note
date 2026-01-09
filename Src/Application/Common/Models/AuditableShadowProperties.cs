using System;
using System.Linq;
using PortalCore.Domain.Common;
using PortalCore.Domain.Entities.Identity;
using DNTCommon.Web.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace PortalCore.Application.Common.Models
{
    public class AppShadowProperties
    {
        public string? UserAgent { set; get; }
        public string? UserIp { set; get; }
        public DateTimeOffset Now { set; get; }
        public Guid? UserId { set; get; }
    }

    public static class AuditableShadowProperties
    {
        public static readonly Func<object, string> EFPropertyCreatedByBrowserName =
                                        entity => EF.Property<string>(entity, CreatedByBrowserName);
        public static readonly string CreatedByBrowserName = nameof(CreatedByBrowserName);

        public static readonly Func<object, string> EFPropertyModifiedByBrowserName =
                                        entity => EF.Property<string>(entity, ModifiedByBrowserName);
        public static readonly string ModifiedByBrowserName = nameof(ModifiedByBrowserName);

        public static readonly Func<object, string> EFPropertyDeletedByBrowserName =
            entity => EF.Property<string>(entity, DeletedByBrowserName);
        public static readonly string DeletedByBrowserName = nameof(DeletedByBrowserName);

        public static readonly Func<object, string> EFPropertyCreatedByIp =
                                        entity => EF.Property<string>(entity, CreatedByIp);
        public static readonly string CreatedByIp = nameof(CreatedByIp);

        public static readonly Func<object, string> EFPropertyModifiedByIp =
                                        entity => EF.Property<string>(entity, ModifiedByIp);
        public static readonly string ModifiedByIp = nameof(ModifiedByIp);

        public static readonly Func<object, string> EFPropertyDeletedByIp =
            entity => EF.Property<string>(entity, DeletedByIp);
        public static readonly string DeletedByIp = nameof(DeletedByIp);

        public static readonly Func<object, Guid?> EFPropertyCreatedByUserId =
                                        entity => EF.Property<Guid?>(entity, CreatedByUserId);
        public static readonly string CreatedByUserId = nameof(CreatedByUserId);

        public static readonly Func<object, Guid?> EFPropertyModifiedByUserId =
                                        entity => EF.Property<Guid?>(entity, ModifiedByUserId);
        public static readonly string ModifiedByUserId = nameof(ModifiedByUserId);

        public static readonly Func<object, Guid?> EFPropertyDeletedByUserId =
            entity => EF.Property<Guid?>(entity, DeletedByUserId);
        public static readonly string DeletedByUserId = nameof(DeletedByUserId);

        public static readonly Func<object, DateTimeOffset?> EFPropertyCreatedDateTime =
                                        entity => EF.Property<DateTimeOffset?>(entity, CreatedDateTime);
        public static readonly string CreatedDateTime = nameof(CreatedDateTime);

        public static readonly Func<object, DateTimeOffset?> EFPropertyModifiedDateTime =
                                        entity => EF.Property<DateTimeOffset?>(entity, ModifiedDateTime);
        public static readonly string ModifiedDateTime = nameof(ModifiedDateTime);

        public static readonly Func<object, DateTimeOffset?> EFPropertyDeletedDateTime =
            entity => EF.Property<DateTimeOffset?>(entity, DeletedDateTime);
        public static readonly string DeletedDateTime = nameof(DeletedDateTime);

        public static readonly Func<object, bool> EFPropertyIsDeleted =
            entity => EF.Property<bool>(entity, IsDeleted);
        public static readonly string IsDeleted = nameof(IsDeleted);

        public static void AddAuditableShadowProperties(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model
                                                .GetEntityTypes()
                                                .Where(e => typeof(IAuditableEntity).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType)
                            .Property<string>(CreatedByBrowserName).HasMaxLength(1000);
                modelBuilder.Entity(entityType.ClrType)
                            .Property<string>(ModifiedByBrowserName).HasMaxLength(1000);

                modelBuilder.Entity(entityType.ClrType)
                            .Property<string>(CreatedByIp).HasMaxLength(255);
                modelBuilder.Entity(entityType.ClrType)
                            .Property<string>(ModifiedByIp).HasMaxLength(255);

                modelBuilder.Entity(entityType.ClrType)
                            .Property<Guid?>(CreatedByUserId);
                modelBuilder.Entity(entityType.ClrType)
                            .Property<Guid?>(ModifiedByUserId);

                modelBuilder.Entity(entityType.ClrType)
                            .Property<DateTimeOffset?>(CreatedDateTime);
                modelBuilder.Entity(entityType.ClrType)
                            .Property<DateTimeOffset?>(ModifiedDateTime);
            }
        }

        public static void AddCreationTrackingShadowProperties(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model
                .GetEntityTypes()
                .Where(e => typeof(ICreationTrackingEntity).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<string>(CreatedByBrowserName).HasMaxLength(1000);

                modelBuilder.Entity(entityType.ClrType)
                    .Property<string>(CreatedByIp).HasMaxLength(255);

                modelBuilder.Entity(entityType.ClrType)
                    .Property<Guid?>(CreatedByUserId);

                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTimeOffset?>(CreatedDateTime);
            }
        }

        public static void AddModificationTrackingShadowProperties(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model
                .GetEntityTypes()
                .Where(e => typeof(IModificationTrackingEntity).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<string>(ModifiedByBrowserName).HasMaxLength(1000);

                modelBuilder.Entity(entityType.ClrType)
                    .Property<string>(ModifiedByIp).HasMaxLength(255);

                modelBuilder.Entity(entityType.ClrType)
                    .Property<Guid?>(ModifiedByUserId);

                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTimeOffset?>(ModifiedDateTime);
            }
        }

        public static void AddSoftDeleteShadowProperties(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model
                .GetEntityTypes()
                .Where(e => typeof(ISoftDeleteEntity).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<string>(DeletedByBrowserName).HasMaxLength(1000);

                modelBuilder.Entity(entityType.ClrType)
                    .Property<string>(DeletedByIp).HasMaxLength(255);

                modelBuilder.Entity(entityType.ClrType)
                    .Property<Guid?>(DeletedByUserId);

                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTimeOffset?>(DeletedDateTime);

                modelBuilder.Entity(entityType.ClrType)
                    .Property<bool>(IsDeleted);

                //instead GlobalFiltersManager.cs
                //var parameter = Expression.Parameter(entityType.ClrType, "e");
                //var body = Expression.Equal(
                //    Expression.Call(typeof(EF), nameof(EF.Property), new[] { typeof(bool) }, parameter, Expression.Constant(IsDeleted)),
                //    Expression.Constant(false));
                //modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(body, parameter));
            }
        }

        public static void AddRowVersionField(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model
                .GetEntityTypes()
                .Where(e => typeof(IHasRowVersion).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<byte[]>(nameof(IHasRowVersion.RowVersion))
                    .IsRowVersion();
                /*
                 If have below error in Update-Database command :
                Failed executing DbCommand (102ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
                ALTER TABLE [Posts] ADD [RowVersion] rowversion NOT NULL DEFAULT 0x;

                in migration => change default value to null
                e.g. : in 'added-row-version' branch => Persistence/Migrations/AddRowVersionToBlogAndPostEntities.cs
                 */
            }
        }

        public static void SetAuditableEntityPropertyValues(
            this ChangeTracker changeTracker,
            AppShadowProperties? props)
        {
            if (props == null)
            {
                return;
            }

            var modifiedEntries = changeTracker.Entries<IAuditableEntity>()
                                            .Where(x => x.State == EntityState.Modified);
            foreach (var modifiedEntry in modifiedEntries)
            {
                modifiedEntry.SetModifiedShadowProperties(props);
            }

            var addedEntries = changeTracker.Entries<IAuditableEntity>()
                                            .Where(x => x.State == EntityState.Added);
            foreach (var addedEntry in addedEntries)
            {
                addedEntry.SetAddedShadowProperties(props);
            }
        }

        public static void SetCreationTrackingEntityPropertyValues(
            this ChangeTracker changeTracker,
            AppShadowProperties? props)
        {
            if (props == null)
            {
                return;
            }

            var addedEntries = changeTracker.Entries<ICreationTrackingEntity>()
                .Where(x => x.State == EntityState.Added);
            foreach (var addedEntry in addedEntries)
            {
                addedEntry.SetAddedShadowProperties(props);
            }
        }

        public static void SetModificationTrackingEntityPropertyValues(
            this ChangeTracker changeTracker,
            AppShadowProperties? props)
        {
            if (props == null)
            {
                return;
            }

            var modifiedEntries = changeTracker.Entries<IModificationTrackingEntity>()
                .Where(x => x.State == EntityState.Modified);
            foreach (var modifiedEntry in modifiedEntries)
            {
                modifiedEntry.SetModifiedShadowProperties(props);
            }
        }

        public static void SetSoftDeleteEntityPropertyValues(
            this ChangeTracker changeTracker,
            AppShadowProperties? props)
        {
            if (props == null)
            {
                return;
            }

            var deletedEntries = changeTracker.Entries<ISoftDeleteEntity>()
                .Where(x => x.State == EntityState.Deleted);
            foreach (var deletedEntry in deletedEntries)
            {
                deletedEntry.State = EntityState.Unchanged;//NOTE: For soft-deletes to work with the original `Remove` method.
                deletedEntry.SetDeletedShadowProperties(props);
            }
        }

        public static void SetAddedShadowProperties(this EntityEntry<IAuditableEntity> addedEntry, AppShadowProperties? props)
        {
            if (props == null)
            {
                return;
            }

            addedEntry.Property(CreatedDateTime).CurrentValue = props.Now;
            if (!string.IsNullOrWhiteSpace(props.UserAgent)) addedEntry.Property(CreatedByBrowserName).CurrentValue = props.UserAgent;
            if (!string.IsNullOrWhiteSpace(props.UserIp)) addedEntry.Property(CreatedByIp).CurrentValue = props.UserIp;
            if (props.UserId.HasValue) addedEntry.Property(CreatedByUserId).CurrentValue = props.UserId;
        }

        public static void SetAddedShadowProperties(this EntityEntry<ICreationTrackingEntity> addedEntry, AppShadowProperties? props)
        {
            if (props == null)
            {
                return;
            }

            addedEntry.Property(CreatedDateTime).CurrentValue = props.Now;
            if (!string.IsNullOrWhiteSpace(props.UserAgent)) addedEntry.Property(CreatedByBrowserName).CurrentValue = props.UserAgent;
            if (!string.IsNullOrWhiteSpace(props.UserIp)) addedEntry.Property(CreatedByIp).CurrentValue = props.UserIp;
            if (props.UserId.HasValue) addedEntry.Property(CreatedByUserId).CurrentValue = props.UserId;
        }

        public static void SetAddedShadowProperties(this EntityEntry<AppLogItem> addedEntry, AppShadowProperties? props)
        {
            if (props == null)
            {
                return;
            }

            addedEntry.Property(CreatedDateTime).CurrentValue = props.Now;
            if (!string.IsNullOrWhiteSpace(props.UserAgent)) addedEntry.Property(CreatedByBrowserName).CurrentValue = props.UserAgent;
            if (!string.IsNullOrWhiteSpace(props.UserIp)) addedEntry.Property(CreatedByIp).CurrentValue = props.UserIp;
            if (props.UserId.HasValue) addedEntry.Property(CreatedByUserId).CurrentValue = props.UserId;
        }

        public static void SetModifiedShadowProperties(this EntityEntry<IAuditableEntity> modifiedEntry, AppShadowProperties? props)
        {
            if (props == null)
            {
                return;
            }

            modifiedEntry.Property(ModifiedDateTime).CurrentValue = props.Now;
            if (!string.IsNullOrWhiteSpace(props.UserAgent)) modifiedEntry.Property(ModifiedByBrowserName).CurrentValue = props.UserAgent;
            if (!string.IsNullOrWhiteSpace(props.UserIp)) modifiedEntry.Property(ModifiedByIp).CurrentValue = props.UserIp;
            if (props.UserId.HasValue) modifiedEntry.Property(ModifiedByUserId).CurrentValue = props.UserId;
        }

        public static void SetModifiedShadowProperties(this EntityEntry<IModificationTrackingEntity> modifiedEntry, AppShadowProperties? props)
        {
            if (props == null)
            {
                return;
            }

            modifiedEntry.Property(ModifiedDateTime).CurrentValue = props.Now;
            if (!string.IsNullOrWhiteSpace(props.UserAgent)) modifiedEntry.Property(ModifiedByBrowserName).CurrentValue = props.UserAgent;
            if (!string.IsNullOrWhiteSpace(props.UserIp)) modifiedEntry.Property(ModifiedByIp).CurrentValue = props.UserIp;
            if (props.UserId.HasValue) modifiedEntry.Property(ModifiedByUserId).CurrentValue = props.UserId;
        }

        public static void SetDeletedShadowProperties(this EntityEntry<ISoftDeleteEntity> deletedEntry, AppShadowProperties? props)
        {
            if (props == null)
            {
                return;
            }

            deletedEntry.Property(DeletedDateTime).CurrentValue = props.Now;
            deletedEntry.Property(IsDeleted).CurrentValue = true;
            if (!string.IsNullOrWhiteSpace(props.UserAgent)) deletedEntry.Property(DeletedByBrowserName).CurrentValue = props.UserAgent;
            if (!string.IsNullOrWhiteSpace(props.UserIp)) deletedEntry.Property(DeletedByIp).CurrentValue = props.UserIp;
            if (props.UserId.HasValue) deletedEntry.Property(DeletedByUserId).CurrentValue = props.UserId;
        }

        public static AppShadowProperties? GetShadowProperties(this IHttpContextAccessor? httpContextAccessor)
        {
            if (httpContextAccessor == null)
            {
                return null;
            }

            var httpContext = httpContextAccessor.HttpContext;
            return new AppShadowProperties
            {
                UserAgent = httpContext?.Request?.Headers["User-Agent"].ToString(),
                UserIp = httpContext?.Connection?.RemoteIpAddress?.ToString(),
                Now = DateTimeOffset.UtcNow,
                UserId = getUserId(httpContext)
            };
        }

        private static Guid? getUserId(HttpContext? httpContext)
        {
            Guid? userId = null;
            var userIdValue = httpContext?.User?.Identity?.GetUserId();
            if (!string.IsNullOrWhiteSpace(userIdValue))
            {
                userId = Guid.Parse(userIdValue);
            }

            return userId;
        }
    }
}
