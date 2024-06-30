using Authentication.Application.Services.Authentication;
using Authentication.Application.Services.Authentication.Dtos;
using Authentication.Application.UseCases.Authentication.Query.SignIn;
using Authentication.Shared.Dto;
using AutoMapper;
using MediatR;

namespace Authentication.Application.UseCases.Authentication.Command.SignIn;

public class SignInQueryHandler(IAuthenticationService service, IMapper mapper): IRequestHandler<SignInQuery, ApiResponse<SignInResult>>
{
    public async Task<ApiResponse<SignInResult>> Handle(SignInQuery request, CancellationToken cancellationToken)
    {
        var loginRequest = new LoginDtoRequest()
        {
            Code = request.Code,
            Key = request.Key,
            Password = request.Password
        };

        var response = await service.Login(loginRequest, cancellationToken);
        
        var result = mapper.Map<LoginDtoResponse, SignInResult>(response.Data);

        result.Email = request.Key;
        
        return ApiResponse<SignInResult>.Success(result);
    }
}