using Microsoft.EntityFrameworkCore;

namespace UserAnimeList.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options);

