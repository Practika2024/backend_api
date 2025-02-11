using Domain.Containers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ContainerTypeConfiguration : IEntityTypeConfiguration<ContainerTypeEntity>
{
    public void Configure(EntityTypeBuilder<ContainerTypeEntity> builder)
    {
        builder.HasKey(ct => ct.Id);
        builder.Property(ct => ct.Id).HasConversion(c => c.Value, x => new ContainerTypeId(x));
        builder.Property(ct => ct.Name).HasMaxLength(50).IsRequired();
        
        builder.HasData(
            ContainerTypeEntity.New("Plastic"),
             ContainerTypeEntity.New("Glass"),
             ContainerTypeEntity.New("Metal"),
             ContainerTypeEntity.New("Other")
        );
    }
}