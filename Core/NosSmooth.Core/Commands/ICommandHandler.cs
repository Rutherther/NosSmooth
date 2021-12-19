using System.Threading;
using System.Threading.Tasks;
using Remora.Results;

namespace NosSmooth.Core.Commands;

public interface ICommandHandler {}

public interface ICommandHandler<TCommand> : ICommandHandler
    where TCommand : ICommand
{
    public Task<Result> HandleCommand(TCommand command, CancellationToken ct = default);
}