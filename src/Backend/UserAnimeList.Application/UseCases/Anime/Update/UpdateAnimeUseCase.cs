using FluentValidation.Results;
using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.Anime;
using UserAnimeList.Domain.Repositories.Genre;
using UserAnimeList.Domain.Repositories.Studio;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.Anime.Update;

public class UpdateAnimeUseCase : IUpdateAnimeUseCase
{
    private readonly IAppMapper _mapper;
    private readonly IAnimeRepository _animeRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IStudioRepository _studioRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public UpdateAnimeUseCase(IAppMapper mapper,
        IAnimeRepository animeRepository,
        IGenreRepository genreRepository,
        IStudioRepository studioRepository,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _animeRepository = animeRepository;
        _genreRepository = genreRepository;
        _studioRepository = studioRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Execute(RequestAnimeJson request, string id)
    {
        var anime = await _animeRepository.GetById(id);

        if (anime is null)
            throw new NotFoundException(ResourceMessagesException.ANIME_NOT_FOUND);
        
        await Validate(request, anime);
        
        var animeUpdated = _mapper.UpdateToAnime(anime, request);
        animeUpdated.NameNormalized = animeUpdated.Name.ToLower();
        
        animeUpdated.Genres.Clear(); 
        animeUpdated.Studios.Clear();
        
        foreach (var genreId in request.Genres)
        {
            animeUpdated.Genres.Add(new AnimeGenre { AnimeId = animeUpdated.Id, GenreId = genreId });
        }

        foreach (var studioId in request.Studios)
        {
            animeUpdated.Studios.Add(new AnimeStudio { AnimeId = animeUpdated.Id, StudioId = studioId });
        }

        _animeRepository.Update(animeUpdated);
        await _unitOfWork.Commit();

    }

    private async Task Validate(RequestAnimeJson request, Domain.Entities.Anime anime)
    {
        var validator = new AnimeValidator();
        
        var result = await validator.ValidateAsync(request);
        if (anime.Name != request.Name)
        {
            var nameExist = await _animeRepository.ExistsActiveAnimeWithName(request.Name);
            if (nameExist)
                result.Errors.Add(new ValidationFailure(string.Empty,
                    ResourceMessagesException.ANIME_ALREADY_REGISTERED));
        }

        if(result.IsValid)
        {
            await ValidateGenre(request, result);

            await ValidateStudio(request, result);
        }
        
        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
    private async Task ValidateStudio(RequestAnimeJson request, ValidationResult result)
    {
        var validStudios = await _studioRepository.GetStudiosInList(request.Studios);

        if (validStudios.Count != request.Studios.Count)
        {
            var validSet = validStudios.ToHashSet();

            var invalidStudiosList = request.Studios.Where(id => !validSet.Contains(id));

            var invalidStudios = String.Join(", ", invalidStudiosList);
            result.Errors.Add(new ValidationFailure(string.Empty, $"{ResourceMessagesException.INVALID_STUDIOS_IN_REQUEST}{invalidStudios}"));
        }
        
    }

    private async Task ValidateGenre(RequestAnimeJson request, ValidationResult result)
    {
        var validGenres = await _genreRepository.GetGenresInList(request.Genres);

        if (validGenres.Count != request.Genres.Count)
        {
            var validSet = validGenres.ToHashSet();

            var invalidGenresList = request.Genres.Where(id => !validSet.Contains(id));

            var invalidGenres = String.Join(", ", invalidGenresList);
            
            result.Errors.Add(new ValidationFailure(string.Empty, $"{ResourceMessagesException.INVALID_GENRES_IN_REQUEST}{invalidGenres}"));
        }
    }
    
}