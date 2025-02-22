﻿using Application.Services.HashPasswordService;
using Domain.Authentications;
using Domain.Authentications.Roles;
using Domain.Authentications.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence;

public static class DataSeed
{
    public static void Seed(ModelBuilder modelBuilder, IHashPasswordService hashPasswordService)
    {
        _seedRoles(modelBuilder);
        _seedUsers(modelBuilder, hashPasswordService);
    }

  
    private static void _seedRoles(ModelBuilder modelBuilder)
    {
        var roles = new List<Role>();

        foreach (var role in AuthSettings.ListOfRoles)
        {
            roles.Add(Role.New(role));
        }

        modelBuilder.Entity<Role>()
            .HasData(roles);
    }
    
    private static void _seedUsers(ModelBuilder modelBuilder, IHashPasswordService hashPasswordService)
    {
        var adminRole = Role.New(AuthSettings.AdminRole);
        var operatorRole = Role.New(AuthSettings.OperatorRole);
    
        var adminId = UserId.New();
        var operatorId = UserId.New();
    
        var admin = User.New(adminId, "admin@example.com", "admin","admin" ,"admin", hashPasswordService.HashPassword("123456"));
        var operatorUser = User.New(operatorId, "operator@example.com", "operator", "operator","operator",hashPasswordService.HashPassword("123456"));
    
        modelBuilder.Entity<User>().HasData(admin, operatorUser);
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity(j => j.HasData(
                new { UsersId = admin.Id, RolesId = adminRole.Id },
                new { UsersId = operatorUser.Id, RolesId = operatorRole.Id }
            ));
    }
}