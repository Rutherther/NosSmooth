
    /// <summary>
    /// Adds the given game event responder.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="gameResponder">The type of the event responder.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddGameResponder(this IServiceCollection serviceCollection, Type gameResponder)
    {
        if (!gameResponder.GetInterfaces().Any(
                i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IGameResponder<>)
            ))
        {
            throw new ArgumentException(
                $"{nameof(gameResponder)} should implement IGameResponder.",
                nameof(gameResponder));
        }

        var handlerTypeInterfaces = gameResponder.GetInterfaces();
        var handlerInterfaces = handlerTypeInterfaces.Where
        (
            r => r.IsGenericType && r.GetGenericTypeDefinition() == typeof(IGameResponder<>)
        );

        foreach (var handlerInterface in handlerInterfaces)
        {
            serviceCollection.AddScoped(handlerInterface, gameResponder);
        }

        return serviceCollection;
    }
}