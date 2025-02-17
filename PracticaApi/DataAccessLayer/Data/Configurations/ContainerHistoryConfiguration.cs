using Domain.ContainerHistories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.Configurations;
public class ContainerHistoryConfiguration : IEntityTypeConfiguration<ContainerHistoryEntity>
{
    public void Configure(EntityTypeBuilder<ContainerHistoryEntity> builder)
    {
        builder.HasKey(ch => ch.Id);
        builder.Property(ch => ch.StartDate).IsRequired();
        builder.Property(ch => ch.EndDate);

        builder.HasOne(ch => ch.Container)
            .WithMany(c => c.Histories)
            .HasForeignKey(ch => ch.ContainerId)
            .HasConstraintName("fk_container_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ch => ch.Product)
            .WithMany(p => p.Histories)
            .HasForeignKey(ch => ch.ProductId)
            .HasConstraintName("fk_product_id")
            .OnDelete(DeleteBehavior.Restrict);
    }
}