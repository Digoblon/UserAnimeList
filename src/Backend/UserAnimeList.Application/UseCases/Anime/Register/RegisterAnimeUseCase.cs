using FluentValidation.Results;
using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.Anime;
using UserAnimeList.Domain.Repositories.Genre;
using UserAnimeList.Domain.Repositories.Studio;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.Anime.Register;

public class RegisterAnimeUseCase : IRegisterAnimeUseCase
{
    private readonly IAppMapper _mapper;
    private readonly IAnimeRepository _animeRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IStudioRepository  _studioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterAnimeUseCase(IAppMapper mapper, 
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
    
    public async Task<ResponseRegisteredAnimeJson> Execute(RequestAnimeJson request)
    {
        await Validate(request);
        
        var anime = _mapper.Map<Domain.Entities.Anime>(request);
        anime.NameNormalized = anime.Name.ToLower();
        
        foreach (var genreId in request.Genres)
        {
            anime.Genres.Add(new AnimeGenre { GenreId = genreId });
        }

        foreach (var studioId in request.Studios)
        {
            anime.Studios.Add(new AnimeStudio { StudioId = studioId });
        }

        await _animeRepository.Add(anime);

        await _unitOfWork.Commit();
        
        return new ResponseRegisteredAnimeJson
        {
            AnimeId = anime.Id,
            Name = request.Name,
        };
    }



    private async Task Validate(RequestAnimeJson request)
    {
        var validator = new AnimeValidator();
        var result = await validator.ValidateAsync(request);
        
        var nameExist = await _animeRepository.ExistsActiveAnimeWithName(request.Name);
        if(nameExist)
            result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.ANIME_ALREADY_REGISTERED));

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