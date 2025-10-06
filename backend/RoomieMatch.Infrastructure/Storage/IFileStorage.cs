namespace RoomieMatch.Infrastructure.Storage;

public interface IFileStorage
{
    Task<string> SaveAsync(Stream stream, string contentType, string fileName, CancellationToken cancellationToken = default);
}
