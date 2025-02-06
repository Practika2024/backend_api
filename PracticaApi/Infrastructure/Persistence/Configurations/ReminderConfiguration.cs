using Domain.Authentications.Users;
using Domain.Reminders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistence.Configurations;
public class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
{
    public void Configure(EntityTypeBuilder<Reminder> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasConversion(r => r.Value, x => new ReminderId(x));
        builder.Property(r => r.Title).HasMaxLength(100).IsRequired();
        builder.Property(r => r.DueDate).IsRequired();
        builder.Property(r => r.Type).HasConversion<int>();

        builder.HasOne(r => r.Container)
            .WithMany()
            .HasForeignKey("ContainerId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(r => r.CreatedBy).HasConversion(r => r.Value, x => new UserId(x));
    }
}