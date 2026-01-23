using Moq;
using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.Repositories.Genre;

public class GenreRepositoryBuilder
{
    private readonly Mock<IGenreRepository> _repository;
    private readonly List<Genre> _genres = [];

    public GenreRepositoryBuilder() => _repository = new Mock<IGenreRepository>();

    public void ExistsActiveGenreWithName(string name)
    {
        _repository
            .Setup(repository => repository.ExistsActiveGenreWithName(name))
            .ReturnsAsync(true);
    }

    public GenreRepositoryBuilder GetById(Genre genre)
    {
        _repository
            .Setup(x => x.GetById(genre.Id.ToString()))
            .ReturnsAsync(genre);

        return this;
    }
    public GenreRepositoryBuilder SearchByName(Genre genre)
    {
        _genres.Add(genre);

        _repository
            .Setup(r => r.SearchByName(It.IsAny<string>()))
            .ReturnsAsync((string name) =>
            {
                if (string.IsNullOrEmpty(name))
                    return new List<Genre>();

                return _genres
                    .Where(s => s.Name.Contains(name))
                    .ToList();
            });

        return this;
    }

    public IGenreRepository Build() => _repository.Object;
}