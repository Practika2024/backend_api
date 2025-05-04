using DataAccessLayer.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).HasMaxLength(25);
        builder.Property(p => p.EmailConfirmed).HasDefaultValue(false);
        builder.Property(p => p.Surname).HasMaxLength(25);
        builder.Property(p => p.Patronymic).HasMaxLength(25);

        builder.Property(p => p.Email).IsRequired();
        builder.Property(x => x.PasswordHash).IsRequired();

        builder.HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        
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