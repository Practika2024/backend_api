using AutoMapper;
using DataAccessLayer.Entities.RefreshTokens;
using Domain.RefreshTokenModels;

namespace DataAccessLayer.MappingProfiles;

public class RefreshTokenMapperProfile : Profile
{
    public RefreshTokenMapperProfile()
    {
        CreateMap<RefreshTokenEntity, CreateRefreshTokenModel>().ReverseMap();
        CreateMap<RefreshTokenEntity, RefreshToken>().ReverseMap();
    }
}