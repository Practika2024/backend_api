using Domain.Authentications.Users;
using Domain.RefreshTokens;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
{
    public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.UserId).HasConversion(x => x.Value, x => new UserId(x));

        builder.Property(x => x.Token).HasMaxLength(450).IsRequired();
        
        builder.Property(x => x.JwtId).HasMaxLength(256).IsRequired();
        
        builder.Property(x => x.CreateDate)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");
        
        builder.Property(x => x.ExpiredDate)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");
        
        builder.HasOne(x=>x.User)
            .WithMany(x=>x.RefreshTokens)
            .HasForeignKey(x=>x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}