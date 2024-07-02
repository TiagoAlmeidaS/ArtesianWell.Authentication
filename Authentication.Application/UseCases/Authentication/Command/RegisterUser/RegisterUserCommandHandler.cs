using System.Net;
using Authentication.Application.Services.Authentication;
using Authentication.Application.Services.Authentication.Dtos;
using AutoMapper;
using MediatR;
using Shared.Messages;

namespace Authentication.Application.UseCases.Authentication.Command.RegisterUser;

public class RegisterUserCommandHandler(IAuthenticationService service, IMapper mapper, IMessageHandlerService msg): IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
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

            if (response.HasError)
            {
                msg.AddError()
                    .WithErrorCode(Guid.NewGuid().ToString())
                    .WithMessage($"Erro ao registrar o cliente.")
                    .WithStackTrace(response.GetFirstErrorMessage())
                    .WithStatusCode((HttpStatusCode) response.GetFirtsErrorCode())
                    .Commit();
                
                return new ();
            }
        
            var result = mapper.Map<SignUpDtoResponse, RegisterUserResult>(response.Data);

            result.Name = request.Name;
            result.Email = request.Email;

            return result;
        }
        catch (Exception e)
        {
            msg.AddError()
                .WithErrorCode(Guid.NewGuid().ToString())
                .WithMessage($"Erro ao registrar o cliente.")
                .WithStackTrace(e.StackTrace)
                .WithStatusCode(HttpStatusCode.BadRequest)
                .Commit();

            return new();
        }
    }
}