using MediatR;
using SmartFlow.Application.Common.Exceptions;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Requests.Commands.AssignManager;

public sealed class AssignManagerCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser)
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
        if (!currentUser.IsInRole(Roles.Administrateur))
        {
            throw new ForbiddenAccessException(
                "Only an administrator can assign a manager.");
}
        request.AssignManager(
            command.ManagerId,
            currentUser.UserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}