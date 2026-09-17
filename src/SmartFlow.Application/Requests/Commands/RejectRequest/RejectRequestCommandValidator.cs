using FluentValidation;

namespace SmartFlow.Application.Requests.Commands.RejectRequest;

public sealed class RejectRequestCommandValidator
    : AbstractValidator<RejectRequestCommand>
{
    public RejectRequestCommandValidator()
    {
        RuleFor(command => command.RequestId)
            .NotEmpty();



        RuleFor(command => command.RejectionReason)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(command => command.Version)
            .Must(version => version > 0)
            .WithMessage("Version must be greater than zero.");
    }
}