// InterManagement.Client/Services/Repository/AuthApiService.cs

using System.Net.Http.Json;
using System.Text.Json;

namespace InterManagement.Client.Services;

public class AuthApiService : IAuthApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthApiService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public AuthApiService(HttpClient httpClient, ILogger<AuthApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login", dto, JsonOptions);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<LoginResponseDto>(JsonOptions);

            _logger.LogWarning("Login failed with {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return null;
        }
    }

    public async Task<string?> CreateAccountAsync(string email, string role)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "auth/create-account",
                new { email, role },
                JsonOptions);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content
                    .ReadFromJsonAsync<CreateAccountResponseDto>(JsonOptions);
                return result?.TemporaryPassword;
            }

            _logger.LogWarning("CreateAccount failed for {Email}", email);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account for {Email}", email);
            return null;
        }
    }

    public async Task DeleteAccountAsync(string email)
    {
        try
        {
            await _httpClient.PostAsJsonAsync(
                "auth/delete-account",
                new { email },
                JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account for {Email}", email);
        }
    }
}
