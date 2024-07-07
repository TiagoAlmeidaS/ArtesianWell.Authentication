using System.Net;
using Authentication.Application.Services.Authentication;
using Authentication.Application.Services.Authentication.Dtos;
using Authentication.Shared.Dto;
using AutoMapper;
using MediatR;
using Shared.Messages;

namespace Authentication.Application.UseCases.Authentication.Command.SignIn;

public class SignInQueryHandler(IAuthenticationService service, IMapper mapper, IMessageHandlerService msg): IRequestHandler<SignInQuery, ApiResponse<SignInResult>>
{
    public async Task<ApiResponse<SignInResult>> Handle(SignInQuery request, CancellationToken cancellationToken)
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
                msg.AddError()
                    .WithErrorCode(Guid.NewGuid().ToString())
                    .WithMessage(response.GetFirstErrorMessage())
                    .WithStatusCode((HttpStatusCode) response.GetFirtsErrorCode())
                    .Commit();

                return ApiResponse<SignInResult>.Error(new()
                {
                    ErrorCode = response.GetFirtsErrorCode(),
                    ErrorMessage = response.GetFirstErrorMessage()
                });
            }

            var result = mapper.Map<LoginDtoResponse, SignInResult>(response.Data);

            result.Email = request.Key;

            return ApiResponse<SignInResult>.Success(result);
        }
        catch (Exception e)
        {
            msg.AddError()
                .WithErrorCode(Guid.NewGuid().ToString())
                .WithMessage($"Erro ao registrar o cliente.")
                .WithStackTrace(e.StackTrace)
                .WithStatusCode(HttpStatusCode.BadRequest)
                .Commit();

            return ApiResponse<SignInResult>.Error(new()
            {
                ErrorCode = (int) HttpStatusCode.InternalServerError,
                ErrorMessage = "Erro ao registrar o cliente"
            });
        }
    }
}