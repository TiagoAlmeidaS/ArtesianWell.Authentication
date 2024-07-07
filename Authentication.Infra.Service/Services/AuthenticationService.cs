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
using Authentication.Shared.Exceptions;
using Authentication.Shared.Utils;
using AutoMapper;
using Microsoft.Extensions.Options;

namespace Authentication.Infra.Service.Services;

public class AuthenticationService
    : IAuthenticationService
{
    private HttpClient _httpClient;
    private HttpClient _httpClientAuthentication;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly IMapper _mapper;
    private readonly KeycloakConfig _keycloakConfig;
    
    
    public AuthenticationService(IHttpClientFactory httpClientFactory, JsonSerializerOptions jsonSerializerOptions, IMapper mapper, IOptions<KeycloakConfig> keycloakConfig)
    {
        _httpClient = httpClientFactory.CreateClient(KeycloakConsts.GetNameApi);
        _httpClientAuthentication = httpClientFactory.CreateClient(KeycloakConsts.GetNameApiToken);
        _jsonSerializerOptions = jsonSerializerOptions;
        _mapper = mapper;
        _keycloakConfig = keycloakConfig.Value;
    }
    

    public async Task<ApiResponse<LoginDtoResponse>> Login(LoginDtoRequest request, CancellationToken cancellationToken)
    {
        var loginRequestKeycloak = new LoginRequestKeycloakDto()
        {
            GrantType = AuthenticationConsts.GetAuthenticationType(AuthenticationTypes.Password),
            Username = request.Key,
            Password = request.Password
        };

        var formContent =
            ContentsUtils.GetHttpContent(loginRequestKeycloak, _jsonSerializerOptions, ContentType.FormUrlEncoded);

        var keycloakResponse = await _httpClient.PostAsync(KeycloakConsts.GetPathAuthorization, formContent);

        if (!keycloakResponse.IsSuccessStatusCode)
        {
            var errorContent = await keycloakResponse.Content.ReadAsStringAsync();
            throw new BadRequestException(ApiResponse<LoginDtoResponse>.Error(new()
            {
                ErrorCode = (int)HttpStatusCode.Unauthorized,
                ErrorMessage = $"Credenciais inválidas: {errorContent}"
            }));
        }

        var responseContent = await keycloakResponse.Content.ReadAsStringAsync();
        var keycloakResponseJson = JsonSerializer.Deserialize<AuthenticationJsonDto>(responseContent, _jsonSerializerOptions);
        var loginResponse = _mapper.Map<LoginDtoResponse>(keycloakResponseJson);
        
        return ApiResponse<LoginDtoResponse>.Success(loginResponse, keycloakResponse.StatusCode);
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
                throw new Exception("Não foi possível criar esse usuário");
            }
            
            var keycloakResponseJson = JsonSerializer.Deserialize<AuthenticationJsonDto>(content, jsonSerializerOptions);

            var resultRegisterMapper = new SignUpDtoResponse()
            {
                Name = request.Name,
                Token = keycloakResponseJson.AccessToken,
                RefreshToken = keycloakResponseJson.RefreshToken,
                TokenExpiration = keycloakResponseJson.ExpiresIn,
                Scope = keycloakResponseJson.Scope
            };
        
            return ApiResponse<SignUpDtoResponse>.Success(resultRegisterMapper, userResponse.StatusCode);
        }
        catch (Exception e)
        {
            throw new BadRequestException(ApiResponse<SignUpDtoResponse>.Error(new()
            {
                ErrorCode = (int)HttpStatusCode.InternalServerError,
                ErrorMessage = e.Message
            }));
        }
    }

    private async Task<string> GetTokenAdmin()
    {
        try
        {
            var tokenRequest = KeycloakConsts.GetFormUrlAuthorizationKeycloak(new()
            {
                GrantType = _keycloakConfig.GrantType,
                Password = _keycloakConfig.Password,
                UserName = _keycloakConfig.Username,
                ClientId = _keycloakConfig.ClientId
            });

            var tokenResponse = await _httpClientAuthentication.PostAsync(KeycloakConsts.GetPathAuthorization, tokenRequest);
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
            Console.WriteLine(e);
            throw;
        }
    }

}