// InterManagement.Server/Services/AuthService.cs
//
// Ce service fait le lien entre :
// - Tes entités métier (Trainee, Mentor, Admin dans la table users)
// - Les comptes Identity (AspNetUsers, AspNetRoles)
//
// Quand l'Admin crée un Trainee → AuthService crée aussi
// le compte Identity correspondant avec un mot de passe
// temporaire affiché UNE SEULE FOIS à l'Admin.

using InternManagement.Infrastructure.Data;
using InterManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace InterManagement.Server.Services
{
    public class AuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public AuthService(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // ── Initialise les 3 rôles au démarrage de l'application ──
        // Appelée depuis Program.cs une seule fois
        public async Task EnsureRolesExistAsync()
        {
            string[] roles = ["Admin", "Trainee", "Mentor"];

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // ── Crée un compte Identity pour un utilisateur existant ──
        // Retourne le mot de passe temporaire généré (affiché UNE SEULE FOIS)
        public async Task<CreateAccountResult> CreateAccountAsync(
            string email, string role)
        {
            // Vérifie si un compte Identity existe déjà pour cet email
            var existing = await _userManager.FindByEmailAsync(email);
            if (existing != null)
            {
                return new CreateAccountResult
                {
                    Success = false,
                    Error = $"Un compte existe déjà pour {email}"
                };
            }

            // Génère un mot de passe temporaire sécurisé
            var tempPassword = GenerateTemporaryPassword();

            var identityUser = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            // Crée le compte avec le mot de passe hashé automatiquement
            var result = await _userManager.CreateAsync(identityUser, tempPassword);

            if (!result.Succeeded)
            {
                return new CreateAccountResult
                {
                    Success = false,
                    Error = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            // Assigne le rôle (Admin / Trainee / Mentor)
            await _userManager.AddToRoleAsync(identityUser, role);

            return new CreateAccountResult
            {
                Success = true,
                // Mot de passe retourné UNE SEULE FOIS — jamais stocké en clair
                TemporaryPassword = tempPassword
            };
        }

        // ── Supprime le compte Identity quand on supprime un utilisateur ──
        public async Task DeleteAccountAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
                await _userManager.DeleteAsync(user);
        }

        // ── Vérifie les identifiants de connexion ──
        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            var identityUser = await _userManager.FindByEmailAsync(email);
            if (identityUser == null)
            {
                return new LoginResult
                {
                    Success = false,
                    Error = "Email ou mot de passe incorrect"
                };
            }

            var passwordValid = await _userManager.CheckPasswordAsync(
                identityUser, password);

            if (!passwordValid)
            {
                return new LoginResult
                {
                    Success = false,
                    Error = "Email ou mot de passe incorrect"
                };
            }

            // Récupère le rôle de cet utilisateur
            var roles = await _userManager.GetRolesAsync(identityUser);
            var role = roles.FirstOrDefault() ?? "Trainee";

            // Trouve l'entité métier correspondante (pour récupérer l'Id)
            var entityId = await FindEntityIdByEmailAsync(email, role);

            return new LoginResult
            {
                Success = true,
                Email = email,
                Role = role,
                EntityId = entityId
            };
        }

        // ── Trouve l'Id de l'entité métier à partir de l'email et du rôle ──
        // Nécessaire pour rediriger vers /Stagiaire?traineeId=X
        private async Task<int> FindEntityIdByEmailAsync(string email, string role)
        {
            return role switch
            {
                "Trainee" => await _context.Trainees
                    .Where(t => t.Email == email)
                    .Select(t => t.Id)
                    .FirstOrDefaultAsync(),

                "Mentor" => await _context.Mentors
                    .Where(m => m.Email == email)
                    .Select(m => m.Id)
                    .FirstOrDefaultAsync(),

                "Admin" => await _context.Admins
                    .Where(a => a.Email == email)
                    .Select(a => a.Id)
                    .FirstOrDefaultAsync(),

                _ => 0
            };
        }

        // ── Génère un mot de passe temporaire sécurisé ──
        // Format : Lettre majuscule + minuscules + chiffres + symbole
        // Exemple : "Kx7#mP9v"
        // Utilise RandomNumberGenerator (cryptographiquement sûr) plutôt
        // que System.Random, qui est prévisible et ne doit jamais servir
        // à générer un secret (mot de passe, jeton...).
        private static string GenerateTemporaryPassword()
        {
            const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string lower = "abcdefghjkmnpqrstuvwxyz";
            const string digits = "23456789";
            const string special = "#@!$";

            var password = new[]
            {
                upper[RandomNumberGenerator.GetInt32(upper.Length)].ToString(),
                lower[RandomNumberGenerator.GetInt32(lower.Length)].ToString(),
                lower[RandomNumberGenerator.GetInt32(lower.Length)].ToString(),
                digits[RandomNumberGenerator.GetInt32(digits.Length)].ToString(),
                special[RandomNumberGenerator.GetInt32(special.Length)].ToString(),
                upper[RandomNumberGenerator.GetInt32(upper.Length)].ToString(),
                lower[RandomNumberGenerator.GetInt32(lower.Length)].ToString(),
                digits[RandomNumberGenerator.GetInt32(digits.Length)].ToString()
            };

            // Mélange les caractères pour éviter un pattern prévisible
            return string.Concat(password.OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue)));
        }
    }

    // ── Résultats retournés par AuthService ──────────────────

    public class CreateAccountResult
    {
        public bool Success { get; set; }
        public string? TemporaryPassword { get; set; }
        public string? Error { get; set; }
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public int EntityId { get; set; }
        public string? Error { get; set; }
    }
}
