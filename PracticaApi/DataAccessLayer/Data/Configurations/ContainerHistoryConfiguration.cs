using DataAccessLayer.Entities.ContainerHistories;
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
            .WithMany()
            .HasForeignKey(ch => ch.ContainerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ch => ch.Product)
            .WithMany()
            .HasForeignKey(ch => ch.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.CreatedByEntity)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}