// Client/Models/StagiaireViewModel.cs
using InterManagement.Application.Features.Phases.DTOs;
using InterManagement.Application.Features.Weeks.DTOs;

namespace InterManagement.Client.Models
{
    // Représente une phase avec ses semaines, prête à afficher
    // dans un onglet "Phase X" de l'espace Stagiaire.
    public class StagiairePhaseItem
    {
        public int PhaseId { get; set; }
        public int PhaseNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public int? MentorId { get; set; }
        public string MentorName { get; set; } = string.Empty;
        public List<WeekDto> Weeks { get; set; } = [];
    }

    public class StagiaireViewModel
    {
        // Le stagiaire actuellement connecté (déterminé plus tard
        // par le système d'authentification — pour l'instant l'Id
        // est passé en paramètre depuis le Controller)
        public int TraineeId { get; set; }
        public string TraineeName { get; set; } = string.Empty;

        // Les 4 phases du parcours de ce stagiaire, avec leurs semaines
        public List<StagiairePhaseItem> Phases { get; set; } = [];
    }
}
