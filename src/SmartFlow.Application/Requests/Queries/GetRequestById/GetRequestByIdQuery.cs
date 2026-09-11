using MediatR;

namespace SmartFlow.Application.Requests.Queries.GetRequestById;

public sealed record GetRequestByIdQuery(Guid RequestId)
    : IRequest<RequestDetailsDto?>;