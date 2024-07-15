using System.Text.Json.Serialization;

namespace Authentication.Infra.Service.Clients.Keycloak.Dtos;

public class LoginResponseKeycloakDto
{
    
}

public class LoginRequestKeycloakDto
{
    [JsonPropertyName("client_id")]
    public string client_id { get; set; }
    [JsonPropertyName("username")]
    public string username { get; set; }
    [JsonPropertyName("password")]
    public string password { get; set; }
    [JsonPropertyName("grant_type")]
    public string grant_type { get; set; }
    [JsonPropertyName("client_secret")]
    public string client_secret { get; set; }
    
    public List<KeyValuePair<string, string>> GetFormUrlAuthorizationKeycloak()
    {
        return new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("grant_type", grant_type),
            new KeyValuePair<string, string>("client_secret", client_secret),
            new KeyValuePair<string, string>("client_id", client_id),
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", password)
        };

        
    }
}