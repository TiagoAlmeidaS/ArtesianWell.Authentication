using Authentication.Application.Services.Authentication.Dtos;
using Authentication.Shared.Dto;

namespace Authentication.Application.Services.Authentication;

public interface IAuthenticationService
{
    Task<ApiResponse<LoginDtoResponse>> Login(LoginDtoRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<SignUpDtoResponse>> SignUp(SignUpDtoRequest request, CancellationToken cancellationToken);
}