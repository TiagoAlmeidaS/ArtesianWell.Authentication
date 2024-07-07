namespace Authentication.Shared.Exceptions;

public class UnauthorizedException: Exception
{
    public UnauthorizedException(object customErrorObject): base(("User not authorized"))
    {
        CustomErrorObject = customErrorObject;
    }

    public object CustomErrorObject { get; }
}