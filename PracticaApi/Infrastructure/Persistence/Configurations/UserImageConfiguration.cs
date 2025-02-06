using Domain.Authentications.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserImageConfiguration : IEntityTypeConfiguration<UserImage>
{
    public void Configure(EntityTypeBuilder<UserImage> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new UserImageId(x));

        builder.Property(x => x.UserId).HasConversion(x => x.Value, x => new UserId(x));
        
        builder.HasOne(x => x.User)
            .WithOne(x=>x.UserImage)
            .HasForeignKey<UserImage>(x => x.UserId)
            .HasConstraintName("fk_user_images_users_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}