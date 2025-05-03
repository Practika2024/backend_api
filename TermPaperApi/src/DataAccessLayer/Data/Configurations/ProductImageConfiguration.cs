using DataAccessLayer.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImageEntity>
{
    public void Configure(EntityTypeBuilder<ProductImageEntity> builder)
    {
        builder.ToTable("product_images");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.FilePath).HasMaxLength(200).IsRequired();
        
        builder.Property(x => x.FileName).HasMaxLength(50).IsRequired();
        
        builder.HasOne<ProductEntity>()
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}