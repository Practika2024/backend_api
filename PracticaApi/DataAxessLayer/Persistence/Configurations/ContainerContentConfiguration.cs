using Domain.Authentications.Users;
using Domain.Containers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ContainerContentConfiguration : IEntityTypeConfiguration<ContainerContentEntity>
{
    public void Configure(EntityTypeBuilder<ContainerContentEntity> builder)
    {
        builder.HasKey(cc => cc.Id);
        builder.Property(cc => cc.Id).HasConversion(c => c.Value, x => new ContentId(x));
        builder.Property(cc => cc.IsEmpty).IsRequired();
        builder.HasOne(cc => cc.Product)
            .WithMany()
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(cc => cc.ModifiedBy).HasConversion(c => c.Value, x => new UserId(x)).IsRequired(false);
    }
}