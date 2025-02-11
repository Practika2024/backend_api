using Domain.Authentications.Users;
using Domain.Containers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ContainerConfiguration : IEntityTypeConfiguration<ContainerEntity>
{
    public void Configure(EntityTypeBuilder<ContainerEntity> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasConversion(c => c.Value, x => new ContainerId(x));
        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Volume).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(c => c.Notes).HasMaxLength(500);
        builder.Property(c => c.UniqueCode).HasMaxLength(50).IsRequired();

        builder.HasOne(c => c.Type)
            .WithMany()
            .HasForeignKey("TypeId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Content)
            .WithOne()
            .HasForeignKey<ContainerEntity>("ContentId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(c => c.CreatedBy).HasConversion(c => c.Value, x => new UserId(x));
        builder.Property(c => c.ModifiedBy).HasConversion(c => c.Value, x => new UserId(x)).IsRequired(false);
    }
}