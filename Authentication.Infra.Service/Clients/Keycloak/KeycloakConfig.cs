namespace Authentication.Infra.Service.Clients.Keycloak;

public class KeycloakConfig
{
    public string ClientId { get; set; }
    public string AdminId { get; set; }
    public string BaseUrl { get; set; }
    public string BaseUrlAuthorization { get; set; }
    public string ClientSecret { get; set; }
    public int Timeout { get; set; } = 15;
    public string GrantType { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}