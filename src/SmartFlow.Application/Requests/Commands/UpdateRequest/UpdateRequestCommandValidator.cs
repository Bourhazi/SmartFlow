using FluentValidation;

namespace SmartFlow.Application.Requests.Commands.UpdateRequest;

public sealed class UpdateRequestCommandValidator
    : AbstractValidator<UpdateRequestCommand>
{
    public UpdateRequestCommandValidator()
    {
        RuleFor(command => command.RequestId)
            .NotEmpty();

        RuleFor(command => command.Title)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(command => command.Description)
            .NotEmpty()
            .MaximumLength(3000);

        RuleFor(command => command.Priority)
            .IsInEnum();

        RuleFor(command => command.Version)
        .Must(version => version > 0)
        .WithMessage("Version must be greater than zero.");

        RuleFor(command => command.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .When(command => command.DueDate.HasValue);
    }
}