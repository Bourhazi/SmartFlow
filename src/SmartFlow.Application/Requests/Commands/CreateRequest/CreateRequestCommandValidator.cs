using FluentValidation;

namespace SmartFlow.Application.Requests.Commands.CreateRequest;

public sealed class CreateRequestCommandValidator
    : AbstractValidator<CreateRequestCommand>
{
    public CreateRequestCommandValidator()
    {
        RuleFor(command => command.Title)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(command => command.Description)
            .NotEmpty()
            .MaximumLength(3000);

        RuleFor(command => command.Priority)
            .IsInEnum();



        RuleFor(command => command.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .When(command => command.DueDate.HasValue);
    }
}