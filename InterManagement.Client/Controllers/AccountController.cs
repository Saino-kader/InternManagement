// InterManagement.Client/Controllers/AccountController.cs
//
// Gère la connexion et déconnexion.
// Stocke les infos de l'utilisateur connecté dans la SESSION
// (rôle, entityId, email) pour savoir où rediriger.

using InterManagement.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Client.Controllers;

public class AccountController : Controller
{
    private readonly IAuthApiService _authService;

        // Ajouter dans AccountController.cs
    public IActionResult AccessDenied()
    {
        return View();
    }

    public AccountController(IAuthApiService authService)
    {
        _authService = authService;
    }

    // ── Page de connexion (GET) ──────────────────────────────
    // Affichée si quelqu'un essaie d'accéder à une page protégée
    // sans être connecté
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        // Si déjà connecté → redirige vers la bonne page
        if (HttpContext.Session.GetString("UserRole") != null)
            return RedirectToDashboard();

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    // ── Traitement du formulaire de connexion (POST) ─────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto model, string? returnUrl = null)
    {
        var emailMissing = string.IsNullOrWhiteSpace(model.Email);
        var passwordMissing = string.IsNullOrWhiteSpace(model.Password);

        if (emailMissing || passwordMissing)
        {
            ViewBag.Error = "Email et mot de passe sont obligatoires.";
            ViewBag.EmailFieldError = emailMissing ? "L'email est obligatoire." : null;
            ViewBag.PasswordFieldError = passwordMissing ? "Le mot de passe est obligatoire." : null;
            return View(model);
        }

        // Appel au Server → vérifie les identifiants
        var result = await _authService.LoginAsync(model);

        if (result == null)
        {
            ViewBag.Error = "Email ou mot de passe incorrect.";
            ViewBag.EmailFieldHighlight = true;
            ViewBag.PasswordFieldHighlight = true;
            return View(model);
        }

        // ── Stocke les infos dans la SESSION ────────────────
        // La session dure jusqu'à la fermeture du navigateur
        // (ou jusqu'au Logout)
        HttpContext.Session.SetString("UserEmail", result.Email);
        HttpContext.Session.SetString("UserRole", result.Role);
        HttpContext.Session.SetInt32("EntityId", result.EntityId);

        // ── Redirige vers la bonne page selon le rôle ───────
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToDashboard(result.Role, result.EntityId);
    }

    // ── Déconnexion ──────────────────────────────────────────
    public IActionResult Logout()
    {
        // Supprime toutes les données de session
        HttpContext.Session.Clear();

        // Redirige vers la page d'accueil publique
        return RedirectToAction("Index", "Home");
    }

    // ── Redirige selon le rôle ───────────────────────────────
    private IActionResult RedirectToDashboard(
        string? role = null,
        int entityId = 0)
    {
        role ??= HttpContext.Session.GetString("UserRole");
        entityId = entityId == 0
            ? HttpContext.Session.GetInt32("EntityId") ?? 0
            : entityId;

        return role switch
        {
            "Admin"   => RedirectToAction("Index", "Dashboard"),
            "Trainee" => RedirectToAction("Index", "Stagiaire",
                            new { traineeId = entityId }),
            "Mentor"  => RedirectToAction("Index", "MentorSpace",
                            new { mentorId = entityId }),
            _         => RedirectToAction("Index", "Home")
        };
    }
}
