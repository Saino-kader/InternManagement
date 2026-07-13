// Client/Models/MentorViewModel.cs
using InterManagement.Application.Features.WeeklyFollowUps.DTOs;
using InterManagement.Application.Features.Feedbacks.DTOs;
using InterManagement.Application.Features.Trainees.DTOs;

namespace InterManagement.Client.Models
{
    public class MentorViewModel
    {
        // Le mentor actuellement connecté (déterminé plus tard par
        // le système d'authentification — pour l'instant l'Id est
        // passé en paramètre depuis le Controller)
        public int MentorId { get; set; }
        public string MentorName { get; set; } = string.Empty;

        // Stagiaires assignés à ce mentor, 1 ligne claire par stagiaire
        public List<MentorAssignedTraineeItem> StagiairesAssignes { get; set; } = [];

        // Pour remplir le <select> Stagiaire du formulaire feedback
        public List<TraineeDto> Stagiaires { get; set; } = [];

        // Historique des feedbacks envoyés par ce mentor
        public List<FeedbackDto> HistoriqueFeedbacks { get; set; } = [];
    }
}
