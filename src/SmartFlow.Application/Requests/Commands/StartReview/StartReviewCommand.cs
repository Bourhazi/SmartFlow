using MediatR;

namespace SmartFlow.Application.Requests.Commands.StartReview;

public sealed record StartReviewCommand(
    Guid RequestId,
    uint Version) : IRequest<bool>;