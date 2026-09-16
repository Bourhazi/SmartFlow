    using MediatR;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Requests.Commands.ApproveRequest;

public sealed class ApproveRequestCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<ApproveRequestCommand, bool>
{
    public async Task<bool> Handle(
        ApproveRequestCommand command,
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

        request.Approve(
            currentUser.UserId,
            command.DecisionComment);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}