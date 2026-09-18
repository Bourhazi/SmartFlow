using FluentValidation;

namespace SmartFlow.Application.AuditLogs.Queries.GetAuditLogs;

public sealed class GetAuditLogsQueryValidator
    : AbstractValidator<GetAuditLogsQuery>
{
    public GetAuditLogsQueryValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThan(0);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(query => query.EntityType)
            .MaximumLength(100)
            .When(query => !string.IsNullOrWhiteSpace(
                query.EntityType));
    }
}