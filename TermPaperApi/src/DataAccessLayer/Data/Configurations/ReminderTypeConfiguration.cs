using DataAccessLayer.Entities.Products;
using DataAccessLayer.Entities.Reminders;
using DataAccessLayer.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.Configurations;

public class ReminderTypeConfiguration : IEntityTypeConfiguration<ReminderTypeEntity>
{
    public void Configure(EntityTypeBuilder<ReminderTypeEntity> builder)
    {
        builder.HasKey(pt => pt.Id);
        builder.Property(pt => pt.Name).HasMaxLength(100).IsRequired();
        
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