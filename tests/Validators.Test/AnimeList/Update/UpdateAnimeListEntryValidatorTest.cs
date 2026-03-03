using CommonTestUtilities.Requests;
using UserAnimeList.Application.UseCases.AnimeList.Update;
using UserAnimeList.Communication.Enums;
using UserAnimeList.Exception;

namespace Validators.Test.AnimeList.Update;

public class UpdateAnimeListEntryValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new UpdateAnimeListEntryValidator();
        var request = RequestUpdateAnimeListEntryJsonBuilder.Build();
        
        var result = validator.Validate(request);
        
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
    
    [Fact]
    public void Error_Status_Not_Supported()
    {
        var validator = new UpdateAnimeListEntryValidator();
        var request = RequestUpdateAnimeListEntryJsonBuilder.Build();
        request.Status = (AnimeEntryStatus)100;
        
        var result = validator.Validate(request);
        
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.ANIME_LIST_INVALID_STATUS, result.Errors.First().ErrorMessage);
    }
    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Error_Score_Invalid(int  score)
    {
        var validator = new UpdateAnimeListEntryValidator();
        var request = RequestUpdateAnimeListEntryJsonBuilder.Build();
        request.Score = score;
        
        var result = validator.Validate(request);
        
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.ANIME_LIST_SCORE_INVALID, result.Errors.First().ErrorMessage);
    }
    
    [Fact]
    public void Error_Progress_Invalid()
    {
        var validator = new UpdateAnimeListEntryValidator();
        var request = RequestUpdateAnimeListEntryJsonBuilder.Build();
        request.Progress = -1;
        
        var result = validator.Validate(request);
        
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.EPISODE_COUNT_INVALID, result.Errors.First().ErrorMessage);
    }
    
    [Fact]
    public void Error_AiredUntil_Earlier_AiredFrom()
    {
        var validator = new UpdateAnimeListEntryValidator();
        var request = RequestUpdateAnimeListEntryJsonBuilder.Build();
        request.DateFinished = request.DateStarted!.Value.AddMonths(-3);
        
        var result = validator.Validate(request);
        
        var errors = result.Errors.Distinct().ToList();
        
        Assert.False(result.IsValid);
        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.ANIME_LIST_DATE_FINISHED_EARLIER_DATE_STARTED, errors.First().ErrorMessage);
    }
}