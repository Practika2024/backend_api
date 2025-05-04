using DataAccessLayer.Entities.EmailVerificationTokens;
using DataAccessLayer.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.Configurations;

public class EmailVerificationTokenConfiguration : IEntityTypeConfiguration<EmailVerificationTokenEntity>
{
    public void Configure(EntityTypeBuilder<EmailVerificationTokenEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne<UserEntity>().WithMany().HasForeignKey(e => e.UserId);
    }
}