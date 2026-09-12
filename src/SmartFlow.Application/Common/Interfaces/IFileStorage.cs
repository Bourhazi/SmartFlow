namespace SmartFlow.Application.Common.Interfaces;

public interface IFileStorage
{
    Task<string> SaveAsync(
        Stream content,
        string originalFileName,
        CancellationToken cancellationToken);

    Task<Stream?> OpenReadAsync(
        string storageFileName,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        string storageFileName,
        CancellationToken cancellationToken);
}