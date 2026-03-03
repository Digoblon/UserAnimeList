using CommonTestUtilities.Requests;
using UserAnimeList.Application.UseCases.Anime;
using UserAnimeList.Communication.Enums;
using UserAnimeList.Exception;

namespace Validators.Test.Anime;

public class AnimeValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new AnimeValidator();
        var request = RequestAnimeJsonBuilder.Build();
        
        var result = validator.Validate(request);
        
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("       ")]
    [InlineData("")]
    public void Error_Name_Empty(string? name)
    {
        var validator = new AnimeValidator();
        var request = RequestAnimeJsonBuilder.Build();
        request.Name = name!;
        
        var result = validator.Validate(request);
        
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.NAME_EMPTY, result.Errors.First().ErrorMessage);
    }
    
    [Fact]
    public void Error_Anime_Exceeds_Limit()
    {
        var validator = new AnimeValidator();
        var request = RequestAnimeJsonBuilder.Build();
        request.Name = RequestStringGenerator.Paragraphs(minCharacters: 257);
        
        var result = validator.Validate(request);
        
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.ANIME_EXCEEDS_LIMIT, result.Errors.First().ErrorMessage);
    }
    
    [Fact]
    public void Error_Synopsis_Exceeds_Limit()
    {
        var validator = new AnimeValidator();
        var request = RequestAnimeJsonBuilder.Build();
        request.Synopsis = RequestStringGenerator.Paragraphs(minCharacters: 5001);
        
        var result = validator.Validate(request);
        
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.SYNOPSIS_EXCEEDS_LIMIT, result.Errors.First().ErrorMessage);
    }
    
    [Fact]
    public void Error_Episode_Count_Invalid()
    {
        var validator = new AnimeValidator();
        var request = RequestAnimeJsonBuilder.Build();
        request.Episodes = -1;
        
        var result = validator.Validate(request);
        
        var errors = result.Errors.Distinct().ToList();
        
        Assert.False(result.IsValid);
        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.EPISODE_COUNT_INVALID, errors.First().ErrorMessage);
    }
    
    [Fact]
    public void Error_AnimeStatus_Not_Supported()
    {
        var validator = new AnimeValidator();
        var request = RequestAnimeJsonBuilder.Build();
        request.Status = (AnimeStatus)100;
        
        var result = validator.Validate(request);
        
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.STATUS_NOT_SUPPORTED, result.Errors.First().ErrorMessage);
    }
    [Fact]
    public void Error_AnimeType_Not_Supported()
    {
        var validator = new AnimeValidator();
        var request = RequestAnimeJsonBuilder.Build();
        request.Type = (AnimeType)100;
        
        var result = validator.Validate(request);
        
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.TYPE_NOT_SUPPORTED, result.Errors.First().ErrorMessage);
    }
    [Fact]
    public void Error_OriginalSource_Not_Supported()
    {
        var validator = new AnimeValidator();
        var request = RequestAnimeJsonBuilder.Build();
        request.Source = (SourceType)100;
        
        var result = validator.Validate(request);
        
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.SOURCE_NOT_SUPPORTED, result.Errors.First().ErrorMessage);
    }
    
    [Fact]
    public void Error_Duplicated_Genres()
    {
        var validator = new AnimeValidator();
        var request = RequestAnimeJsonBuilder.Build();
        request.Genres.Add(request.Genres.First());
        
        var result = validator.Validate(request);
        
        var errors = result.Errors.Distinct().ToList();
        
        Assert.False(result.IsValid);
        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.DUPLICATED_GENRES, errors.First().ErrorMessage);
    }
    
    [Fact]
    public void Error_AiredUntil_Filled_AiredFrom_Not_Filled()
    {
        var validator = new AnimeValidator();
        var request = RequestAnimeJsonBuilder.Build();
        request.Status = (AnimeStatus)2;
        request.AiredFrom = null;
        
        var result = validator.Validate(request);
        
        var errors = result.Errors.Distinct().ToList();
        
        Assert.False(result.IsValid);
        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.ANIME_AIRED_UNTIL_FILLED_AIRED_FROM_NOT_FILLED, errors.First().ErrorMessage);
    }
    
    [Fact]
    public void Error_AiredUntil_Earlier_AiredFrom()
    {
        var validator = new AnimeValidator();
        var request = RequestAnimeJsonBuilder.Build();
        request.Status = AnimeStatus.NotYetAired;
        request.AiredUntil = request.AiredFrom!.Value.AddMonths(-3);
        
        var result = validator.Validate(request);
        
        var errors = result.Errors.Distinct().ToList();
        
        Assert.False(result.IsValid);
        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.ANIME_AIRED_UNTIL_EARLY_FROM, errors.First().ErrorMessage);
    }
    
    [Fact]
    public void Error_Anime_Finished_AiredFrom_Required()
    {
        var validator = new AnimeValidator();
        var request = RequestAnimeJsonBuilder.Build();
        request.Status = AnimeStatus.Finished;
        request.AiredFrom = null;
        
        var result = validator.Validate(request);
        
        var errors = result.Errors.Distinct().ToList();
        
        Assert.False(result.IsValid);
        Assert.Contains(ResourceMessagesException.ANIME_AIRED_FROM_REQUIRED_WHEN_FINISHED,errors.Select(e => e.ErrorMessage));
    }
    
    [Fact]
    public void Error_Anime_Finished_AiredUntil_Required()
    {
        var validator = new AnimeValidator();
        var request = RequestAnimeJsonBuilder.Build();
        request.Status = AnimeStatus.Finished;
        request.AiredUntil = null;
        
        var result = validator.Validate(request);
        
        var errors = result.Errors.Distinct().ToList();
        
        Assert.False(result.IsValid);
        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.ANIME_AIRED_UNTIL_REQUIRED_WHEN_FINISHED, errors.First().ErrorMessage);
    }
    
    [Fact]
    public void Error_Anime_Finished_Episode_Null()
    {
        var validator = new AnimeValidator();
        var request = RequestAnimeJsonBuilder.Build();
        request.Status = AnimeStatus.Finished;
        request.Episodes = null!;
        
        var result = validator.Validate(request);
        
        var errors = result.Errors.Distinct().ToList();
        
        Assert.False(result.IsValid);
        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.EPISODE_MUST_BE_SPECIFIED, errors.First().ErrorMessage);
    }
    
    [Fact]
    public void Error_Anime_Finished_Episode_Invalid()
    {
        var validator = new AnimeValidator();
        var request = RequestAnimeJsonBuilder.Build();
        request.Status = AnimeStatus.Finished;
        request.Episodes = 0;
        
        var result = validator.Validate(request);
        
        var errors = result.Errors.Distinct().ToList();
        
        Assert.False(result.IsValid);
        Assert.Single(errors);
        Assert.Contains(ResourceMessagesException.EPISODE_COUNT_INVALID,errors.Select(e => e.ErrorMessage));
    }
    
    [Fact]
    public void Error_Anime_Airing_AiredFrom_Null()
    {
        var validator = new AnimeValidator();
        var request = RequestAnimeJsonBuilder.Build();
        request.Status = AnimeStatus.Airing;
        request.AiredFrom = null;
        request.AiredUntil = null;
        
        var result = validator.Validate(request);
        
        var errors = result.Errors.Distinct().ToList();
        
        Assert.False(result.IsValid);
        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.ANIME_AIRED_FROM_REQUIRED_WHEN_AIRING, errors.First().ErrorMessage);
    }
    
}