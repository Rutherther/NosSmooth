using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Core.Errors;
using Remora.Results;

namespace NosSmooth.Core.Commands;

public class CommandProcessor
{
    private readonly IServiceProvider _provider;

    public CommandProcessor(IServiceProvider provider)
    {
        _provider = provider;
    }

    public Task<Result> ProcessCommand(ICommand command, CancellationToken ct = default)
    {
        var processMethod = GetType().GetMethod
        (
            nameof(DispatchCommandHandler),
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        if (processMethod is null)
        {
            throw new InvalidOperationException("Could not find process command generic method in command processor.");
        }

        var boundProcessMethod = processMethod.MakeGenericMethod(command.GetType());

        return (Task<Result>)boundProcessMethod.Invoke(this, new object[] { command, ct })!;
    }

    private Task<Result> DispatchCommandHandler<TCommand>(TCommand command, CancellationToken ct = default)
        where TCommand : class, ICommand
    {
        using var scope = _provider.CreateScope();
        var commandHandler = scope.ServiceProvider.GetService<ICommandHandler<TCommand>>();
        if (commandHandler is null)
        {
            return Task.FromResult(Result.FromError(new CommandHandlerNotFound(command.GetType())));
        }

        return commandHandler.HandleCommand(command, ct);
    }
}