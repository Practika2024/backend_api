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
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    public DbSet<Container> Containers { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<ContainerHistory> ContainerHistories { get; set; }

    public DbSet<Reminder> Reminders { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
        DataSeed.Seed(builder, hashPasswordService);
    }
}