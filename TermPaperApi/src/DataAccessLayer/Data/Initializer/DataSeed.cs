using Application.Services.HashPasswordService;
using Application.Settings;
using DataAccessLayer.Entities.Containers;
using DataAccessLayer.Entities.Products;
using DataAccessLayer.Entities.Roles;
using DataAccessLayer.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Data.Initializer
{
    public static class DataSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            SeedRoles(modelBuilder);
        }

        private static void SeedRoles(ModelBuilder modelBuilder)
        {
            var roles = new List<RoleEntity>();

            foreach (var role in AuthSettings.ListOfRoles)
            {
                roles.Add(new RoleEntity { Name = role, Id = role });
            }

            modelBuilder.Entity<RoleEntity>().HasData(roles);
        }
    }
}