using MediatR;
using SmartFlow.Application.Common.Interfaces;

namespace SmartFlow.Application.Requests.Commands.RejectRequest;

public sealed class RejectRequestCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RejectRequestCommand, bool>
{
    public async Task<bool> Handle(
        RejectRequestCommand command,
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

        request.Reject(
            command.ManagerId,
            command.RejectionReason);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}