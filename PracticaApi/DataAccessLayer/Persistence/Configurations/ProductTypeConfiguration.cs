using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductTypeConfiguration : IEntityTypeConfiguration<ProductTypeEntity>
{
    public void Configure(EntityTypeBuilder<ProductTypeEntity> builder)
    {
        builder.HasKey(pt => pt.Id);
        builder.Property(pt => pt.Id).HasConversion(c => c.Value, x => new ProductTypeId(x));
        builder.Property(pt => pt.Name).HasMaxLength(50).IsRequired();

        builder.HasData(
            ProductTypeEntity.New("Liquid"),
            ProductTypeEntity.New("Solid"),
            ProductTypeEntity.New("Powder"),
            ProductTypeEntity.New("Other")
        );
    }
}