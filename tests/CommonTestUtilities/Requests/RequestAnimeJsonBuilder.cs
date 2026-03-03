using Bogus;
using UserAnimeList.Communication.Enums;
using UserAnimeList.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestAnimeJsonBuilder
{
    public static RequestAnimeJson Build()
    {
        return new Faker<RequestAnimeJson>()
            .RuleFor(a => a.Name, f => f.Lorem.Word())
            .RuleFor(a => a.Synopsis, f => f.Lorem.Paragraph())
            .RuleFor(a => a.Episodes, f => f.Random.Int(1, 100))
            .RuleFor(a => a.Status, f => f.PickRandom<AnimeStatus>())
            .RuleFor(a => a.Source, f => f.PickRandom<SourceType>())
            .RuleFor(a => a.Type, f => f.PickRandom<AnimeType>())
            .RuleFor(a => a.AiredFrom, f => f.Date.PastDateOnly())
            .RuleFor(a => a.AiredUntil, f => f.Date.FutureDateOnly())
            .RuleFor(a => a.Genres, f => f.Make(3, Guid.NewGuid).ToList())
            .RuleFor(a => a.Studios, f => f.Make(3, Guid.NewGuid).ToList());
    }
}