using FluentValidation;

namespace SmartFlow.Application.Requests.Commands.UploadAttachment;

public sealed class UploadAttachmentCommandValidator
    : AbstractValidator<UploadAttachmentCommand>
{
    public UploadAttachmentCommandValidator()
    {
        RuleFor(command => command.RequestId)
            .NotEmpty();

        RuleFor(command => command.OriginalFileName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(command => command.ContentType)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(command => command.Size)
            .GreaterThan(0)
            .LessThanOrEqualTo(5 * 1024 * 1024);

        RuleFor(command => command.UploadedById)
            .NotEmpty();

        RuleFor(command => command.Version)
            .Must(version => version > 0)
            .WithMessage("Version must be greater than zero.");
    }
}