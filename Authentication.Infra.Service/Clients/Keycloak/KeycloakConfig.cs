namespace Authentication.Infra.Service.Clients.Keycloak;

public class KeycloakConfig
{
    public string ClientId { get; set; } = "admin-cli";
    public string BaseUrl { get; set; } = "http://localhost:8080/admin/realms/ArtesianWell/";
    public string BaseUrlAuthorization { get; set; } = "http://localhost:8080/realms/master/";
    public string ClientSecret { get; set; } = "sZOjz2meNh1CTq4eTFQksZG77GdqFLUe";
    public int Timeout { get; set; } = 15;
    public string GrantType { get; set; } = "password";
    public string Username { get; set; } = "admin";
    public string Password { get; set; } = "admin";
}