using Bogus;

namespace CommonTestUtilities.Requests;

public static class PasswordGenerator
{
    public static string GenerateValid(int length = 10)
    {
        var faker = new Faker();

        var upper = faker.Random.Char('A', 'Z');
        var lower = faker.Random.Char('a', 'z');
        var digit = faker.Random.Number(0, 9).ToString();

        var remainingLength = length - 3;
        
        if (remainingLength < 0)
            remainingLength = 0;

        var remaining = faker.Random.String2(
            remainingLength,
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
        );

        return new string($"{upper}{lower}{digit}{remaining}");

    }
}