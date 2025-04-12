using AutoMapper;
using DataAccessLayer.Entities.Roles;
using DataAccessLayer.Entities.Users;
using Domain.Roles;
using Domain.Users;
using Domain.Users.Models;

namespace DataAccessLayer.MappingProfiles;

public class UserMapperProfile: Profile
{
    public UserMapperProfile()
    {
        CreateMap<UserEntity, User>().ReverseMap();
        
        CreateMap<CreateUserModel, UserEntity>()
            //.ForMember(dest => dest.Id, opt => opt.Ignore()) // Ігноруємо Id, оскільки воно генерується автоматично
            .ForMember(dest => dest.ExternalProvider, opt => opt.Ignore()) // Ігноруємо ExternalProvider
            .ForMember(dest => dest.ExternalProviderKey, opt => opt.Ignore()); // Ігнору
        CreateMap<UserEntity, CreateUserModel>();
        
        CreateMap<RoleEntity, Role>().ReverseMap();
        
        CreateMap<UserEntity, UpdateUserModel>().ReverseMap();
    }
}