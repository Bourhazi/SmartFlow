using SmartFlow.Application.Common.Interfaces;

namespace SmartFlow.Infrastructure.FileStorage;

public sealed class LocalFileStorage(string rootPath) : IFileStorage
{
    private readonly string _rootPath = rootPath;

    public async Task<string> SaveAsync(
        Stream content,
        string originalFileName,
        CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_rootPath);

        var extension = Path.GetExtension(originalFileName);

        var storageFileName =
            $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";

        var filePath = GetFilePath(storageFileName);

        await using var destination = new FileStream(
            filePath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None);

        await content.CopyToAsync(destination, cancellationToken);

        return storageFileName;
    }

    public Task<Stream?> OpenReadAsync(
        string storageFileName,
        CancellationToken cancellationToken)
    {
        var filePath = GetFilePath(storageFileName);

        if (!File.Exists(filePath))
        {
            return Task.FromResult<Stream?>(null);
        }

        Stream fileStream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read);

        return Task.FromResult<Stream?>(fileStream);
    }

    public Task DeleteAsync(
        string storageFileName,
        CancellationToken cancellationToken)
    {
        var filePath = GetFilePath(storageFileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }

    private string GetFilePath(string storageFileName)
    {
        var safeFileName = Path.GetFileName(storageFileName);

        if (!string.Equals(
                safeFileName,
                storageFileName,
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Invalid storage file name.");
        }

        return Path.Combine(_rootPath, safeFileName);
    }
}