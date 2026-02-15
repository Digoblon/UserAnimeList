using CommonTestUtilities.Requests;
using UserAnimeList.Application.UseCases.Anime.Filter;
using UserAnimeList.Communication.Enums;
using UserAnimeList.Exception;

namespace Validators.Test.Anime.Filter;

public class AnimeFilterValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new AnimeFilterValidator();
        var request = RequestAnimeFilterJsonBuilder.Build();

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Error_Query_Exceeded()
    {
        var validator = new AnimeFilterValidator();
        var request = RequestAnimeFilterJsonBuilder.Build();
        request.Query = RequestStringGenerator.Paragraphs(minCharacters: 257);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.ANIME_QUERY_EXCEEDED, result.Errors.First().ErrorMessage);
    }

    [Fact]
    public void Error_Status_Not_Supported()
    {
        var validator = new AnimeFilterValidator();
        var request = RequestAnimeFilterJsonBuilder.Build();
        request.Status = (AnimeStatus)100;

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.STATUS_NOT_SUPPORTED, result.Errors.First().ErrorMessage);
    }

    [Fact]
    public void Error_Type_Not_Supported()
    {
        var validator = new AnimeFilterValidator();
        var request = RequestAnimeFilterJsonBuilder.Build();
        request.Type = (AnimeType)100;

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.TYPE_NOT_SUPPORTED, result.Errors.First().ErrorMessage);
    }

    [Fact]
    public void Error_SortField_Not_Supported()
    {
        var validator = new AnimeFilterValidator();
        var request = RequestAnimeFilterJsonBuilder.Build();
        request.SortField = (AnimeSort)100;

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.ANIME_INVALID_SORT_FIELD, result.Errors.First().ErrorMessage);
    }

    [Fact]
    public void Error_SortDirection_Not_Supported()
    {
        var validator = new AnimeFilterValidator();
        var request = RequestAnimeFilterJsonBuilder.Build();
        request.SortDirection = (SortDirection)100;

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.ANIME_INVALID_SORT_DIR, result.Errors.First().ErrorMessage);
    }

    [Fact]
    public void Error_PremieredYear_Required_When_PremieredSeason_Is_Filled()
    {
        var validator = new AnimeFilterValidator();
        var request = RequestAnimeFilterJsonBuilder.Build();
        request.PremieredSeason = Season.Winter;
        request.PremieredYear = null;

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.ANIME_PREMIERED_YEAR_REQUIRED, result.Errors.First().ErrorMessage);
    }

    [Fact]
    public void Error_PremieredSeason_Required_When_PremieredYear_Is_Filled()
    {
        var validator = new AnimeFilterValidator();
        var request = RequestAnimeFilterJsonBuilder.Build();
        request.PremieredSeason = null;
        request.PremieredYear = 2024;

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.ANIME_PREMIERED_SEASON_REQUIRED, result.Errors.First().ErrorMessage);
    }

    [Fact]
    public void Error_Invalid_PremieredSeason()
    {
        var validator = new AnimeFilterValidator();
        var request = RequestAnimeFilterJsonBuilder.Build();
        request.PremieredSeason = (Season)99;
        request.PremieredYear = 2024;

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.INVALID_SEASON, result.Errors.First().ErrorMessage);
    }

    [Fact]
    public void Error_AiredUntil_Earlier_AiredFrom()
    {
        var validator = new AnimeFilterValidator();
        var request = RequestAnimeFilterJsonBuilder.Build();
        request.AiredFrom = DateOnly.FromDateTime(DateTime.UtcNow);
        request.AiredUntil = request.AiredFrom.Value.AddDays(-5);

        var result = validator.Validate(request);

        var errors = result.Errors.Distinct().ToList();

        Assert.False(result.IsValid);
        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.ANIME_AIRED_UNTIL_EARLY_FROM, errors.First().ErrorMessage);
    }

    [Fact]
    public void Error_Duplicated_Genres()
    {
        var validator = new AnimeFilterValidator();
        var request = RequestAnimeFilterJsonBuilder.Build();
        var genreId = Guid.NewGuid();
        request.Genres = [genreId, genreId];

        var result = validator.Validate(request);

        var errors = result.Errors.Distinct().ToList();

        Assert.False(result.IsValid);
        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.DUPLICATED_GENRES, errors.First().ErrorMessage);
    }

    [Fact]
    public void Error_Duplicated_Studios()
    {
        var validator = new AnimeFilterValidator();
        var request = RequestAnimeFilterJsonBuilder.Build();
        var studioId = Guid.NewGuid();
        request.Studios = [studioId, studioId];

        var result = validator.Validate(request);

        var errors = result.Errors.Distinct().ToList();

        Assert.False(result.IsValid);
        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.DUPLICATED_STUDIOS, errors.First().ErrorMessage);
    }
}
