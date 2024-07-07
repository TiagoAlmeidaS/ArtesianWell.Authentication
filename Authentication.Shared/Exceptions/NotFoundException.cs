namespace Authentication.Shared.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(object customErrorObject): base(("Not found"))
    {
        CustomErrorObject = customErrorObject;
    }

    public object CustomErrorObject { get; }
}