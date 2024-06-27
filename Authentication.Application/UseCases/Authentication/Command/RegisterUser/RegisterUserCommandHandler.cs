using Authentication.Application.Services.Authentication;
using Authentication.Application.Services.Authentication.Dtos;
using Authentication.Shared.Dto;
using AutoMapper;
using MediatR;

namespace Authentication.Application.UseCases.Authentication.Command.RegisterUser;

public class RegisterUserCommandHandler(IAuthenticationService service, IMapper mapper): IRequestHandler<RegisterUserCommand, ApiResponse<RegisterUserResult>>
{
    public async Task<ApiResponse<RegisterUserResult>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var registerRequest = new SignUpDtoRequest()
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                LastName = request.LastName
            };
        
            var response = await service.SignUp(registerRequest, cancellationToken);
        
            var result = mapper.Map<SignUpDtoResponse, RegisterUserResult>(response.Data);
            return ApiResponse<RegisterUserResult>.Success(result);
        }
        catch (Exception e)
        {
            throw new Exception("Erro ao registrar usuário", e);
        }
    }
}