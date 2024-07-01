using Microsoft.Extensions.DependencyInjection;

namespace Shared.Messages;

public static class DependencyInjection
{
    public static IServiceCollection AddMessageHandling(this IServiceCollection services)
        => services.AddScoped<IMessageHandlerService, MessageHandlerService>();
}