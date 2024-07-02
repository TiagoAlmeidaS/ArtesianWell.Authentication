using Authentication.Application.Services.Authentication.Dtos;
using Authentication.Infra.Service.Clients.Keycloak.Dtos;
using AutoMapper;

namespace Authentication.Infra.Service.Mapper;

public class AuthenticationProfile: Profile
{
    public AuthenticationProfile()
    {
        CreateMap<AuthenticationJsonDto, LoginDtoResponse>()
            .ForMember(dest => dest.Token, src => src.MapFrom(x => x.AccessToken))
            .ForMember(dest => dest.RefreshToken, src => src.MapFrom(x => x.RefreshToken))
            .ForMember(dest => dest.TokenExpiration, src => src.MapFrom(x => x.ExpiresIn));
        
    }
}