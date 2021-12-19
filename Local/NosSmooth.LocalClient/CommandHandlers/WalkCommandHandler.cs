using NosSmooth.Core.Commands;
using Remora.Results;

namespace NosSmooth.LocalClient.CommandHandlers;

public class WalkCommandHandler : ICommandHandler<WalkCommand>
{
    public Task<Result> HandleCommand(WalkCommand command, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}