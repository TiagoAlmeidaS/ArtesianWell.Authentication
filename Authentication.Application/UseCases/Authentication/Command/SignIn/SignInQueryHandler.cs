using System.Net;
using Authentication.Application.Services.Authentication;
using Authentication.Application.Services.Authentication.Dtos;
using Authentication.Shared.Common;
using Authentication.Shared.Dto;
using Authentication.Shared.Exceptions;
using AutoMapper;
using MediatR;
using Shared.Messages;

namespace Authentication.Application.UseCases.Authentication.Command.SignIn;

public class SignInQueryHandler: IRequestHandler<SignInQuery, SignInResult>
{
    private readonly IAuthenticationService service;
    private readonly IMapper mapper;
    private readonly IMessageHandlerService msg;
    public SignInQueryHandler(IAuthenticationService service, IMapper mapper, IMessageHandlerService msg)
    {
        this.service = service;
        this.mapper = mapper;
        this.msg = msg;
    }
    public async Task<SignInResult> Handle(SignInQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var loginRequest = new LoginDtoRequest()
            {
                Code = request.Code,
                Key = request.Key,
                Password = request.Password
            };

            var response = await service.Login(loginRequest, cancellationToken);

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

            var result = mapper.Map<LoginDtoResponse, SignInResult>(response.Data);

            result.Email = request.Key;

            return result;
        }
        catch (Exception e)
        {
            msg
                .AddError()
                .WithMessage(MessagesConsts.ErrorDefault)
                .WithStatusCode(HttpStatusCode.UnprocessableEntity)
                .WithStackTrace(e.StackTrace)
                .WithErrorCode(Guid.NewGuid().ToString())
                .Commit();

            return new();
        }
    }
}