// InterManagement.Server/Controllers/AuthController.cs
//
// 2 endpoints :
// POST /api/auth/login        → vérifie identifiants, retourne rôle + entityId
// POST /api/auth/create-account → crée le compte Identity, retourne le mot de passe temporaire

using InterManagement.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // POST api/auth/login
        // Appelé par AccountController (Client) quand l'utilisateur soumet le formulaire
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { error = "Email et mot de passe obligatoires" });
            }

            var result = await _authService.LoginAsync(request.Email, request.Password);

            if (!result.Success)
                return Unauthorized(new { error = result.Error });

            return Ok(new
            {
                email = result.Email,
                role = result.Role,
                entityId = result.EntityId
            });
        }

        // POST api/auth/create-account
        // Appelé automatiquement quand l'Admin crée un Trainee/Mentor/Admin
        // Retourne le mot de passe temporaire affiché UNE SEULE FOIS
        [HttpPost("create-account")]
        public async Task<IActionResult> CreateAccount(
            [FromBody] CreateAccountRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Role))
            {
                return BadRequest(new { error = "Email et rôle obligatoires" });
            }

            var result = await _authService.CreateAccountAsync(
                request.Email, request.Role);

            if (!result.Success)
                return BadRequest(new { error = result.Error });

            // Le mot de passe temporaire est retourné UNE SEULE FOIS
            // Le Client l'affiche à l'Admin qui doit le noter et le transmettre
            return Ok(new
            {
                temporaryPassword = result.TemporaryPassword
            });
        }

        // POST api/auth/delete-account
        // Appelé quand l'Admin supprime un utilisateur
        [HttpPost("delete-account")]
        public async Task<IActionResult> DeleteAccount(
            [FromBody] DeleteAccountRequest request)
        {
            await _authService.DeleteAccountAsync(request.Email);
            return Ok();
        }
    }

    // ── DTOs des requêtes ────────────────────────────────────

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class CreateAccountRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class DeleteAccountRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
