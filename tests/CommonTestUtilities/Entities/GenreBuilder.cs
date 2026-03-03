using Bogus;
using UserAnimeList.Domain.Entities;

namespace CommonTestUtilities.Entities;

public class GenreBuilder
{
    public static Genre Build()
    {
        var genre = new Faker<Genre>()
            .RuleFor(genre => genre.Id, Guid.NewGuid)
            .RuleFor(genre => genre.Name, (f) => f.Company.CompanyName())
            .RuleFor(genre => genre.NameNormalized, (f,s) => s.Name.ToLower())
            .RuleFor(genre => genre.Description, (f) => f.Lorem.Paragraph());

        return genre;

    }
}