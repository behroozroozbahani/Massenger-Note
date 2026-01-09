using Microsoft.EntityFrameworkCore;
using PortalCore.Domain.Entities;
using PortalCore.Domain.Entities.Identity;

namespace PortalCore.Persistence.Configurations
{
    public static class EntityMappings
    {
        public static void AddCustomEntityMappings(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Note>(n =>
            {
                n.HasOne(x => x.User)
                    .WithMany(x => x.Notes)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                n.HasMany(x => x.NoteFiles)
                   .WithOne(x => x.Note)
                   .HasForeignKey(x => x.NoteId)
                   .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<NoteFile>(n =>
            {
                n.HasOne(x => x.Note)
                    .WithMany(x => x.NoteFiles)
                    .HasForeignKey(x => x.NoteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(n =>
            {
                n.HasMany(x => x.Notes)
                   .WithOne(x => x.User)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

                n.HasMany(x => x.MessengerGroups)
                   .WithOne(x => x.Owner)
                   .HasForeignKey(x => x.OwnerId)
                   .OnDelete(DeleteBehavior.Restrict);

                n.HasMany(x => x.MessengerGroupUsers)
                   .WithOne(x => x.User)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

                n.HasMany(x => x.SentMessages)
                   .WithOne(x => x.Sender)
                   .HasForeignKey(x => x.SenderId)
                   .OnDelete(DeleteBehavior.Restrict);

                n.HasMany(x => x.RecipientMessages)
                    .WithOne(x => x.Recipient)
                    .HasForeignKey(x => x.RecipientId)
                    .OnDelete(DeleteBehavior.Restrict);

                n.HasMany(x => x.MessengerGroupMessages)
                    .WithOne(x => x.Sender)
                    .HasForeignKey(x => x.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MessengerGroup>(n =>
            {
                n.HasOne(x => x.Owner)
                    .WithMany(x => x.MessengerGroups)
                    .HasForeignKey(x => x.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);

                n.HasMany(x => x.MessengerGroupUsers)
                    .WithOne(x => x.MessengerGroup)
                    .HasForeignKey(x => x.MessengerGroupId)
                    .OnDelete(DeleteBehavior.Restrict);

                n.HasMany(x => x.MessengerGroupMessages)
                    .WithOne(x => x.MessengerGroup)
                    .HasForeignKey(x => x.MessengerGroupId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MessengerGroupMessageSeenMessage>(n =>
            {
                n.HasKey(x => new {x.MessengerGroupMessageId, x.UserId});

                n.HasOne(x => x.MessengerGroupMessage)
                    .WithMany(x => x.MessengerGroupMessageSeenMessages)
                    .HasForeignKey(x => x.MessengerGroupMessageId)
                    .OnDelete(DeleteBehavior.Restrict);

                n.HasOne(x => x.User)
                    .WithMany(x => x.MessengerGroupMessageSeenMessages)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MessengerGroupUser>(n =>
            {
                n.HasOne(x => x.User)
                    .WithMany(x => x.MessengerGroupUsers)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                n.HasOne(x => x.MessengerGroup)
                    .WithMany(x => x.MessengerGroupUsers)
                    .HasForeignKey(x => x.MessengerGroupId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MessengerMessageFile>(n =>
            {
                n.HasOne(x => x.MessengerPrivateMessage)
                    .WithMany(x => x.MessengerMessageFiles)
                    .HasForeignKey(x => x.MessengerPrivateMessageId)
                    .OnDelete(DeleteBehavior.Restrict);

                n.HasOne(x => x.MessengerGroupMessage)
                    .WithMany(x => x.MessengerMessageFiles)
                    .HasForeignKey(x => x.MessengerGroupMessageId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MessengerPrivateMessage>(n =>
            {
                n.HasOne(x => x.Sender)
                    .WithMany(x => x.SentMessages)
                    .HasForeignKey(x => x.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                n.HasOne(x => x.Recipient)
                    .WithMany(x => x.RecipientMessages)
                    .HasForeignKey(x => x.RecipientId)
                    .OnDelete(DeleteBehavior.Restrict);

                n.HasMany(x => x.MessengerPrivateMessages)
                    .WithOne(x => x.ParentMessage)
                    .HasForeignKey(x => x.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MessengerGroupMessage>(n =>
            {
                n.HasOne(x => x.Sender)
                    .WithMany(x => x.MessengerGroupMessages)
                    .HasForeignKey(x => x.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                n.HasOne(x => x.MessengerGroup)
                    .WithMany(x => x.MessengerGroupMessages)
                    .HasForeignKey(x => x.MessengerGroupId)
                    .OnDelete(DeleteBehavior.Restrict);

                n.HasMany(x => x.MessengerMessageFiles)
                    .WithOne(x => x.MessengerGroupMessage)
                    .HasForeignKey(x => x.MessengerGroupMessageId)
                    .OnDelete(DeleteBehavior.Restrict);

                n.HasMany(x => x.MessengerGroupMessageSeenMessages)
                    .WithOne(x => x.MessengerGroupMessage)
                    .HasForeignKey(x => x.MessengerGroupMessageId)
                    .OnDelete(DeleteBehavior.Restrict);

                n.HasMany(x => x.MessengerGroupMessages)
                    .WithOne(x => x.ParentMessage)
                    .HasForeignKey(x => x.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}