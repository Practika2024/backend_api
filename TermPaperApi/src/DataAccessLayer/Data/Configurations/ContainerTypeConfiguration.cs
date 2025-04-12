using DataAccessLayer.Entities.Containers;
using DataAccessLayer.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.Configurations;

public class ContainerTypeConfiguration : IEntityTypeConfiguration<ContainerTypeEntity>
{
    public void Configure(EntityTypeBuilder<ContainerTypeEntity> builder)
    {
        builder.HasKey(ct => ct.Id);
        builder.Property(ct => ct.Name).HasMaxLength(50).IsRequired();
        
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