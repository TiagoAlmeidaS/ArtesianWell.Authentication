using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Authentication.Application.Services.Authentication;
using Authentication.Application.Services.Authentication.Dtos;
using Authentication.Infra.Service.Clients.Keycloak;
using Authentication.Infra.Service.Clients.Keycloak.Dtos;
using Authentication.Shared.Common;
using Authentication.Shared.Dto;
using Authentication.Shared.Utils;
using AutoMapper;
using Microsoft.Extensions.Options;
using Shared.Messages;

namespace Authentication.Infra.Service.Services;

public class AuthenticationService(IMessageHandlerService msg, IMapper mapper, IHttpClientFactory httpClientFactory, IOptions<KeycloakConfig> keycloakConfig, JsonSerializerOptions jsonSerializerOptions)
    : IAuthenticationService
{
    private HttpClient _httpClient = httpClientFactory.CreateClient(KeycloakConsts.GetNameApi);
    private HttpClient _httpClientAuthentication = httpClientFactory.CreateClient(KeycloakConsts.GetNameApiToken);
    

    public async Task<ApiResponse<LoginDtoResponse>> Login(LoginDtoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var loginRequestKeycloak = new LoginRequestKeycloakDto()
            {
                grant_type = AuthenticationConsts.GetAuthenticationType(AuthenticationTypes.Password),
                client_secret = keycloakConfig.Value.ClientSecret,
                client_id = keycloakConfig.Value.ClientId,
                username = request.Key,
                password = request.Password
            };
            
            var bodyContent = ContentsUtils.GetHttpContent(loginRequestKeycloak, jsonSerializerOptions, ContentType.FormUrlEncoded);
            
            var keycloakResponse = await _httpClient.PostAsync(KeycloakConsts.GetPathAuthorization, bodyContent);

            if (!keycloakResponse.IsSuccessStatusCode)
            {
                var errorContent = await keycloakResponse.Content.ReadAsStringAsync();
                msg
                    .AddError()
                    .WithMessage($"Credenciais inválidas: {errorContent}")
                    .WithStatusCode(HttpStatusCode.Unauthorized)
                    .WithErrorCode(Guid.NewGuid().ToString())
                    .Commit();
                return ApiResponse<LoginDtoResponse>.Error(new()
                {
                    ErrorCode = (int)HttpStatusCode.Unauthorized,
                    ErrorMessage = $"Credenciais inválidas: {errorContent}"
                });
            }

            var responseContent = await keycloakResponse.Content.ReadAsStringAsync();
            var keycloakResponseJson =
                JsonSerializer.Deserialize<AuthenticationJsonDto>(responseContent, jsonSerializerOptions);
            var loginResponse = mapper.Map<LoginDtoResponse>(keycloakResponseJson);

            return ApiResponse<LoginDtoResponse>.Success(loginResponse, keycloakResponse.StatusCode);
        }
        catch (Exception e)
        {
            msg
                .AddError()
                .WithMessage(MessagesConsts.ErrorDefault)
                .WithStatusCode(HttpStatusCode.InternalServerError)
                .WithStackTrace(e.StackTrace)
                .WithErrorCode(Guid.NewGuid().ToString())
                .Commit();

            return new ApiResponse<LoginDtoResponse>();
        }
    }

    public async Task<ApiResponse<SignUpDtoResponse>> SignUp(SignUpDtoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var token = await GetTokenAdmin();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(KeycloakConsts.AuthorizationHeader, token);

            var registerUser = new RegisterUserDto()
            {
                Email = request.Email,
                Enabled = true,
                FirstName = request.Name,
                LastName = request.LastName,
                UserName = request.Email,
                Credentials = new()
                {
                    new()
                    {
                        Temporary = false,
                        Type = AuthenticationConsts.GetAuthenticationType(AuthenticationTypes.Password),
                        Value = request.Password,
                    }
                }
            };

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var bodyRequest = new StringContent(JsonSerializer.Serialize(registerUser, jsonSerializerOptions), Encoding.UTF8, "application/json");
            
            var userResponse = await _httpClient.PostAsync(KeycloakConsts.GetPathUsers, bodyRequest);
            var content = await userResponse.Content.ReadAsStringAsync();

            if (userResponse.StatusCode != HttpStatusCode.OK && userResponse.StatusCode != HttpStatusCode.Created)
            {
                msg
                    .AddError()
                    .WithMessage(content)
                    .WithStatusCode(userResponse.StatusCode)
                    .WithErrorCode(Guid.NewGuid().ToString())
                    .Commit();

                return ApiResponse<SignUpDtoResponse>.Error(new ()
                {
                    ErrorCode = (int) userResponse.StatusCode,
                    ErrorMessage = content
                });
            }
            
            return ApiResponse<SignUpDtoResponse>.Success(new (), userResponse.StatusCode);
        }
        catch (Exception e)
        {
            msg
                .AddError()
                .WithMessage(MessagesConsts.ErrorDefault)
                .WithStatusCode(HttpStatusCode.InternalServerError)
                .WithErrorCode(Guid.NewGuid().ToString())
                .WithStackTrace(e.StackTrace)
                .Commit();

            return new ApiResponse<SignUpDtoResponse>();
        }
    }

    private async Task<string> GetTokenAdmin()
    {
        try
        {
            var keycloak = keycloakConfig.Value;
            
            var tokenRequest = KeycloakConsts.GetFormUrlAuthorizationKeycloak(new()
            {
                GrantType = keycloak.GrantType,
                Password = keycloak.Password,
                UserName = keycloak.Username,
                ClientId = keycloak.AdminId
            });

            var tokenResponse = await _httpClientAuthentication.PostAsync(KeycloakConsts.GetPathToken, tokenRequest);
            if (!tokenResponse.IsSuccessStatusCode)
            {
                var errorContent = await tokenResponse.Content.ReadAsStringAsync();
            }

            var token = JsonDocument.Parse(await tokenResponse.Content.ReadAsStringAsync()).RootElement
                .GetProperty(KeycloakConsts.AccessTokenHeader).GetString();

            return token ?? String.Empty;
        }
        catch (Exception e)
        {
            msg
                .AddError()
                .WithMessage(MessagesConsts.ErrorDefault)
                .WithStatusCode(HttpStatusCode.InternalServerError)
                .WithErrorCode(Guid.NewGuid().ToString())
                .Commit();
            
            return String.Empty;
        }
    }

}