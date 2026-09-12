using FluentValidation;

namespace SmartFlow.Application.Requests.Queries.GetRequests;

public sealed class GetRequestsQueryValidator
    : AbstractValidator<GetRequestsQuery>
{
    public GetRequestsQueryValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThan(0);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(query => query.Search)
            .MaximumLength(150)
            .When(query => !string.IsNullOrWhiteSpace(query.Search));

        RuleFor(query => query.Status)
            .Must(status =>
                !status.HasValue ||
                Enum.IsDefined(status.Value))
            .WithMessage("Status is invalid.");

        RuleFor(query => query.Priority)
            .Must(priority =>
                !priority.HasValue ||
                Enum.IsDefined(priority.Value))
            .WithMessage("Priority is invalid.");

        RuleFor(query => query.SortBy)
            .IsInEnum();

        RuleFor(query => query.SortDirection)
            .IsInEnum();
    }
}