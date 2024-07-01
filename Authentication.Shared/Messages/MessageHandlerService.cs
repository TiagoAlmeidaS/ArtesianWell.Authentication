namespace Shared.Messages;

public class MessageHandlerService: IMessageHandlerService
{
    private readonly List<MessageBase> _errors = new List<MessageBase>();
    private readonly List<MessageBase> _warnings = new List<MessageBase>();

    public bool HasWarnings => _warnings.Any();
    public bool HasErrors => _errors.Any();
    public List<MessageBase> GetErrors() => _errors;
    public List<MessageBase> GetWarnings() => _warnings;

    public ErrorBuilder AddError()
    {
        return new ErrorBuilder(this);
    }

    public void AddError(MessageBase error)
    {
        _errors.Add(error);
    }

    public WarningBuilder AddWarning()
    {
        return new WarningBuilder(this);
    }

    public void AddWarning(MessageBase warning)
    {
        _warnings.Add(warning);
    }
}