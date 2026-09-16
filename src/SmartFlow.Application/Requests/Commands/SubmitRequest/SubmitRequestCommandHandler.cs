using MediatR;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Requests.Commands.SubmitRequest;

public sealed class SubmitRequestCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<SubmitRequestCommand, bool>
{
    public async Task<bool> Handle(
        SubmitRequestCommand command,
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

        request.Submit(currentUser.UserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}