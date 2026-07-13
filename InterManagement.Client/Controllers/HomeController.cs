// InterManagement.Client/Controllers/HomeController.cs
//
// IMPORTANT : hérite de Controller (pas BaseController)
// → la page d'accueil est PUBLIQUE, accessible sans connexion
// → BaseController redirige vers Login si non connecté,
//   ce qu'on ne veut pas pour la page d'accueil

using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Client.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
