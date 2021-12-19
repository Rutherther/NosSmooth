using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NosSmooth.Core.Client;
using NosSmooth.Core.Extensions;
using NosSmooth.LocalClient.CommandHandlers;

namespace NosSmooth.LocalClient.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLocalClient(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddNostaleCore();
        serviceCollection.AddCommandHandler<WalkCommandHandler>();
        
        serviceCollection.TryAddSingleton<INostaleClient, NostaleLocalClient>();

        return serviceCollection;
    }
}