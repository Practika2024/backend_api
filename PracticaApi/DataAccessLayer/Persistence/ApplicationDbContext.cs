using System.Reflection;
using Application.Services.HashPasswordService;
using Domain.Authentications.Roles;
using Domain.Authentications.Users;
using Domain.ContainerHistories;
using Domain.RefreshTokens;
using Domain.Containers;
using Domain.Products;
using Domain.Reminders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;
public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IHashPasswordService hashPasswordService) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    
    public DbSet<ContainerEntity> Containers { get; set; }

    public DbSet<ProductEntity> Products { get; set; }

    public DbSet<ContainerHistoryEntity> ContainerHistories { get; set; }

    public DbSet<ReminderEntity> Reminders { get; set; }
    public DbSet<ContainerContentEntity> ContainerContents { get; set; }
    public DbSet<ContainerTypeEntity> ContainerTypes { get; set; }
    public DbSet<ProductTypeEntity> ProductTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
        DataSeed.Seed(builder, hashPasswordService);
    }
}