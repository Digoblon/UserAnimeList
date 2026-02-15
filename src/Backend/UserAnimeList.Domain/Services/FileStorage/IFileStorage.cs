namespace UserAnimeList.Domain.Services.FileStorage;

public interface IFileStorage
{
    public void Delete(string filePath);
    Task<string> Save(Type entityType, Guid entityId, Stream fileStream,string fileExtension);
}