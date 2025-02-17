using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.Configurations;

public class ProductTypeConfiguration : IEntityTypeConfiguration<ProductTypeEntity>
{
    public void Configure(EntityTypeBuilder<ProductTypeEntity> builder)
    {
        builder.HasKey(pt => pt.Id);
        builder.Property(pt => pt.Name).HasMaxLength(50).IsRequired();

        builder.HasData(
            ProductTypeEntity.New("Liquid"),
            ProductTypeEntity.New("Solid"),
            ProductTypeEntity.New("Powder"),
            ProductTypeEntity.New("Other")
        );
    }
}