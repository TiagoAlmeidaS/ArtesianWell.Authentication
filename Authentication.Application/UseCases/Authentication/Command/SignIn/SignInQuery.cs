using Authentication.Application.UseCases.Authentication.Command.SignIn;
using Authentication.Shared.Dto;
using MediatR;

namespace Authentication.Application.UseCases.Authentication.Query.SignIn;

public class SignInQuery: IRequest<ApiResponse<SignInResult>>
{
    public string Key { get; set; }
    public string Password { get; set; }
    public string Code { get; set; }
}