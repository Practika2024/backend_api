﻿using Application.Services.HashPasswordService;
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
        public static void Seed(ModelBuilder modelBuilder, IHashPasswordService hashPasswordService)
        {
            SeedRoles(modelBuilder);
            var seededUsers = SeedUsers(modelBuilder, hashPasswordService);
            SeedTypes(modelBuilder, seededUsers);
            SeedProductTypes(modelBuilder, seededUsers);
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

        private static List<UserEntity> SeedUsers(ModelBuilder modelBuilder, IHashPasswordService hashPasswordService)
        {
            var adminRole = new RoleEntity { Name = AuthSettings.AdminRole, Id = AuthSettings.AdminRole };
            var operatorRole = new RoleEntity { Name = AuthSettings.OperatorRole, Id = AuthSettings.OperatorRole };

            var adminId = Guid.NewGuid();
            var operatorId = Guid.NewGuid();

            var admin = new UserEntity
            {
                Id = adminId,
                RoleId = adminRole.Id,
                Email = "admin@example.com",
                Name = "admin",
                Surname = "admin",
                Patronymic = "admin",
                PasswordHash = hashPasswordService.HashPassword("123456")
            };

            var operatorUser = new UserEntity
            {
                Id = operatorId,
                RoleId = operatorRole.Id,
                Email = "operator@example.com",
                Name = "operator",
                Surname = "operator",
                Patronymic = "operator",
                PasswordHash = hashPasswordService.HashPassword("123456")
            };

            modelBuilder.Entity<UserEntity>().HasData(admin, operatorUser);

            return new List<UserEntity> { admin, operatorUser };
        }

        private static void SeedTypes(ModelBuilder modelBuilder, List<UserEntity> users)
        {
            var userId = users[0].Id;
            modelBuilder.Entity<ContainerTypeEntity>().HasData(
                new ContainerTypeEntity { Id = Guid.NewGuid(), Name = "Plastic", CreatedBy = userId },
                new ContainerTypeEntity { Id = Guid.NewGuid(), Name = "Glass", CreatedBy = userId },
                new ContainerTypeEntity { Id = Guid.NewGuid(), Name = "Metal", CreatedBy = userId },
                new ContainerTypeEntity { Id = Guid.NewGuid(), Name = "Other", CreatedBy = userId }
            );
        }

        private static void SeedProductTypes(ModelBuilder modelBuilder, List<UserEntity> users)
        {
            var userId = users[0].Id;
            modelBuilder.Entity<ProductTypeEntity>().HasData(
                new ProductTypeEntity { Id = Guid.NewGuid(), Name = "Liquid", CreatedBy = userId },
                new ProductTypeEntity { Id = Guid.NewGuid(), Name = "Solid", CreatedBy = userId },
                new ProductTypeEntity { Id = Guid.NewGuid(), Name = "Powder", CreatedBy = userId },
                new ProductTypeEntity { Id = Guid.NewGuid(), Name = "Other", CreatedBy = userId }
            );
        }
    }
}