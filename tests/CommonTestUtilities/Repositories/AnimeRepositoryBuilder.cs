using Moq;
using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.Repositories.Anime;

namespace CommonTestUtilities.Repositories;

public class AnimeRepositoryBuilder
{
    private readonly Mock<IAnimeRepository> _repository;
    private readonly List<Anime> _animes = [];
    
    public AnimeRepositoryBuilder() => _repository = new Mock<IAnimeRepository>();
    
    public void ExistsActiveAnimeWithId(string name)
    {
        _repository
            .Setup(repository => repository.ExistsActiveAnimeWithName(name))
            .ReturnsAsync(true);
    }

    public void AddList(IList<Anime> list)
    {
        foreach (var anime in list)
            _animes.Add(anime);
    }
    
    public AnimeRepositoryBuilder WithAnime(Anime anime)
    {
        _animes.Add(anime);
        return this;
    }
    
    public AnimeRepositoryBuilder GetById(Anime anime)
    {
        _repository
            .Setup(x => x.GetById(anime.Id.ToString()))
            .ReturnsAsync(anime);

        return this;
    }

    public AnimeRepositoryBuilder Search()
    {
        _repository
            .Setup(r => r.Search(It.IsAny<string>()))
            .ReturnsAsync((string query) =>
            {
                if(query.Equals("*"))
                    return _animes.ToList();
                
                if (string.IsNullOrEmpty(query))
                    return new List<Anime>();

                return _animes
                    .Where(a => a.DeletedOn == null && a.IsActive)
                    .Where(a => a.NameNormalized.Contains(query.ToLower()) || a.Synopsis.ToLower().Contains(query.ToLower()))
                    .ToList();
            });

        return this;
    }
    
    public IAnimeRepository Build() => _repository.Object;
}