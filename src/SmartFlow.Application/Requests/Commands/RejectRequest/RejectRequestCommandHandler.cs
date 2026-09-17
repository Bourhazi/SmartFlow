using MediatR;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Requests.Commands.RejectRequest;

public sealed class RejectRequestCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser)
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
            currentUser.UserId,
            command.RejectionReason);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}