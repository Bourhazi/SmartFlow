using MediatR;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Domain.Entities;
using SmartFlow.Application.Common.Security;
using SmartFlow.Application.Users;

using SmartFlow.Domain.Enums;


namespace SmartFlow.Application.Requests.Commands.CreateRequest;

public sealed class CreateRequestCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    INotificationRepository notificationRepository,
    IUserAdministrationService userAdministrationService,
    IAuditLogger auditLogger)
    : IRequestHandler<CreateRequestCommand, Guid>
{
    public async Task<Guid> Handle(
        CreateRequestCommand command,
        CancellationToken cancellationToken)
    {
        var request = Request.Create(
            command.Title,
            command.Description,
            command.Priority,
            command.DueDate,
            currentUser.UserId);

        await requestRepository.AddAsync(request, cancellationToken);

        var administrators =
        await userAdministrationService.GetActiveUserIdsInRoleAsync(
            Roles.Administrateur,
            cancellationToken);

        foreach (var administratorId in administrators)
        {
            await notificationRepository.AddAsync(
                Notification.Create(
                    "New request created",
                    $"A new request titled '{request.Title}' was created.",
                    NotificationType.NewRequest,
                    administratorId,
                    request.Id),
                cancellationToken);
        }

        await auditLogger.WriteAsync(
        "RequestCreated",
        "Request",
        request.Id,
        null,
        new
        {
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            request.CreatorId,
            request.Status
        },
        cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return request.Id;
    }
}