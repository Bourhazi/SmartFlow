using FluentValidation;

namespace SmartFlow.Application.Requests.Commands.StartReview;

public sealed class StartReviewCommandValidator
    : AbstractValidator<StartReviewCommand>
{
    public StartReviewCommandValidator()
    {
        RuleFor(command => command.RequestId)
            .NotEmpty();

        RuleFor(command => command.Version)
            .Must(version => version > 0)
            .WithMessage("Version must be greater than zero.");
    }
}