using AutoMapper;
using DataAccessLayer.Entities.Users;
using Domain.UserModels;

namespace DataAccessLayer.MappingProfiles;

public class UserMapperProfile: Profile
{
    public UserMapperProfile()
    {
        CreateMap<UserEntity, User>().ReverseMap();
    }
}