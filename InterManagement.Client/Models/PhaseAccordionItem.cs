// Client/Models/PhaseAccordionItem.cs
// Un bloc d'accordéon = 1 numéro de phase
// avec N lignes (1 par stagiaire dans cette phase)

using InterManagement.Application.Features.Weeks.DTOs;

namespace InterManagement.Client.Models
{
    // Un accordéon = Phase N (ex: Phase 1)
    public class PhaseAccordionItem
    {
        public int PhaseNumber { get; set; }
        public string Title { get; set; } = string.Empty;

        // Une ligne par stagiaire dans cette phase
        public List<PhaseAccordionRowItem> Rows { get; set; } = [];
    }

    // Une ligne dans l'accordéon = un stagiaire + ses semaines
    public class PhaseAccordionRowItem
    {
        public int PhaseId { get; set; }
        public string TraineeName { get; set; } = string.Empty;
        public string MentorName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        // Semaines RÉELLES de cette phase
        // (chargées depuis GET /api/week?phaseId=X)
        public List<WeekDto> Weeks { get; set; } = [];
    }
}