using System.Text.Json.Serialization;

namespace Authentication.Infra.Service.Clients.Keycloak.Dtos;

public class AuthenticationJsonDto
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    
    [JsonPropertyName("refresh_expires_in")]
    public int RefreshExpireIn { get; set; }
    
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
    
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }
    
    [JsonPropertyName("session_state")]
    public string SessionState { get; set; }
    
    [JsonPropertyName("scope")]
    public string Scope { get; set; }
}