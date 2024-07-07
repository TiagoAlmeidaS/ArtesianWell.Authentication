using Authentication.Application;
using Authentication.Infra.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.Function.External;

public static class ServiceExtension
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services
            .ApplicationExtension()
            .InfraServiceExtension(configuration);
    }
}