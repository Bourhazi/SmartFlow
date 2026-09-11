using MediatR;
using SmartFlow.Application.Common.Interfaces;

namespace SmartFlow.Application.Requests.Commands.StartReview;

public sealed class StartReviewCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<StartReviewCommand, bool>
{
    public async Task<bool> Handle(
        StartReviewCommand command,
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

        request.StartReview(command.ManagerId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}