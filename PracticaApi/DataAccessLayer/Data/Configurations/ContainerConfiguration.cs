using DataAccessLayer.Entities.Containers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.Configurations;

public class ContainerConfiguration : IEntityTypeConfiguration<ContainerEntity>
{
    public void Configure(EntityTypeBuilder<ContainerEntity> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Volume).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(c => c.Notes).HasMaxLength(500);
        builder.Property(c => c.UniqueCode).HasMaxLength(50).IsRequired();

        builder.HasOne(c => c.Type)
            .WithMany()
            .HasForeignKey(x => x.TypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cc => cc.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByEntity)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}