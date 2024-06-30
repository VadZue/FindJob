using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace FindJob;

internal class HhApiClient
{
    private readonly HttpClient _client;

    public HhApiClient()
    {
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

    

    private async Task<TParsed?> Send<TParsed>(HttpRequestMessage request) 
    {
        try
        {
            using var response = await _client.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<TParsed>(content);

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

public class HhResponse 
{
    public Vacansy[] items { get; set; }
    public int found { get; set; }
    public int page { get; set; }
    public int pages { get; set; }
    public int per_page { get; set; }
    public string? alternate_url { get; set; }
}

public class Vacansy
{
    public bool? accept_temporary { get; set; }
    public Address address { get; set; }
    public bool? archived { get; set; }
    public Contacts contacts { get; set; }
    public DateTime created_at { get; set; }
    public bool has_test { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public bool? premium { get; set; }
    public DateTime published_at { get; set; }
    public bool? response_letter_required { get; set; }
    public Salary salary { get; set; }


}


public class Address 
{ 
    public string? building { get; set; }
    public string? city { get; set; }
    public string? description { get; set; }
    public string? id { get; set; }
    public string? raw { get; set; }
    public string? street { get; set; }
}


public class Contacts
{
    public bool? call_tracking_enabled { get; set; }
    public string? email { get; set; }
    public string? name { get; set; }

}

public class Salary
{
    public string? currency { get; set; }
    public int? from { get; set; }
    public bool? gross { get; set; }
    public int? to { get; set; }

}




