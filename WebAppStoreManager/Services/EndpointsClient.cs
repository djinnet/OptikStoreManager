using Core.Models;
using System.Text;
using Newtonsoft.Json;
using System.Globalization;

namespace WebAppStoreManager.Services;

/// <summary>
/// This is the class for httpclient for poco classes RetailChain and RetailStore
/// The ids are Guid-based.
/// The class are interacting with crud endpoints:
/// * api/store
/// * api/store/GetSingle
/// * api/chain
/// * api/chain/GetSingle
/// </summary>
public class EndpointsClient : IEndpointsClient
{
    private readonly HttpClient _httpClient;
    public JsonSerializer serializer { get; }
    public EndpointsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://localhost:7195/");
        serializer = new JsonSerializer();
    }

    private T Deserialize<T>(string json)
    {
        using var stringReader = new StringReader(json);
        using var jsonReader = new JsonTextReader(stringReader);
        return serializer.Deserialize<T>(jsonReader);
    }

    public string Serialize<T>(T data) where T : class
    {
        using var stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
        using (var jsonTextWriter = new JsonTextWriter(stringWriter))
        {
            jsonTextWriter.Formatting = serializer.Formatting;
            serializer.Serialize(jsonTextWriter, data, typeof(T));
        }

        return stringWriter.ToString();
    }

    public async Task<List<RetailStore>?> GetStoresAsync()
    {
        var response = await _httpClient.GetAsync("api/store");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return Deserialize<List<RetailStore>>(content);
    }

    public async Task<RetailStore?> GetStoreAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"api/store/{id}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return Deserialize<RetailStore>(content);
    }

    public async Task<List<RetailChain>?> GetChainsAsync()
    {
        var response = await _httpClient.GetAsync("api/chain");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return Deserialize<List<RetailChain>>(content);
    }

    public async Task<RetailChain?> GetChainAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"api/chain/{id}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return Deserialize<RetailChain>(content);
    }

    public async Task<Response<RetailStore>?> CreateStoreAsync(RetailStore store)
    {
        var json = Serialize(store);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/store", data);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return Deserialize<Response<RetailStore>>(content);
    }

    public async Task<Response<RetailStore>?> UpdateStoreAsync(RetailStore store)
    {
        var json = Serialize(store);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync("api/store/"+store.Id, data);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return Deserialize<Response<RetailStore>>(content);
    }
    
    public async Task<Response<string>?> DeleteStoreAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/store/{id}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return Deserialize<Response<string>>(content);
    }

    public async Task<Response<RetailChain>?> CreateChainAsync(RetailChain chain)
    {
        var json = Serialize(chain);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/chain", data);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return Deserialize<Response<RetailChain>>(content);
    }

    public async Task<Response<RetailChain>?> UpdateChainAsync(RetailChain chain)
    {
        var json = Serialize(chain);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync("api/chain/"+chain.Id, data);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return Deserialize<Response<RetailChain>>(content);
    }

    public async Task<Response<string>?> DeleteChainAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/chain/{id}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return Deserialize<Response<string>>(content);
    }
}

public class Response<T>
{
    public T result { get; set; }
    public int httpstatus { get; set; }
    public string status { get; set; }
    public List<object> errors { get; set; }
}
