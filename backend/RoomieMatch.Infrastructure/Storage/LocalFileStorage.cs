using Microsoft.Extensions.Configuration;

namespace RoomieMatch.Infrastructure.Storage;

public class LocalFileStorage : IFileStorage
{
    private readonly string _root;

    public LocalFileStorage(IConfiguration configuration)
    {
        _root = configuration.GetValue<string>("Storage:LocalPath") ?? Path.Combine(AppContext.BaseDirectory, "uploads");
        Directory.CreateDirectory(_root);
    }

    public async Task<string> SaveAsync(Stream stream, string contentType, string fileName, CancellationToken cancellationToken = default)
    {
        var safeName = Path.GetFileNameWithoutExtension(fileName);
        var extension = Path.GetExtension(fileName);
        var finalName = $"{safeName}-{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(_root, finalName);
        await using var fileStream = File.Create(fullPath);
        await stream.CopyToAsync(fileStream, cancellationToken);
        return $"/uploads/{finalName}";
    }
}
