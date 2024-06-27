namespace Authentication.Infra.Service.Clients.Keycloak.Dtos;

public class CredentialDto
{
    public string Type { get; set; }
    public string Value { get; set; }
    public bool Temporary { get; set; }
}