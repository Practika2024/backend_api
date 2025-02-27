using DataAccessLayer.Entities.Containers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.Configurations;

public class ContainerTypeConfiguration : IEntityTypeConfiguration<ContainerTypeEntity>
{
    public void Configure(EntityTypeBuilder<ContainerTypeEntity> builder)
    {
        builder.HasKey(ct => ct.Id);
        builder.Property(ct => ct.Name).HasMaxLength(50).IsRequired();
        
        builder.HasOne(x => x.CreatedByEntity)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}