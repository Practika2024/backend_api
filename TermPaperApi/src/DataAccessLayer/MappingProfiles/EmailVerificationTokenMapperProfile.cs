using AutoMapper;
using DataAccessLayer.Entities.EmailVerificationTokens;
using DataAccessLayer.Entities.RefreshTokens;
using Domain.EmailVerificationToken;
using Domain.RefreshTokens;
using Domain.RefreshTokens.Models;

namespace DataAccessLayer.MappingProfiles;

public class EmailVerificationTokenMapperProfile : Profile
{
    public EmailVerificationTokenMapperProfile()
    {
        CreateMap<EmailVerificationToken, EmailVerificationTokenEntity>().ReverseMap();
    }
}