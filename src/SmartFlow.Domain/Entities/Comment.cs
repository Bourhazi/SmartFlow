using SmartFlow.Domain.Common;
using SmartFlow.Domain.Exceptions;

namespace SmartFlow.Domain.Entities;

public sealed class Comment : BaseEntity
{
    private Comment()
    {
    }

    private Comment(
        Guid requestId,
        string content,
        Guid authorId)
    {
        RequestId = requestId;
        Content = content;
        AuthorId = authorId;
    }

    public Guid RequestId { get; private set; }

    public string Content { get; private set; } = string.Empty;

    public Guid AuthorId { get; private set; }

    internal static Comment Create(
        Guid requestId,
        string content,
        Guid authorId)
    {
        if (requestId == Guid.Empty)
        {
            throw new DomainException("Request identifier is required.");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new DomainException("Comment content is required.");
        }

        if (authorId == Guid.Empty)
        {
            throw new DomainException("Comment author is required.");
        }

        return new Comment(requestId, content.Trim(), authorId);
    }
}