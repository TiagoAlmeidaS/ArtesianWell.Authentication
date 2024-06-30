using Authentication.Infra.Service.Clients.Keycloak.Dtos;

namespace Authentication.Infra.Service.Clients.Keycloak;

public class KeycloakConsts
{
   public static string GetPathUsers = "users";
   public static string GetPathAuthorization = "protocol/openid-connect/token";
   public static string GetNameApi = "keycloak";
   public static string GetNameApiToken = "token";

   public static string AuthorizationHeader = "Bearer";
   public static string AccessTokenHeader = "access_token";

   public static FormUrlEncodedContent GetFormUrlAuthorizationKeycloak(GetAdminTokenDto getAdminTokenDto)
   {
    return new FormUrlEncodedContent(new[]
    {
        new KeyValuePair<string, string>("client_id", getAdminTokenDto.ClientId),
        new KeyValuePair<string, string>("username", getAdminTokenDto.UserName),
        new KeyValuePair<string, string>("password", getAdminTokenDto.Password),
        new KeyValuePair<string, string>("grant_type", getAdminTokenDto.GrantType)
    });  
   }
}