using Microsoft.AspNetCore.Hosting;
using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.Services.FileStorage;
using UserAnimeList.Domain.ValueObjects;

namespace UserAnimeList.Infrastructure.Services.LocalFileStorage;

public class LocalFileStorage : IFileStorage
{
    private readonly IWebHostEnvironment _env;

    public LocalFileStorage(IWebHostEnvironment env)
    {
        _env = env;
    }
    public void Delete(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return;

        var normalized = relativePath.Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar)
            .TrimStart(Path.DirectorySeparatorChar);

        var physicalPath = Path.Combine(_env.WebRootPath, normalized);

        if (File.Exists(physicalPath))
            File.Delete(physicalPath);
    }

    public async Task<string> Save(Type entityType, Guid entityId, Stream stream, string fileExtension)
    {
        var folderEntity = entityType == typeof(User) ? "users"
            : entityType == typeof(Anime) ? "animes"
            : string.Empty;

        if (string.IsNullOrWhiteSpace(folderEntity))
            return string.Empty;

        var extension = fileExtension.StartsWith('.') ? fileExtension : $".{fileExtension}";
        extension = extension.ToLowerInvariant();

        var fileName = $"{Guid.NewGuid()}{extension}";

        var relativeFolderUrl = $"{UserAnimeListConstants.ImageFilePath}/{folderEntity}/{entityId}";
        var relativeFileUrl = $"{relativeFolderUrl}/{fileName}";

        var physicalFolder = Path.Combine(_env.WebRootPath, UserAnimeListConstants.ImageFilePath, folderEntity, entityId.ToString());
        Directory.CreateDirectory(physicalFolder);

        var physicalFile = Path.Combine(physicalFolder, fileName);

        if (stream.CanSeek)
            stream.Position = 0;

        await using var fileStream = new FileStream(physicalFile, FileMode.Create, FileAccess.Write, FileShare.None);
        await stream.CopyToAsync(fileStream);

        return relativeFileUrl;
    }
}