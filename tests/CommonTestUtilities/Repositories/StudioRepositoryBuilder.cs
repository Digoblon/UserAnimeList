using Moq;
using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.Repositories.Studio;

public class StudioRepositoryBuilder
{
    private readonly Mock<IStudioRepository> _repository;
    private readonly List<Studio> _studios = [];

    public StudioRepositoryBuilder() => _repository = new Mock<IStudioRepository>();
    
    public StudioRepositoryBuilder WithStudio(Studio studio)
    {
        _studios.Add(studio);
        return this;
    }

    public void ExistsActiveStudioWithName(string name)
    {
        _repository
            .Setup(repository => repository.ExistsActiveStudioWithName(name))
            .ReturnsAsync(true);
    }

    public StudioRepositoryBuilder GetById(Studio studio)
    {
        _repository
            .Setup(x => x.GetById(studio.Id.ToString()))
            .ReturnsAsync(studio);

        return this;
    }
    public StudioRepositoryBuilder SearchByName(Studio studio)
    {
        _studios.Add(studio);

        _repository
            .Setup(r => r.SearchByName(It.IsAny<string>()))
            .ReturnsAsync((string name) =>
            {
                if (string.IsNullOrEmpty(name))
                    return new List<Studio>();

                return _studios
                    .Where(s => s.Name.Contains(name))
                    .ToList();
            });

        return this;
    }
    
    public StudioRepositoryBuilder GetStudiosInList()
    {
        _repository
            .Setup(r => r.GetStudiosInList(It.IsAny<IList<Guid>>()))
            .ReturnsAsync((IList<Guid> ids) =>
            {
                if (ids is null || ids.Count == 0)
                    return new List<Guid>();

                return _studios
                    .Where(g => ids.Contains(g.Id))
                    .Where(g => g.DeletedOn == null && g.IsActive)
                    .Select(g => g.Id)
                    .ToList();
            });

        return this;
    }

    public IStudioRepository Build() => _repository.Object;
}