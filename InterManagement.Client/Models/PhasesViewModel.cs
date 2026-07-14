// Client/Models/PhasesViewModel.cs
using InterManagement.Application.Features.Trainees.DTOs;
using InterManagement.Application.Features.Mentors.DTOs;
using InterManagement.Application.Features.Phases.DTOs;

namespace InterManagement.Client.Models
{
    public class PhasesViewModel
    {
        // Les phases brutes (toutes confondues, tous stagiaires)
        public List<PhaseDto> Phases { get; set; } = [];

        // Les 4 blocs d'accordéon (Phase 1 à 4), chacun avec ses
        // semaines distinctes — préparés par PhaseController
        public List<PhaseAccordionItem> AccordionItems { get; set; } = [];

        // Nécessaires pour remplir le multi-select et le <select> Mentor du modal
        public List<TraineeDto> Stagiaires { get; set; } = [];
        public List<MentorDto> Mentors { get; set; } = [];
    }
}
