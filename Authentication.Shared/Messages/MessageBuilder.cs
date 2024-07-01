using System.Net;

namespace Shared.Messages;

public class MessageBuilder
{
    protected MessageBase _message;
    protected IMessageHandlerService _service;

    public MessageBuilder(IMessageHandlerService service)
    {
        _service = service;
        _message = new MessageBase();
    }

    public MessageBuilder WithMessage(string message)
    {
        _message.Message = message;
        return this;
    }

    public MessageBuilder WithErrorCode(string errorCode)
    {
        _message.Code = errorCode;
        return this;
    }

    public MessageBuilder WithStatusCode(HttpStatusCode statusCode)
    {
        _message.StatusCode = statusCode;
        _message.CustomStatusCode = true;
        return this;
    }
    
    public MessageBuilder WithStackTrace(string stackTrace)
    {
        _message.StackTrace = stackTrace;
        return this;
    }


    public MessageBuilder ThrowsException(Exception exception)
    {
        _message.ThrowException = true;
        _message.Exception = exception;
        return this;
    }
    
    public void Commit() => FinalizeAndAddToService();

    protected void FinalizeAndAddToService()
    {
        switch (this)
        {
            case ErrorBuilder:
                _service.AddError(_message);
                break;
            case WarningBuilder:
                _service.AddWarning(_message);
                break;
        }
    }
}

public class ErrorBuilder : MessageBuilder
{
    public ErrorBuilder(IMessageHandlerService service) : base(service)
    {
    }
}

public class WarningBuilder : MessageBuilder
{
    public WarningBuilder(IMessageHandlerService service) : base(service)
    {
    }
}