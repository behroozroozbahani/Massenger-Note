using System;
using System.Threading;
using System.Threading.Tasks;
using PortalCore.Domain.Entities;
using PortalCore.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace PortalCore.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<AppLogItem> AppLogItems { get; set; }

        DbSet<MessengerGroup> MessengerGroups { get; set; }
        DbSet<MessengerGroupMessage> MessengerGroupMessages { get; set; }
        DbSet<MessengerGroupMessageSeenMessage> MessengerGroupMessageSeenMessages { get; set; }
        DbSet<MessengerMessageFile> MessengerMessageFiles { get; set; }
        DbSet<MessengerPrivateMessage> MessengerPrivateMessages { get; set; }
        DbSet<MessengerGroupUser> MessengerGroupUsers { get; set; }
        DbSet<Note> Notes { get; set; }
        DbSet<NoteFile> NoteFiles { get; set; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        void Migrate();

        T GetShadowPropertyValue<T>(object entity, string propertyName) where T : IConvertible;
        object GetShadowPropertyValue(object entity, string propertyName);

        int SaveChanges(bool acceptAllChangesOnSuccess);

        int SaveChanges();

        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new());

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new());
    }
}
