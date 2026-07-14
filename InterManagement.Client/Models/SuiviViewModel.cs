// Client/Models/SuiviViewModel.cs
using InterManagement.Application.Features.WeeklyFollowUps.DTOs;
using InterManagement.Application.Features.Trainees.DTOs;
using InterManagement.Application.Features.Mentors.DTOs;
using InterManagement.Application.Features.Weeks.DTOs;
using InterManagement.Application.Features.ImportedFollowUps.DTOs;

namespace InterManagement.Client.Models
{
    public class SuiviViewModel
    {
        public List<WeeklyFollowUpDto> Suivis { get; set; } = [];

        // Nécessaires pour remplir les <select> Stagiaire/Mentor/Week du modal
        public List<TraineeDto> Stagiaires { get; set; } = [];
        public List<MentorDto> Mentors { get; set; } = [];
        public List<WeekDto> Weeks { get; set; } = [];

        // Tableau 2 — Suivis importés depuis Excel
        public List<ImportedFollowUpDto> SuivisImportes { get; set; } = [];
    }
}
