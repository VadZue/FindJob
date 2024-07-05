using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace FindJob;

public class Database(DbContextOptions<Database> options) : DbContext(options)
{
    public DbSet<Vacansy> Vacansies => Set<Vacansy>();

    private async Task MergeVacancies(IEnumerable<Vacansy> vacansies, CancellationToken token = default)
    {
        using var tran = await Database.BeginTransactionAsync(token);

        try
        {
            var update = await Vacansies
                .Where(x => vacansies.Any(y => y.Id == x.Id))
                .ToListAsync(token);

            var insert = vacansies.Where(x => !update.Exists(u => u.Id == x.Id));

            await Vacansies.AddRangeAsync(insert, token);
            await tran.CommitAsync(token);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            await tran.RollbackAsync(token);

            return;
        }
    }
}

public sealed class InsertVacansyResponse
{
    public int Inserted { get; set; }
    
    public int Updated { get; set; }
}
