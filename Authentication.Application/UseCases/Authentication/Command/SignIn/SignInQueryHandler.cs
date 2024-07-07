using System.Net;
using Authentication.Application.Services.Authentication;
using Authentication.Application.Services.Authentication.Dtos;
using Authentication.Shared.Dto;
using Authentication.Shared.Exceptions;
using AutoMapper;
using MediatR;
using Shared.Messages;

namespace Authentication.Application.UseCases.Authentication.Command.SignIn;

public class SignInQueryHandler: IRequestHandler<SignInQuery, ApiResponse<SignInResult>>
{
    private readonly IAuthenticationService service;
    private readonly IMapper mapper;
    public SignInQueryHandler(IAuthenticationService service, IMapper mapper)
    {
        this.service = service;
        this.mapper = mapper;
    }
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
                throw new BadRequestException(ApiResponse<SignInResult>.Error(new()
                {
                    ErrorCode = response.GetFirtsErrorCode(),
                    ErrorMessage = response.GetFirstErrorMessage()
                }));
            }

            var result = mapper.Map<LoginDtoResponse, SignInResult>(response.Data);

            result.Email = request.Key;

            return ApiResponse<SignInResult>.Success(result);
        }
        catch (Exception e)
        {
            throw new BadRequestException(ApiResponse<SignInResult>.Error(new()
            {
                ErrorCode = (int) HttpStatusCode.InternalServerError,
                ErrorMessage = "Erro ao registrar o cliente"
            }));
        }
    }
}