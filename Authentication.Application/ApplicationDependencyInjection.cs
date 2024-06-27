using System.Reflection;
using Authentication.Application.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.Application;


public static class ApplicationDependencyInjection
{
    public static IServiceCollection ApplicationExtension(this IServiceCollection services) => services
        .AddUseCases()
        .AddMapper();
    
    private static IServiceCollection AddUseCases(this IServiceCollection services) => services.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()); });
    
    public static IServiceCollection AddMapper(this IServiceCollection services) => 
        services
            .AddAutoMapper(typeof(AuthenticationProfile));
}