using MediatR;

namespace SmartFlow.Application.Requests.Commands.StartReview;

public sealed record StartReviewCommand(
    Guid RequestId,
    Guid ManagerId,
    uint Version) : IRequest<bool>;