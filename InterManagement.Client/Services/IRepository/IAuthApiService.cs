// InterManagement.Client/Services/IRepository/IAuthApiService.cs

namespace InterManagement.Client.Services;

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int EntityId { get; set; }
}

public class CreateAccountResponseDto
{
    public string TemporaryPassword { get; set; } = string.Empty;
}

public interface IAuthApiService
{
    // Vérifie les identifiants → retourne rôle + entityId
    Task<LoginResponseDto?> LoginAsync(LoginDto dto);

    // Crée le compte Identity → retourne le mot de passe temporaire
    Task<string?> CreateAccountAsync(string email, string role);

    // Supprime le compte Identity
    Task DeleteAccountAsync(string email);
}
