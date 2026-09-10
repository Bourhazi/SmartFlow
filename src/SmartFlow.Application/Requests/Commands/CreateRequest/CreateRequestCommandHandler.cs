using MediatR;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Domain.Entities;

namespace SmartFlow.Application.Requests.Commands.CreateRequest;

public sealed class CreateRequestCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork)
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
            command.CreatorId);

        await requestRepository.AddAsync(request, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return request.Id;
    }
}