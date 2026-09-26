using Microsoft.EntityFrameworkCore;
using MilanSetu.API.Models;

namespace MilanSetu.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; } = null!;
        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public DbSet<PartnerPreference> PartnerPreferences { get; set; } = null!;
        public DbSet<ProfileView> ProfileViews { get; set; } = null!;
        public DbSet<UserShortlist> UserShortlists { get; set; } = null!;
        public DbSet<UserInterest> UserInterests { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<SuccessStory> SuccessStories { get; set; } = null!;
        public DbSet<MasterReligion> MasterReligions { get; set; } = null!;
        public DbSet<MasterMotherTongue> MasterMotherTongues { get; set; } = null!;
        public DbSet<MasterEducation> MasterEducations { get; set; } = null!;
        public DbSet<MasterOccupation> MasterOccupations { get; set; } = null!;
        public DbSet<MasterIncomeRange> MasterIncomeRanges { get; set; } = null!;
        public DbSet<MasterLocation> MasterLocations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Mobile).IsUnique();
            });

            modelBuilder.Entity<ProfileView>(entity =>
            {
                entity.HasOne(pv => pv.ViewerUser)
                      .WithMany()
                      .HasForeignKey(pv => pv.ViewerUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pv => pv.ViewedUser)
                      .WithMany()
                      .HasForeignKey(pv => pv.ViewedUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserShortlist>(entity =>
            {
                entity.HasOne(us => us.User)
                      .WithMany()
                      .HasForeignKey(us => us.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(us => us.ShortlistedUser)
                      .WithMany()
                      .HasForeignKey(us => us.ShortlistedUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserInterest>(entity =>
            {
                entity.HasOne(ui => ui.SenderUser)
                      .WithMany()
                      .HasForeignKey(ui => ui.SenderUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ui => ui.ReceiverUser)
                      .WithMany()
                      .HasForeignKey(ui => ui.ReceiverUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasOne(cm => cm.Sender)
                      .WithMany()
                      .HasForeignKey(cm => cm.SenderId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(cm => cm.Receiver)
                      .WithMany()
                      .HasForeignKey(cm => cm.ReceiverId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                      .WithMany()
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
