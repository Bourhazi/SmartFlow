using FluentValidation;

namespace SmartFlow.Application.Requests.Commands.AssignManager;

public sealed class AssignManagerCommandValidator
    : AbstractValidator<AssignManagerCommand>
{
    public AssignManagerCommandValidator()
    {
        RuleFor(command => command.RequestId)
            .NotEmpty();

        RuleFor(command => command.ManagerId)
            .NotEmpty();

        RuleFor(command => command.Version)
            .Must(version => version > 0)
            .WithMessage("Version must be greater than zero.");
    }
}