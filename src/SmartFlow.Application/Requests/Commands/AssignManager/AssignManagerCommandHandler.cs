using MediatR;
using SmartFlow.Application.Common.Exceptions;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Security;
using SmartFlow.Application.Users;

namespace SmartFlow.Application.Requests.Commands.AssignManager;

public sealed class AssignManagerCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork,
    IUserAdministrationService userAdministrationService,
    ICurrentUser currentUser)
    : IRequestHandler<AssignManagerCommand, bool>
{
    public async Task<bool> Handle(
        AssignManagerCommand command,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsInRole(Roles.Administrateur))
        {
            throw new ForbiddenAccessException(
                "Only an administrator can assign a manager.");
        }

        var isManager = await userAdministrationService.IsInRoleAsync(
            command.ManagerId,
            Roles.Manager,
            cancellationToken);

        if (!isManager)
        {
            throw new InvalidOperationException(
                "The selected user must have the Manager role.");
        }

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
            currentUser.UserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}