using System.Net;
using Authentication.Application.Services.Authentication;
using Authentication.Application.Services.Authentication.Dtos;
using Authentication.Shared.Common;
using AutoMapper;
using MediatR;
using Shared.Messages;

namespace Authentication.Application.UseCases.Authentication.Command.RegisterUser;

public class RegisterUserCommandHandler: IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    
    private readonly IAuthenticationService service;
    private readonly IMapper mapper;
    private readonly IMessageHandlerService msg;
    
    public RegisterUserCommandHandler(IAuthenticationService service, IMapper mapper, IMessageHandlerService msg)
    {
        this.service = service;
        this.mapper = mapper;
        this.msg = msg;
    }
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
                msg
                    .AddError()
                    .WithMessage(response.GetFirstErrorMessage())
                    .WithStatusCode((HttpStatusCode) response.GetFirtsErrorCode())
                    .WithErrorCode(Guid.NewGuid().ToString())
                    .Commit();

                return new();
            }

            return new();
        }
        catch (Exception e)
        {
            msg
                .AddError()
                .WithMessage(MessagesConsts.ErrorDefault)
                .WithStatusCode(HttpStatusCode.UnprocessableEntity)
                .WithErrorCode(Guid.NewGuid().ToString())
                .WithStackTrace(e.StackTrace)
                .Commit();

            return new();
        }
    }
}