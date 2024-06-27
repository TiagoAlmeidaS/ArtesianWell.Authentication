using System.Text.Json.Serialization;

namespace Authentication.Infra.Service.Clients.Keycloak.Dtos;

public class GetAdminTokenDto
{
    [JsonPropertyName("client_id")]
    public string ClientId { get; set; } = "admin-cli";
    
    [JsonPropertyName("username")]
    public string UserName { get; set; } = "admin";
    
    [JsonPropertyName("password")]
    public string Password { get; set; } = "admin";
    
    [JsonPropertyName("grant_type")]
    public string GrantType { get; set; } = "password";
}