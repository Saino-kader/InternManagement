// Controllers/BaseController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InterManagement.Client.Controllers;

public abstract class BaseController : Controller
{
    // ── Messages flash ───────────────────────────────────────
    protected void SetSuccess(string message)
        => TempData["SuccessMessage"] = message;

    protected void SetError(string message)
        => TempData["ErrorMessage"] = message;

    // ── Lecture de la session ────────────────────────────────
    protected string? CurrentUserRole
        => HttpContext.Session.GetString("UserRole");

    protected string? CurrentUserEmail
        => HttpContext.Session.GetString("UserEmail");

    protected int CurrentEntityId
        => HttpContext.Session.GetInt32("EntityId") ?? 0;

    protected bool IsAuthenticated
        => !string.IsNullOrWhiteSpace(CurrentUserRole);

    // ── Vérification rôle ────────────────────────────────────
    // Appelée au début de chaque action pour restreindre l'accès
    protected IActionResult? RequireRole(params string[] allowedRoles)
    {
        // 1. Pas connecté → page de connexion
        if (!IsAuthenticated)
        {
            var returnUrl = HttpContext.Request.Path
                          + HttpContext.Request.QueryString;
            return RedirectToAction("Login", "Account",
                new { returnUrl });
        }

        // 2. Connecté mais mauvais rôle → page AccessDenied
        if (!allowedRoles.Contains(CurrentUserRole))
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        // 3. Tout est bon → null = on continue normalement
        return null;
    }

    // ── Filtre global : vérifie juste la connexion ──────────
    // (le rôle est vérifié par RequireRole dans chaque action)
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!IsAuthenticated)
        {
            var returnUrl = HttpContext.Request.Path
                          + HttpContext.Request.QueryString;
            context.Result = RedirectToAction("Login", "Account",
                new { returnUrl });
            return;
        }

        ViewBag.CurrentUserEmail = CurrentUserEmail;
        ViewBag.CurrentUserRole  = CurrentUserRole;
        base.OnActionExecuting(context);
    }
}





