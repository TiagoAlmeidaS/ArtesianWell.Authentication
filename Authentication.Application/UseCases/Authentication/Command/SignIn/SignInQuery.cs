using MediatR;

namespace Authentication.Application.UseCases.Authentication.Command.SignIn;

public class SignInQuery: IRequest<SignInResult>
{
    public string Key { get; set; }
    public string Password { get; set; }
    public string Code { get; set; }
}