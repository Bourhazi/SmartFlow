using FluentValidation;

namespace SmartFlow.Application.Requests.Commands.RemoveAttachment;

public sealed class RemoveAttachmentCommandValidator
    : AbstractValidator<RemoveAttachmentCommand>
{
    public RemoveAttachmentCommandValidator()
    {
        RuleFor(command => command.RequestId)
            .NotEmpty();

        RuleFor(command => command.AttachmentId)
            .NotEmpty();


        RuleFor(command => command.Version)
            .Must(version => version > 0)
            .WithMessage("Version must be greater than zero.");
    }
}