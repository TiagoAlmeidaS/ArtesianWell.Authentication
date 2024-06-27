using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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

namespace Authentication.Infra.Service.Services;

public class AuthenticationService(IHttpClientFactory httpClientFactory, JsonSerializerOptions jsonSerializerOptions, IMapper mapper)
    : IAuthenticationService
{
    private HttpClient _httpClient = httpClientFactory.CreateClient(KeycloakConsts.GetNameApi);
    private HttpClient _httpClientAuthentication = httpClientFactory.CreateClient(KeycloakConsts.GetNameApiToken);

    public async Task<ApiResponse<LoginDtoResponse>> Login(LoginDtoRequest request, CancellationToken cancellationToken)
    {
        var loginRequestKeycloak = new LoginRequestKeycloakDto()
        {
            GrantType = AuthenticationConsts.GetAuthenticationType(AuthenticationTypes.Password),
            Username = request.Key,
            Password = request.Password
        };

        var formContent =
            ContentsUtils.GetHttpContent(loginRequestKeycloak, jsonSerializerOptions, ContentType.FormUrlEncoded);

        var keycloakResponse = await _httpClient.PostAsync(KeycloakConsts.GetPathAuthorization, formContent);

        if (!keycloakResponse.IsSuccessStatusCode)
        {
            var errorContent = await keycloakResponse.Content.ReadAsStringAsync();
            return ApiResponse<LoginDtoResponse>.Error(new ()
            {
                ErrorCode = (int) HttpStatusCode.Unauthorized,
                ErrorMessage = $"Credenciais inválidas: {errorContent}"
            });
        }

        var responseContent = await keycloakResponse.Content.ReadAsStringAsync();
        var keycloakResponseJson = JsonSerializer.Deserialize<AuthenticationJsonDto>(responseContent, jsonSerializerOptions);
        var loginResponse = mapper.Map<LoginDtoResponse>(keycloakResponseJson);
        
        return ApiResponse<LoginDtoResponse>.Success(loginResponse);
    }

    public async Task<ApiResponse<SignUpDtoResponse>> SignUp(SignUpDtoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var token = await GetTokenAdmin();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Create user
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

            // var bodyRequest = ContentsUtils.GetHttpContent(registerUser, jsonSerializerOptions);

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
        
            return ApiResponse<SignUpDtoResponse>.Success(resultRegisterMapper);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<string> GetTokenAdmin()
    {
        try
        {
            var tokenRequest = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", "admin-cli"),
                new KeyValuePair<string, string>("username", "admin"),
                new KeyValuePair<string, string>("password", "admin"),
                new KeyValuePair<string, string>("grant_type", "password")
            });

            var tokenResponse = await _httpClientAuthentication.PostAsync(KeycloakConsts.GetPathAuthorization, tokenRequest);
            if (!tokenResponse.IsSuccessStatusCode)
            {
                var errorContent = await tokenResponse.Content.ReadAsStringAsync();
            }

            var token = JsonDocument.Parse(await tokenResponse.Content.ReadAsStringAsync()).RootElement
                .GetProperty("access_token").GetString();

            return token ?? String.Empty;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

}