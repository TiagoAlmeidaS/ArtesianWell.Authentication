using System.Text.Json.Serialization;

namespace Authentication.Infra.Service.Clients.Keycloak.Dtos;

public class RegisterUserDto
{
    [JsonPropertyName("username")]
    public string UserName { get; set; }
    public string Email { get; set; }
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }
    [JsonPropertyName("lastName")]
    public string LastName { get; set; }
    public bool Enabled { get; set; }
    public List<CredentialDto> Credentials { get; set; } 
}