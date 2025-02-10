﻿using Domain.Authentications.Users;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasConversion(p => p.Value, x => new ProductId(x));
        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.ManufactureDate).IsRequired();

        builder.HasOne(p => p.Type)
            .WithMany()
            .HasForeignKey("TypeId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.CreatedBy).HasConversion(p => p.Value, x => new UserId(x));
        builder.Property(p => p.ModifiedBy).HasConversion(p => p.Value, x => new UserId(x)).IsRequired(false);
    }
}