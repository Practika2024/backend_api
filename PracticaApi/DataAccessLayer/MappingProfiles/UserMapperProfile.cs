using AutoMapper;
using DataAccessLayer.Entities.Roles;
using DataAccessLayer.Entities.Users;
using Domain.RoleModels;
using Domain.UserModels;

namespace DataAccessLayer.MappingProfiles;

public class UserMapperProfile: Profile
{
    public UserMapperProfile()
    {
        CreateMap<UserEntity, User>();
        CreateMap<User, UserEntity>();
        
        CreateMap<CreateUserModel, UserEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ігноруємо Id, оскільки воно генерується автоматично
            .ForMember(dest => dest.ExternalProvider, opt => opt.Ignore()) // Ігноруємо ExternalProvider
            .ForMember(dest => dest.ExternalProviderKey, opt => opt.Ignore()); // Ігнору
        
        CreateMap<RoleEntity, Role>();
        CreateMap<Role, RoleEntity>();
        
        CreateMap<UserEntity, CreateUserModel>();
    }
}