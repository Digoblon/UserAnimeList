using Moq;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.Repositories.AnimeList;

namespace CommonTestUtilities.Repositories;

public class AnimeListRepositoryBuilder
{
    private readonly Mock<IAnimeListRepository> _repository;
    private readonly IList<AnimeList> _animeList = [];

    public AnimeListRepositoryBuilder() => _repository = new Mock<IAnimeListRepository>();

    public void ExistsEntry(Anime anime, User user)
    {
        _repository
            .Setup(r => r.ExistsEntry(user.Id, anime.Id))
            .ReturnsAsync(true);
    }

    public AnimeListRepositoryBuilder WithLists(IList<AnimeList> animeLists)
    {
        foreach (var list in animeLists)
        {
            _animeList.Add(list);
        }
    
        return this;
    }
    
    public AnimeListRepositoryBuilder GetById(string id, User user, AnimeList animeList)
    {
        _repository
            .Setup(x => x.GetById(id, user.Id))
            .ReturnsAsync(() =>
            {
                var animeListId = Guid.Parse(id);
                if (animeListId != animeList.Id && animeListId != animeList.AnimeId)
                    return null;
                
                return animeList;
            });

        return this;
    }

    public AnimeListRepositoryBuilder List(User user)
    {
        _repository
            .Setup(x => x.List(user.Id, It.IsAny<RequestAnimeListEntryFilterJson>())).ReturnsAsync(_animeList);
        
        return this;
    }
    
    public IAnimeListRepository Build() => _repository.Object;
}