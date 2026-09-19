using MediatR;

namespace SmartFlow.Application.Dashboard.Queries.GetDashboard;

public sealed record GetDashboardQuery
    : IRequest<DashboardDto>;