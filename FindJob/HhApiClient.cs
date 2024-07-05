using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace FindJob;

public class HhApiClient
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _serializerOptions;

    public HhApiClient()
    {
        _serializerOptions = new()
        { 
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        var socket = new SocketsHttpHandler
        {
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
            PooledConnectionLifetime = TimeSpan.FromMinutes(1)
        };

        _client = new(socket)
        {
            BaseAddress = new("https://api.hh.ru/")
        };
    }

    public async Task<HhResponse<Vacansy>?> GetVacancies(VacanciesRequest? request = null)
    {
        var values = ParseToQueryValues(request);
        var url = QueryHelpers.AddQueryString("vacancies", values);

        return await Send<HhResponse<Vacansy>>(new(HttpMethod.Get, url));
    }

    private readonly static DefaultContractResolver _snakeCase = new() { NamingStrategy = new SnakeCaseNamingStrategy() };
    private static IEnumerable<KeyValuePair<string, string?>> ParseToQueryValues<T>(T? request)
    {
        if (request is null) yield break;

        foreach (var property in typeof(T).GetProperties())
            yield return new(_snakeCase.GetResolvedPropertyName(property.Name), property.GetValue(request) as string);
    }

    private async Task<TParsed?> Send<TParsed>(HttpRequestMessage request) 
    {
        try
        {
            request.Headers.TryAddWithoutValidation("HH-User-Agent", "FindJob(pet project) (vadim-zue@lenta.ru)");

            using var response = await _client.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
#if DEBUG
            var str = await response.Content.ReadAsStringAsync();
#endif
            var content = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<TParsed>(content, _serializerOptions);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {   
            request.Dispose();
        }

        return default;
    }
}

public sealed class VacanciesRequest
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public long? Salary { get; set; }

    public bool OnlyWithSalary { get; set; }

    public Experience? Experience { get; set; }

    public Employment? Employment { get; set; }

    public Schedule? Schedule { get; set; }

    public DateOnly? DateFrom { get; set; }

    public DateOnly? DateTo { get; set; }
}

public enum Experience : byte
{
    noExperience,
    between1And3,
    between3And6,
    moreThan6,
}

public enum Employment : byte
{
    full,
    part,
    project,
    volunteer,
    probation,
}

public enum Schedule : byte
{
    fullDay,
    flexible,
    remote,
    flyInFlyOut,
}

public class HhResponse<TItems>
{
    public virtual List<TItems> Items { get; set; }

    public int Found { get; set; }
    
    public int Page { get; set; }
    
    public int Pages { get; set; }
    
    public int PerPage { get; set; }
    
    public string? AlternateUrl { get; set; }
}

public class Vacansy
{
    [Key]
    public string Id { get; set; }
    
    public bool? AcceptTemporary { get; set; }
    
    public Address? Address { get; set; }
    
    public bool? Archived { get; set; }
    
    public Contacts? Contacts { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public bool HasTest { get; set; }
    
    public string Name { get; set; }
    
    public bool? Premium { get; set; }
    
    public DateTime PublishedAt { get; set; }
    
    public bool? ResponseLetterRequired { get; set; }
    
    public Salary Salary { get; set; }
}

[Owned]
public class Address
{ 
    public string? Building { get; set; }
    
    public string? City { get; set; }
    
    public string? Description { get; set; }
    
    public string? Id { get; set; }
    
    public string? Raw { get; set; }
    
    public string? Street { get; set; }
}

[Owned]
public class Contacts
{
    public bool? CallTrackingEnabled { get; set; }
    
    public string? Email { get; set; }
    
    public string? Name { get; set; }
}

[Owned]
public class Salary
{
    public string? Currency { get; set; }
    
    public int? From { get; set; }
    
    public bool? Gross { get; set; }
    
    public int? To { get; set; }
}




