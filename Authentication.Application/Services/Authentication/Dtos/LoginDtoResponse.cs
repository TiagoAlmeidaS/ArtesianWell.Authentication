namespace Authentication.Application.Services.Authentication.Dtos;

public class LoginDtoResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public string Name { get; set; }
    public string TokenExpiration { get; set; }
}

public class LoginDtoRequest
{
    public string Key { get; set; }
    public string Password { get; set; }
    public string Code { get; set; }   
}