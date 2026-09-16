using MediatR;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Requests.Commands.UpdateRequest;

public sealed class UpdateRequestCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<UpdateRequestCommand, bool>
{
    public async Task<bool> Handle(
        UpdateRequestCommand command,
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

        request.Update(
            command.Title,
            command.Description,
            command.Priority,
            command.DueDate,
            currentUser.UserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}