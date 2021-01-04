using GSCrm.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GSCrm.Data.EntityConfigurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> modelBuilder)
        {
            modelBuilder
                .HasMany(userNot => userNot.UserNotifications)
                .WithOne(not => not.Notification)
                .HasForeignKey(notId => notId.NotificationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
