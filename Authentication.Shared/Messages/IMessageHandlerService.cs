namespace Shared.Messages;

public interface IMessageHandlerService
{
    List<MessageBase> GetWarnings();
    List<MessageBase> GetErrors();
    bool HasErrors { get; }
    bool HasWarnings { get; }
    void AddError(MessageBase messageBase);
    ErrorBuilder AddError();
    WarningBuilder AddWarning();
    void AddWarning(MessageBase warning);
}