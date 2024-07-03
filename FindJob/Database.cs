using Microsoft.EntityFrameworkCore;

namespace FindJob;

internal class Database : DbContext
{
    public Database(DbContextOptions<Database> options) : base(options)
    { }

    public DbSet<Vacansy> Vacansies => Set<Vacansy>();
}
