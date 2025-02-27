using Api.Dtos.Authentications;
using Api.Dtos.ContainersType;
using Api.Dtos.Users;
using AutoMapper;
using DataAccessLayer.Entities.Containers;
using Domain.Users;
using Domain.Users.Models;

namespace Api.MappingProfiles;

public class UserMapperProfile: Profile
{
    public UserMapperProfile()
    {
        CreateMap<UserDto, User>().ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Role)).ReverseMap();
        CreateMap<ExternalLoginDto, ExternalLoginModel>().ReverseMap();
    }
}
