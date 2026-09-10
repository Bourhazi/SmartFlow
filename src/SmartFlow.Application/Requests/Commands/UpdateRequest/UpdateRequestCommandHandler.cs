using MediatR;
using SmartFlow.Application.Common.Interfaces;

namespace SmartFlow.Application.Requests.Commands.UpdateRequest;

public sealed class UpdateRequestCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork)
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
            command.CurrentUserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}