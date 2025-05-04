using DataAccessLayer.Entities.Reminders;
using DataAccessLayer.Entities.Users;
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
            .HasForeignKey(x => x.ContainerId)
            .OnDelete(DeleteBehavior.Cascade);

       builder.HasOne<UserEntity>()
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne<UserEntity>()
            .WithMany()
            .HasForeignKey(x => x.ModifiedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}