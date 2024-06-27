using System.Text.Json.Serialization;

namespace Authentication.Infra.Service.Clients.Keycloak.Dtos;

public class LoginResponseKeycloakDto
{
    
}

public class LoginRequestKeycloakDto
{
    [JsonPropertyName("client_id")]
    public string ClientId { get; set; } = "artesianwell-client";
    [JsonPropertyName("username")]
    public string Username { get; set; }
    [JsonPropertyName("password")]
    public string Password { get; set; }
    [JsonPropertyName("grant_type")]
    public string GrantType { get; set; }
    [JsonPropertyName("client_secret")]
    public string ClientSecret { get; set; } = "sZOjz2meNh1CTq4eTFQksZG77GdqFLUe";
}