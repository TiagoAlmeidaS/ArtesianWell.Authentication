using Authentication.Shared.Dto;
using MediatR;

namespace Authentication.Application.UseCases.Authentication.Command.RegisterUser;

public class RegisterUserCommand: IRequest<ApiResponse<RegisterUserResult>>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string LastName { get; set; }
}