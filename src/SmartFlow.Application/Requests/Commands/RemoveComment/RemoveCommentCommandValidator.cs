using FluentValidation;

namespace SmartFlow.Application.Requests.Commands.RemoveComment;

public sealed class RemoveCommentCommandValidator
    : AbstractValidator<RemoveCommentCommand>
{
    public RemoveCommentCommandValidator()
    {
        RuleFor(command => command.RequestId)
            .NotEmpty();

        RuleFor(command => command.CommentId)
            .NotEmpty();

        RuleFor(command => command.CurrentUserId)
            .NotEmpty();

        RuleFor(command => command.Version)
            .Must(version => version > 0)
            .WithMessage("Version must be greater than zero.");
    }
}