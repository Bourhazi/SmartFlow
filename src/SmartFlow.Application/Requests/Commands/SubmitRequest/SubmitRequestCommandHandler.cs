using MediatR;
using SmartFlow.Application.Common.Interfaces;

namespace SmartFlow.Application.Requests.Commands.SubmitRequest;

public sealed class SubmitRequestCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork)
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

        request.Submit(command.CurrentUserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}