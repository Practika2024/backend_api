using Domain.Authentications.Users;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasConversion(p => p.Value, x => new ProductId(x));
        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.ManufactureDate).IsRequired();
        builder.Property(p => p.CreatedBy).HasConversion(p => p.Value, x => new UserId(x));
        builder.Property(p => p.ModifiedBy).HasConversion(p => p.Value, x => new UserId(x)).IsRequired(false);
        builder.Property(p => p.Type).HasConversion<int>();
    }
}