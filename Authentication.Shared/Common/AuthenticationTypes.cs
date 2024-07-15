namespace Authentication.Shared.Common;

public enum AuthenticationTypes
{
    Password = 1,
    ClientCredentials = 2
}

public class AuthenticationConsts
{
    public const string DefaultAuthenticationType = "password";
    public const String DeafultClientCredential = "client_credentials";
    
    public static string GetAuthenticationType(AuthenticationTypes authenticationType)
    {
        return authenticationType switch
        {
            AuthenticationTypes.Password => DefaultAuthenticationType,
            AuthenticationTypes.ClientCredentials => DeafultClientCredential,
            _ => DefaultAuthenticationType
        };
    }
}