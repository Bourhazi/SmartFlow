    using MediatR;
using SmartFlow.Application.Common.Interfaces;

namespace SmartFlow.Application.Requests.Commands.ApproveRequest;

public sealed class ApproveRequestCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork)
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
            command.ManagerId,
            command.DecisionComment);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}