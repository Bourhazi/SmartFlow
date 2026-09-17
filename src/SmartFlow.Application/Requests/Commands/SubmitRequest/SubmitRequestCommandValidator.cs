using FluentValidation;

namespace SmartFlow.Application.Requests.Commands.SubmitRequest;

public sealed class SubmitRequestCommandValidator
    : AbstractValidator<SubmitRequestCommand>
{
    public SubmitRequestCommandValidator()
    {
        RuleFor(command => command.RequestId)
            .NotEmpty();


        RuleFor(command => command.Version)
            .Must(version => version > 0)
            .WithMessage("Version must be greater than zero.");
    }
}