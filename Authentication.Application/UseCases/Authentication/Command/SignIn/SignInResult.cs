namespace Authentication.Application.UseCases.Authentication.Command.SignIn;

public class SignInResult
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public int TokenExpiration { get; set; }
}