// Services/Repository/BaseApiService.cs
using System.Net.Http.Json;
using System.Text.Json;

namespace InterManagement.Client.Services;

// Classe de base générique pour tous les services d'accès à l'API REST.
// Centralise les 5 opérations CRUD (GET all, GET by id, POST, PUT, DELETE)
// pour éviter de dupliquer la gestion HTTP/JSON/erreurs dans chaque service.
public abstract class BaseApiService<TDto, TCreateDto, TUpdateDto>
    : IBaseApiService<TDto, TCreateDto, TUpdateDto>
{
    protected readonly HttpClient _httpClient;
    protected readonly ILogger _logger;
    protected abstract string ResourcePath { get; }

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    protected BaseApiService(HttpClient httpClient, ILogger logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<TDto>> GetAllAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<TDto>>(ResourcePath, JsonOptions);
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all {Resource}", typeof(TDto).Name);
            return [];
        }
    }

    public async Task<TDto?> GetByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<TDto>($"{ResourcePath}/{id}", JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching {Resource} with id {Id}", typeof(TDto).Name, id);
            return default;
        }
    }

    public async Task<TDto?> CreateAsync(TCreateDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(ResourcePath, dto, JsonOptions);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<TDto>(JsonOptions);

            _logger.LogWarning("Create {Resource} failed with {StatusCode}", typeof(TDto).Name, response.StatusCode);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating {Resource}", typeof(TDto).Name);
            return default;
        }
    }

    public async Task<TDto?> UpdateAsync(int id, TUpdateDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{ResourcePath}/{id}", dto, JsonOptions);

            if (response.IsSuccessStatusCode)
            {
                // Beaucoup d'endpoints renvoient 204 NoContent après update :
                // dans ce cas on recharge l'entité plutôt que de lire un corps vide.
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent ||
                    response.Content.Headers.ContentLength == 0)
                    return await GetByIdAsync(id);

                return await response.Content.ReadFromJsonAsync<TDto>(JsonOptions);
            }

            _logger.LogWarning("Update {Resource}/{Id} failed with {StatusCode}", typeof(TDto).Name, id, response.StatusCode);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating {Resource} {Id}", typeof(TDto).Name, id);
            return default;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{ResourcePath}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting {Resource} {Id}", typeof(TDto).Name, id);
            return false;
        }
    }
}
