using Bogus;
using UserAnimeList.Domain.Entities;

namespace CommonTestUtilities.Entities;

public class StudioBuilder
{
    public static Studio Build()
    {
        var studio = new Faker<Studio>()
            .RuleFor(studio => studio.Id, Guid.NewGuid)
            .RuleFor(studio => studio.Name, (f) => f.Company.CompanyName())
            .RuleFor(studio => studio.NameNormalized, (f,s) => s.Name.ToLower())
            .RuleFor(studio => studio.Description, (f) => f.Lorem.Paragraph());

        return studio;

    }
}