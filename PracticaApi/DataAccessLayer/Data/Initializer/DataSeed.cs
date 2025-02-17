using Application.Services.HashPasswordService;
using Domain.Authentications;
using Domain.Containers;
using Domain.Roles;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Data.Initializer
{
    public static class DataSeed
    {
        public static void Seed(ModelBuilder modelBuilder, IHashPasswordService hashPasswordService)
        {
            SeedRoles(modelBuilder);
            var seededUsers = SeedUsers(modelBuilder, hashPasswordService);
            SeedTypes(modelBuilder, seededUsers);
        }

        private static void SeedRoles(ModelBuilder modelBuilder)
        {
            var roles = new List<RoleEntity>();

            foreach (var role in AuthSettings.ListOfRoles)
            {
                roles.Add(RoleEntity.New(role));
            }

            modelBuilder.Entity<RoleEntity>().HasData(roles);
        }

        private static List<UserEntity> SeedUsers(ModelBuilder modelBuilder, IHashPasswordService hashPasswordService)
        {
            var adminRole = RoleEntity.New(AuthSettings.AdminRole);
            var operatorRole = RoleEntity.New(AuthSettings.OperatorRole);

            var adminId = Guid.NewGuid();
            var operatorId = Guid.NewGuid();

            var admin = UserEntity.New(
                adminId,
                adminRole.Id,
                "admin@example.com",
                "admin",
                "admin",
                "admin",
                hashPasswordService.HashPassword("123456")
            );

            var operatorUser = UserEntity.New(
                operatorId,
                operatorRole.Id,
                "operator@example.com",
                "operator",
                "operator",
                "operator",
                hashPasswordService.HashPassword("123456")
            );

            modelBuilder.Entity<UserEntity>().HasData(admin, operatorUser);

            return new List<UserEntity> { admin, operatorUser };
        }

        private static void SeedTypes(ModelBuilder modelBuilder, List<UserEntity> users)
        {
            modelBuilder.Entity<ContainerTypeEntity>().HasData(
                ContainerTypeEntity.New("Plastic", users[0].Id),
                ContainerTypeEntity.New("Glass", users[0].Id),
                ContainerTypeEntity.New("Metal", users[0].Id),
                ContainerTypeEntity.New("Other", users[0].Id)
            );
        }
    }
}