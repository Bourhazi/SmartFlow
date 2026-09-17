using FluentValidation;

namespace SmartFlow.Application.Requests.Commands.ApproveRequest;

public sealed class ApproveRequestCommandValidator
    : AbstractValidator<ApproveRequestCommand>
{
    public ApproveRequestCommandValidator()
    {
        RuleFor(command => command.RequestId)
            .NotEmpty();
            
        RuleFor(command => command.DecisionComment)
            .MaximumLength(1000)
            .When(command => !string.IsNullOrWhiteSpace(
                command.DecisionComment));

        RuleFor(command => command.Version)
            .Must(version => version > 0)
            .WithMessage("Version must be greater than zero.");
    }
}