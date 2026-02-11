using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.Enums;
using UserAnimeList.Domain.Security.Cryptography;
using UserAnimeList.Domain.Services.DataSeed;
using UserAnimeList.Enums;
using UserAnimeList.Infrastructure.Data.Seed.DTOs;

namespace UserAnimeList.Infrastructure.Data.Seed;

//CLASSE GERADA POR INTELIGÊNCIA ARTIFICIAL
//Gerada para popular os dados no banco em ambientes de teste baseado em arquivos json
public sealed class DatabaseSeeder : IDatabaseSeeder
{
    private readonly UserAnimeListDbContext _context;
    private readonly IPasswordEncrypter _passwordEncrypter;
    private readonly IHostEnvironment _env;

    public DatabaseSeeder(
        UserAnimeListDbContext context,
        IPasswordEncrypter passwordHasher,
        IHostEnvironment env)
    {
        _context = context;
        _passwordEncrypter = passwordHasher;
        _env = env;
    }

    public async Task SeedAsync()
    {
        var seedFolder = Path.Combine(_env.ContentRootPath, "Data", "Seed", "SeedData");

        await SeedUsers(Path.Combine(seedFolder, "users.json"));
        await _context.SaveChangesAsync();

        await SeedStudios(Path.Combine(seedFolder, "studios.json"));
        await SeedGenres(Path.Combine(seedFolder, "genres.json"));

        await SeedAnimes(Path.Combine(seedFolder, "animes.json"));
        await _context.SaveChangesAsync();

        await SeedAnimeListsRandomlyFromUsersJson(
            usersJsonPath: Path.Combine(seedFolder, "users.json"),
            minEntriesPerUser: 3,
            maxEntriesPerUser: 15
        );

        await _context.SaveChangesAsync();
    }

    private async Task SeedUsers(string path)
    {
        var items = await LoadJson<List<SeedUserJson>>(path);
        if (items is null || items.Count == 0) return;

        var emails = items
            .Select(x => x.Email.Trim().ToLowerInvariant())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        var existingEmails = await _context.Users
            .AsNoTracking()
            .Where(u => emails.Contains(u.Email.ToLower()))
            .Select(u => u.Email.ToLower())
            .ToListAsync();

        var existingSet = existingEmails.ToHashSet();

        foreach (var dto in items)
        {
            var emailNorm = dto.Email.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(emailNorm)) continue;

            if (existingSet.Contains(emailNorm))
                continue;

            var role = Enum.TryParse<UserRole>(dto.Role, ignoreCase: true, out var parsed)
                ? parsed
                : UserRole.User;

            var user = new User
            {
                UserName = dto.UserName.Trim(),
                Email = dto.Email.Trim(),
                Password = _passwordEncrypter.Encrypt(dto.Password),
                Role = role
            };

            _context.Users.Add(user);
            existingSet.Add(emailNorm);
        }
    }

    private async Task SeedStudios(string path)
    {
        var items = await LoadJson<List<SeedStudioJson>>(path);
        if (items is null || items.Count == 0) return;

        var namesNorm = items
            .Select(x => x.Name.Trim().ToLowerInvariant())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        var existing = await _context.Studios
            .AsNoTracking()
            .Where(s => namesNorm.Contains(s.NameNormalized))
            .Select(s => s.NameNormalized)
            .ToListAsync();

        var existingSet = existing.ToHashSet();

        foreach (var dto in items)
        {
            var name = dto.Name.Trim();
            if (string.IsNullOrWhiteSpace(name)) continue;

            var normalized = name.ToLowerInvariant();
            if (existingSet.Contains(normalized))
                continue;

            _context.Studios.Add(new Studio
            {
                Name = name,
                NameNormalized = normalized,
                Description = dto.Description?.Trim() ?? string.Empty
            });

            existingSet.Add(normalized);
        }
    }

    private async Task SeedGenres(string path)
    {
        var items = await LoadJson<List<SeedGenreJson>>(path);
        if (items is null || items.Count == 0) return;

        var namesNorm = items
            .Select(x => x.Name.Trim().ToLowerInvariant())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        var existing = await _context.Genres
            .AsNoTracking()
            .Where(g => namesNorm.Contains(g.NameNormalized))
            .Select(g => g.NameNormalized)
            .ToListAsync();

        var existingSet = existing.ToHashSet();

        foreach (var dto in items)
        {
            var name = dto.Name.Trim();
            if (string.IsNullOrWhiteSpace(name)) continue;

            var normalized = name.ToLowerInvariant();
            if (existingSet.Contains(normalized))
                continue;

            _context.Genres.Add(new Genre
            {
                Name = name,
                NameNormalized = normalized,
                Description = dto.Description?.Trim() ?? string.Empty
            });

            existingSet.Add(normalized);
        }
    }

    private async Task SeedAnimes(string path)
    {
        var items = await LoadJson<List<SeedAnimeJson>>(path);
        if (items is null || items.Count == 0) return;

        var genresDict = await _context.Genres
            .AsNoTracking()
            .Where(g => g.DeletedOn == null && g.IsActive)
            .ToDictionaryAsync(g => g.NameNormalized, g => g.Id);

        var studiosDict = await _context.Studios
            .AsNoTracking()
            .Where(s => s.DeletedOn == null && s.IsActive)
            .ToDictionaryAsync(s => s.NameNormalized, s => s.Id);

        var animeNamesNorm = items
            .Select(x => x.Name.Trim().ToLowerInvariant())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        var existingAnimes = await _context.Animes
            .AsNoTracking()
            .Where(a => animeNamesNorm.Contains(a.NameNormalized))
            .Select(a => a.NameNormalized)
            .ToListAsync();

        var existingSet = existingAnimes.ToHashSet();

        foreach (var dto in items)
        {
            var name = dto.Name.Trim();
            if (string.IsNullOrWhiteSpace(name)) continue;

            var nameNorm = name.ToLowerInvariant();
            if (existingSet.Contains(nameNorm))
                continue;

            var anime = new Anime
            {
                Name = name,
                NameNormalized = nameNorm,
                Synopsis = dto.Synopsis?.Trim() ?? string.Empty,
                Episodes = dto.Episodes,
                Status = dto.Status,
                Source = dto.Source,
                Type = dto.Type,
                AiredFrom = dto.AiredFrom,
                AiredUntil = dto.AiredUntil
            };

            var uniqueGenres = dto.Genres
                .Select(x => x.Trim().ToLowerInvariant())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct();

            foreach (var gn in uniqueGenres)
            {
                if (genresDict.TryGetValue(gn, out var genreId))
                    anime.Genres.Add(new AnimeGenre { GenreId = genreId });
            }

            var uniqueStudios = dto.Studios
                .Select(x => x.Trim().ToLowerInvariant())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct();

            foreach (var sn in uniqueStudios)
            {
                if (studiosDict.TryGetValue(sn, out var studioId))
                    anime.Studios.Add(new AnimeStudio { StudioId = studioId });
            }

            _context.Animes.Add(anime);
            existingSet.Add(nameNorm);
        }
    }

    private async Task SeedAnimeListsRandomlyFromUsersJson(
    string usersJsonPath,
    int minEntriesPerUser = 3,
    int maxEntriesPerUser = 15)
    {
        if (minEntriesPerUser < 0) minEntriesPerUser = 0;
        if (maxEntriesPerUser < minEntriesPerUser) maxEntriesPerUser = minEntriesPerUser;

        var seedUsers = await LoadJson<List<SeedUserJson>>(usersJsonPath);
        if (seedUsers is null || seedUsers.Count == 0)
            return;

        var emails = seedUsers
            .Select(u => u.Email.Trim().ToLowerInvariant())
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Distinct()
            .ToList();

        if (emails.Count == 0)
            return;

        var users = await _context.Users
            .AsNoTracking()
            .Where(u => u.DeletedOn == null && u.IsActive && emails.Contains(u.Email.ToLower()))
            .Select(u => new { u.Id })
            .ToListAsync();

        if (users.Count == 0)
            return;

        var animes = await _context.Animes
            .AsNoTracking()
            .Where(a => a.DeletedOn == null && a.IsActive)
            .Select(a => new { a.Id, a.Episodes })
            .ToListAsync();

        if (animes.Count == 0)
            return;
        
        var userIds = users.Select(u => u.Id).ToList();
        var animeIds = animes.Select(a => a.Id).ToList();

        var existingPairs = await _context.AnimeLists
            .AsNoTracking()
            .Where(x => userIds.Contains(x.UserId) && animeIds.Contains(x.AnimeId))
            .Select(x => new { x.UserId, x.AnimeId })
            .ToListAsync();

        var existingSet = existingPairs
            .Select(x => (x.UserId, x.AnimeId))
            .ToHashSet();

        var statusValues = Enum.GetValues<AnimeEntryStatus>();
        var rng = new Random(1337);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        foreach (var user in users)
        {
            var countForUser = rng.Next(minEntriesPerUser, maxEntriesPerUser + 1);
            countForUser = Math.Min(countForUser, animes.Count);

            var chosen = animes
                .OrderBy(_ => rng.Next())
                .Take(countForUser)
                .ToList();

            foreach (var anime in chosen)
            {
                if (existingSet.Contains((user.Id, anime.Id)))
                    continue;

                var status = statusValues[rng.Next(statusValues.Length)];

                int? score = rng.NextDouble() < 0.15 ? null : rng.Next(1, 11);

                var eps = anime.Episodes;
                int? progress = null;

                DateOnly? started = null;
                DateOnly? finished = null;

                var startDate = today.AddDays(-rng.Next(10, 400));
                if (status != AnimeEntryStatus.PlanToWatch)
                    started = startDate;

                if (eps is > 0)
                {
                    switch (status)
                    {
                        case AnimeEntryStatus.Completed:
                            progress = eps;
                            finished = started?.AddDays(rng.Next(3, 120));
                            break;

                        case AnimeEntryStatus.Watching:
                            progress = eps == 1 ? 1 : rng.Next(1, eps.Value);
                            break;

                        case AnimeEntryStatus.Dropped:
                        case AnimeEntryStatus.OnHold:
                            progress = eps == 1 ? 1 : rng.Next(1, Math.Min(eps.Value, 10));
                            break;

                        case AnimeEntryStatus.PlanToWatch:
                            progress = null;
                            finished = null;
                            break;
                    }
                }
                else
                {
                    progress = null;
                    if (status == AnimeEntryStatus.Completed)
                        finished = started?.AddDays(rng.Next(5, 180));
                }

                if (started is null)
                    finished = null;

                _context.AnimeLists.Add(new AnimeList
                {
                    UserId = user.Id,
                    AnimeId = anime.Id,
                    Status = status,
                    Score = score,
                    Progress = progress,
                    DateStarted = started,
                    DateFinished = finished
                });

                existingSet.Add((user.Id, anime.Id));
            } 
        }
    }
    
/*
private async Task SeedAnimeListsRandomly(int entriesPerUser = 10)
{
    var users = await _context.Users
        .AsNoTracking()
        .Where(u => u.DeletedOn == null && u.IsActive)
        .Select(u => new { u.Id })
        .ToListAsync();

    if (users.Count == 0)
        return;

    var animes = await _context.Animes
        .AsNoTracking()
        .Where(a => a.DeletedOn == null && a.IsActive)
        .Select(a => new { a.Id, a.Episodes })
        .ToListAsync();

    if (animes.Count == 0)
        return;

    var userIds = users.Select(u => u.Id).ToList();
    var animeIds = animes.Select(a => a.Id).ToList();

    var existingPairs = await _context.AnimeLists
        .AsNoTracking()
        .Where(x => userIds.Contains(x.UserId) && animeIds.Contains(x.AnimeId))
        .Select(x => new { x.UserId, x.AnimeId })
        .ToListAsync();

    var existingSet = existingPairs
        .Select(x => (x.UserId, x.AnimeId))
        .ToHashSet();

    var statusValues = Enum.GetValues<AnimeEntryStatus>();
    var rng = new Random(1337);

    var maxPerUser = Math.Max(1, Math.Min(entriesPerUser, animes.Count));

    var today = DateOnly.FromDateTime(DateTime.UtcNow);

    foreach (var user in users)
    {
        var chosen = animes
            .OrderBy(_ => rng.Next())
            .Take(maxPerUser)
            .ToList();

        foreach (var anime in chosen)
        {
            if (existingSet.Contains((user.Id, anime.Id)))
                continue;

            var status = statusValues[rng.Next(statusValues.Length)];

            int? score = rng.NextDouble() < 0.15 ? null : rng.Next(1, 11);

            var eps = anime.Episodes;
            int? progress = null;

            DateOnly? started = null;
            DateOnly? finished = null;

            var startDate = today.AddDays(-rng.Next(10, 400));

            if (status != AnimeEntryStatus.PlanToWatch)
                started = startDate;

            if (eps is > 0)
            {
                switch (status)
                {
                    case AnimeEntryStatus.Completed:
                        progress = eps;
                        finished = started?.AddDays(rng.Next(3, 120));
                        break;

                    case AnimeEntryStatus.Watching:
                        progress = eps == 1 ? 1 : rng.Next(1, eps.Value);
                        break;

                    case AnimeEntryStatus.Dropped:
                    case AnimeEntryStatus.OnHold:
                        progress = eps == 1 ? 1 : rng.Next(1, Math.Min(eps.Value, 10));
                        break;

                    case AnimeEntryStatus.PlanToWatch:
                        progress = null;
                        finished = null;
                        break;
                }
            }
            else
            {
                progress = null;

                if (status == AnimeEntryStatus.Completed)
                    finished = started?.AddDays(rng.Next(5, 180));
            }

            if (started is null)
                finished = null;

            _context.AnimeLists.Add(new AnimeList
            {
                UserId = user.Id,
                AnimeId = anime.Id,
                Status = status,
                Score = score,
                Progress = progress,
                DateStarted = started,
                DateFinished = finished
            });

            existingSet.Add((user.Id, anime.Id));
        }
    }   


}
*/
    private static async Task<T?> LoadJson<T>(string path)
    {
        if (!File.Exists(path))
            return default;

        var json = await File.ReadAllTextAsync(path);

        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        });
    }
}