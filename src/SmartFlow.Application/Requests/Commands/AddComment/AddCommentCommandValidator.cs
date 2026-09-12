using FluentValidation;

namespace SmartFlow.Application.Requests.Commands.AddComment;

public sealed class AddCommentCommandValidator
    : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(command => command.RequestId)
            .NotEmpty();

        RuleFor(command => command.Content)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(command => command.AuthorId)
            .NotEmpty();

        RuleFor(command => command.Version)
            .Must(version => version > 0)
            .WithMessage("Version must be greater than zero.");
    }
}