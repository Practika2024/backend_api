using Domain.Reminders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.Configurations;
public class ReminderConfiguration : IEntityTypeConfiguration<ReminderEntity>
{
    public void Configure(EntityTypeBuilder<ReminderEntity> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Title).HasMaxLength(100).IsRequired();
        builder.Property(r => r.DueDate).IsRequired();
        builder.Property(r => r.Type).HasConversion<int>();

        builder.HasOne(r => r.Container)
            .WithMany()
            .HasForeignKey("ContainerId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}