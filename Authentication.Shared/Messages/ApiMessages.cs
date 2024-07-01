using System.Net;

namespace Shared.Messages;

public class MessageBase
{
    public string Code { get; set; }
    public string Message { get; set; }
    public bool ThrowException { get; set; } = false;
    public Exception Exception { get; set; }
    public bool CustomStatusCode { get; set; } = false;
    public HttpStatusCode StatusCode { get; set; }
    public string StackTrace { get; set; }
}