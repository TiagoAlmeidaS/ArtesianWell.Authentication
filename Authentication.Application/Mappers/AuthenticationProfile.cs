using Authentication.Application.Services.Authentication.Dtos;
using Authentication.Application.UseCases.Authentication.Command.RegisterUser;
using Authentication.Application.UseCases.Authentication.Query.SignIn;
using AutoMapper;

namespace Authentication.Application.Mappers;

public class AuthenticationProfile: Profile
{
    public AuthenticationProfile()
    {
        CreateMap<SignInQuery, LoginDtoRequest>();
        CreateMap<LoginDtoResponse, SignInResult>();
        CreateMap<RegisterUserCommand, SignUpDtoRequest>();
        CreateMap<SignUpDtoResponse, RegisterUserResult>();
    }
}