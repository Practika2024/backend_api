using Domain.Containers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.Configurations;

public class ContainerContentConfiguration : IEntityTypeConfiguration<ContainerContentEntity>
{
    public void Configure(EntityTypeBuilder<ContainerContentEntity> builder)
    {
        builder.HasKey(cc => cc.Id);
        builder.Property(cc => cc.IsEmpty).IsRequired();
        builder.HasOne(cc => cc.Product)
            .WithMany()
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.CreatedByEntity)
            .WithMany()
            .HasForeignKey("CreatedBy")
            .OnDelete(DeleteBehavior.Restrict);
    }
}