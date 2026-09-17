using FluentValidation;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Users.Commands.CreateUser;

public sealed class CreateUserCommandValidator
    : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(command => command.FullName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(command => command.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(100);

        RuleFor(command => command.Role)
            .Must(role => Roles.All.Contains(role))
            .WithMessage("Role is invalid.");
    }
}