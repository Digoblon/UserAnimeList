using Moq;
using UserAnimeList.Communication.Requests;
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

    public AnimeRepositoryBuilder Filter()
    {
        _repository
            .Setup(r => r.Filter(It.IsAny<RequestAnimeFilterJson>()))
            .ReturnsAsync((RequestAnimeFilterJson filter) =>
            {
                var query = _animes
                    .Where(a => a.DeletedOn == null && a.IsActive)
                    .AsEnumerable();

                if (!string.IsNullOrWhiteSpace(filter.Query))
                {
                    var search = filter.Query.ToLower();
                    query = query.Where(a => a.NameNormalized.Contains(search));
                }

                if (filter.Status.HasValue)
                    query = query.Where(a => a.Status == (UserAnimeList.Domain.Enums.AnimeStatus)filter.Status.Value);

                if (filter.Type.HasValue)
                    query = query.Where(a => a.Type == (UserAnimeList.Domain.Enums.AnimeType)filter.Type.Value);

                if (filter.Genres is { Count: > 0 })
                    query = query.Where(a => a.Genres.Any(g => filter.Genres.Contains(g.GenreId)));

                if (filter.Studios is { Count: > 0 })
                    query = query.Where(a => a.Studios.Any(s => filter.Studios.Contains(s.StudioId)));

                return query.ToList();
            });

        return this;
    }
    
    public IAnimeRepository Build() => _repository.Object;
}
