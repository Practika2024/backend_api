using Domain.Authentications.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasConversion(p => p.Value, x => new UserId(x));

        builder.Property(p => p.Name).HasMaxLength(25);
        builder.Property(p => p.Surname).HasMaxLength(25);
        builder.Property(p => p.Patronymic).HasMaxLength(25);

        builder.Property(p => p.Email).IsRequired();
        builder.Property(x => x.PasswordHash).IsRequired();

        builder.HasMany(x => x.Roles)
            .WithMany(x => x.Users)
            .UsingEntity(x => x.ToTable("user_roles"));
        
        builder.HasOne(x => x.UserImage)
            .WithOne(x => x.User)
            .HasForeignKey<UserImage>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(u => u.CreatedContainers)
            .WithOne(c => c.CreatedByNavigation)
            .HasForeignKey("CreatedBy") 
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.CreatedProducts)
            .WithOne(p => p.CreatedByNavigation)
            .HasForeignKey("CreatedBy")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.CreatedHistories)
            .WithOne(ch => ch.CreatedByNavigation)
            .HasForeignKey("CreatedBy") 
            .OnDelete(DeleteBehavior.Restrict); 

        builder.HasMany(u => u.CreatedReminders)
            .WithOne(r => r.CreatedByNavigation)
            .HasForeignKey("CreatedBy") 
            .OnDelete(DeleteBehavior.Restrict);
    }
}