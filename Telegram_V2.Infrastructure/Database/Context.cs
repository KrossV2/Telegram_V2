using Microsoft.EntityFrameworkCore;
using Telegram_V2.Core.Models;

namespace Telegram_V2.Infrastructure.Database
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<BlockedUser> BlockedUsers { get; set; }
        public DbSet<CallHistory> CallHistories { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatParticipant> ChatParticipants { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<FileAttachment> FileAttachments { get; set; }
        public DbSet<GroupSettings> GroupSettings { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageReaction> MessageReactions { get; set; }
        public DbSet<MessageStatus> MessageStatuses { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<PinnedMessage> PinnedMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Users
            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.UserName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(20);
                entity.Property(u => u.Password).IsRequired().HasMaxLength(255);
            });

            // BlockedUser
            modelBuilder.Entity<BlockedUser>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.HasOne(b => b.User)
                      .WithMany(u => u.BlockedUsers)
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(b => b.Blocked)
                      .WithMany()
                      .HasForeignKey(b => b.BlockedUserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // CallHistory
            modelBuilder.Entity<CallHistory>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasOne(c => c.Caller)
                      .WithMany(u => u.CallHistories)
                      .HasForeignKey(c => c.CallerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Receiver)
                      .WithMany()
                      .HasForeignKey(c => c.RecieverId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Channel
            modelBuilder.Entity<Channel>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasOne(c => c.Chat)
                      .WithMany()
                      .HasForeignKey(c => c.ChatId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.CreatedBy)
                      .WithMany()
                      .HasForeignKey(c => c.CreatedById)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Chat
            modelBuilder.Entity<Chat>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasOne(c => c.CreatedBy)
                      .WithMany()
                      .HasForeignKey(c => c.CreatedById)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ChatParticipant
            modelBuilder.Entity<ChatParticipant>(entity =>
            {
                entity.HasKey(cp => cp.Id);

                entity.HasOne(cp => cp.Chat)
                      .WithMany(c => c.Participants)
                      .HasForeignKey(cp => cp.ChatId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cp => cp.User)
                      .WithMany(u => u.ChatParticipants)
                      .HasForeignKey(cp => cp.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Contact
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasOne(c => c.User)
                      .WithMany(u => u.Contacts)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.ContactUser)
                      .WithMany()
                      .HasForeignKey(c => c.ContactUserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // FileAttachment
            modelBuilder.Entity<FileAttachment>(entity =>
            {
                entity.HasKey(f => f.Id);

                entity.HasOne(f => f.Message)
                      .WithMany(m => m.Attachments)
                      .HasForeignKey(f => f.MessageId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // GroupSettings
            modelBuilder.Entity<GroupSettings>(entity =>
            {
                // ChatId ni PK qilamiz
                entity.HasKey(g => g.ChatId);

                // 1:1 relationship Chat ↔ GroupSettings
                entity.HasOne(g => g.Chat)
                      .WithOne(c => c.GroupSettings)
                      .HasForeignKey<GroupSettings>(g => g.ChatId)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            // Message
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.HasOne(m => m.Chat)
                      .WithMany(c => c.Messages)
                      .HasForeignKey(m => m.ChatId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(m => m.Sender)
                      .WithMany(u => u.Messages)
                      .HasForeignKey(m => m.SenderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(m => m.ReplyTo)
                      .WithMany()
                      .HasForeignKey(m => m.ReplyToMessageId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // MessageReaction
            modelBuilder.Entity<MessageReaction>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.HasOne(r => r.Message)
                      .WithMany(m => m.Reactions)
                      .HasForeignKey(r => r.MessageId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.User)
                      .WithMany(u => u.Reactions)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // MessageStatus
            modelBuilder.Entity<MessageStatus>(entity =>
            {
                entity.HasKey(ms => ms.Id);

                entity.HasOne(ms => ms.Message)
                      .WithMany(m => m.Statuses)
                      .HasForeignKey(ms => ms.MessageId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ms => ms.User)
                      .WithMany(u => u.MessageStatuses)
                      .HasForeignKey(ms => ms.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Notification
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(n => n.Id);

                entity.HasOne(n => n.User)
                      .WithMany(u => u.Notifications)
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // PinnedMessage
            modelBuilder.Entity<PinnedMessage>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.HasOne(p => p.Chat)
                      .WithMany(c => c.PinnedMessages)
                      .HasForeignKey(p => p.ChatId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Message)
                      .WithMany()
                      .HasForeignKey(p => p.MessageId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
