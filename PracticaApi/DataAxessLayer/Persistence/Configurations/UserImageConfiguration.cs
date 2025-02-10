using Domain.Authentications.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserImageConfiguration : IEntityTypeConfiguration<UserImageEntity>
{
    public void Configure(EntityTypeBuilder<UserImageEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new UserImageId(x));

        builder.Property(x => x.UserId).HasConversion(x => x.Value, x => new UserId(x));
        
        builder.HasOne(x => x.UserEntity)
            .WithOne(x=>x.UserImage)
            .HasForeignKey<UserImageEntity>(x => x.UserId)
            .HasConstraintName("fk_user_images_users_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}