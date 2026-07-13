// Client/Models/UtilisateursViewModel.cs
using InterManagement.Application.Features.Trainees.DTOs;
using InterManagement.Application.Features.Mentors.DTOs;
using InterManagement.Application.Features.Admins.DTOs;

namespace InterManagement.Client.Models
{
    public class UtilisateursViewModel
    {
        public List<TraineeDto> Stagiaires { get; set; } = [];
        public List<MentorDto> Mentors { get; set; } = [];
        public List<AdminDto> Admins { get; set; } = [];
    }
}
