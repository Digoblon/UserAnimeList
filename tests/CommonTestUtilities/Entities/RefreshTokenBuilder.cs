using Bogus;
using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.ValueObjects;

namespace CommonTestUtilities.Entities;

public class RefreshTokenBuilder
{
    public static RefreshToken Build(User user)
    {
        return new Faker<RefreshToken>()
            .RuleFor(r => r.Id, _ => Guid.NewGuid())
            .RuleFor(r => r.CreatedOn, f => f.Date.Soon(days: -UserAnimeListConstants.RefreshTokenExpirationDays))
            .RuleFor(r => r.Token, f => f.Lorem.Word())
            .RuleFor(r => r.UserId, _ => user.Id)
            .RuleFor(r => r.User, _ => user);
    }
}