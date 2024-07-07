namespace Authentication.Shared.Exceptions;

public class BadRequestException: Exception
{
    public BadRequestException(object customErrorObject): base(("Bad Request"))
    {
        CustomErrorObject = customErrorObject;
    }

    public object CustomErrorObject { get; }
}