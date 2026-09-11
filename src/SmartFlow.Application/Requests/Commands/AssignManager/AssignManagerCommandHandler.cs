using MediatR;
using SmartFlow.Application.Common.Interfaces;

namespace SmartFlow.Application.Requests.Commands.AssignManager;

public sealed class AssignManagerCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AssignManagerCommand, bool>
{
    public async Task<bool> Handle(
        AssignManagerCommand command,
        CancellationToken cancellationToken)
    {
        var request = await requestRepository.GetForUpdateAsync(
            command.RequestId,
            command.Version,
            cancellationToken);

        if (request is null)
        {
            return false;
        }

        request.AssignManager(
            command.ManagerId,
            command.PerformedById);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}