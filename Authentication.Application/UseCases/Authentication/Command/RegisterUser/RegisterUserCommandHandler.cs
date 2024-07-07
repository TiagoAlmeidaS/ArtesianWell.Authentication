using System.Net;
using Authentication.Application.Services.Authentication;
using Authentication.Application.Services.Authentication.Dtos;
using Authentication.Shared.Dto;
using Authentication.Shared.Exceptions;
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
                throw new BadRequestException(ApiResponse<RegisterUserResult>.Error(new ()
                {
                    ErrorCode = response.GetFirtsErrorCode(),
                    ErrorMessage = response.GetFirstErrorMessage()
                }));
            }
        
            var result = mapper.Map<SignUpDtoResponse, RegisterUserResult>(response.Data);

            result.Name = request.Name;
            result.Email = request.Email;

            return result;
        }
        catch (Exception e)
        {
            throw new BadRequestException(ApiResponse<RegisterUserResult>.Error(new ()
            {
                ErrorMessage = "Erro ao registrar usuário."
            }));
        }
    }
}