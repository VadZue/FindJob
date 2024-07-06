using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FindJob;

public class Database(DbContextOptions<Database> options) : DbContext(options)
{
    public DbSet<Vacansy> Vacansies => Set<Vacansy>();

    public async Task MergeVacancies(IEnumerable<Vacansy> vacansies, CancellationToken token = default)
    {
        using var tran = await Database.BeginTransactionAsync(token);

        try
        {
            var ids = vacansies.Select(x => x.Id).ToList();
            var finded = await Vacansies
                .Where(x => ids.Contains(x.Id))
                .ToListAsync(token);

            foreach(var updateTarget in finded)
            {
                var source = vacansies.First(x => x.Id == updateTarget.Id);
                Update(source, updateTarget);
                Vacansies.Update(updateTarget);
            }

            var add = vacansies.Where(x => !finded.Exists(u => u.Id == x.Id)).ToList();

            Vacansies.AddRange(add);
            await tran.CommitAsync(token);
            await SaveChangesAsync(token);
        }
        catch (Exception)
        {
            await tran.RollbackAsync(token);
            throw;
        }
    }

    public async Task<IList<TopCity>> GetTopCitiesByVacanyCount(int top = 10)
    {
        return await Vacansies
            .Where(x => x.Address != null && x.Address.City != null)
            .GroupBy(x => x.Address.City)
            .OrderByDescending(x => x.Count())
            .Take(top)
            .Select(x => new TopCity(x.Key, x.Count()))
            .ToListAsync();
    }

    public async Task<double?> GetAvgSalary()
    {
        return await Vacansies.Where(x => x.Salary.To != null).AverageAsync(x => x.Salary.To);
    }

    public BiggestAndLowestSalaryVacancies GetBiggestAndLowestSalary()
    {
        var min = Vacansies.MinBy(x => x.Salary.To);
        var max = Vacansies.MaxBy(x => x.Salary.To);

        return new(max, min);
    }
    public async Task<BiggestAndLowestSalaryVacancies> GetBiggestAndLowestSalaryAsync()
    {
        var min = await Vacansies.MinAsync(x => x.Salary.To);
        var max = await Vacansies.MaxAsync(x => x.Salary.To);
        var minVac = await Vacansies.FirstOrDefaultAsync(x => x.Salary.To == min);
        var maxVac = await Vacansies.FirstOrDefaultAsync(x => x.Salary.To == max);
        
        return new(minVac, maxVac);
    }


    private void Update<T>(T source, T destination)
    {
        foreach(var property in typeof(T).GetProperties())
        {
            var sourceValue = property.GetValue(source);
            property.SetValue(destination, sourceValue);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Vacansy>()
            .HasKey(x => x.Id);
    }
}

public readonly record struct BiggestAndLowestSalaryVacancies(Vacansy? Biggest, Vacansy? Lowest);

public readonly record struct TopCity(string Name, int VacancyCount);
