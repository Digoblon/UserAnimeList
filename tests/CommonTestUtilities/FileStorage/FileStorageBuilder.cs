using Bogus;
using Moq;
using UserAnimeList.Domain.Services.FileStorage;

namespace CommonTestUtilities.FileStorage;

public class FileStorageBuilder
{
    private readonly Mock<IFileStorage> _mock;

    public FileStorageBuilder() => _mock = new Mock<IFileStorage>();
    
    public IFileStorage Build() => _mock.Object;

    
    public FileStorageBuilder Save()
    {
        var faker = new Faker();
        var imageUrl = faker.Image.LoremFlickrUrl(); 
        
        _mock.Setup(fileStorage => fileStorage.Save(It.IsAny<Type>(),It.IsAny<Guid>(),It.IsAny<Stream>(),It.IsAny<string>()))
            .ReturnsAsync(imageUrl);
        
        return this;
    }
}