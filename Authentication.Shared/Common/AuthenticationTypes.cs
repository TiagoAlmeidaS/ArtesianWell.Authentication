namespace Authentication.Shared.Common;

public enum AuthenticationTypes
{
    Password = 1,
}

public class AuthenticationConsts
{
    public const string DefaultAuthenticationType = "Password";
    
    public static string GetAuthenticationType(AuthenticationTypes authenticationType)
    {
        return authenticationType switch
        {
            AuthenticationTypes.Password => "password",
            _ => DefaultAuthenticationType
        };
    }
}