//**`CreateTraineeDto`** : utilisé pour la création. Le client envoie les données sans l'Id (c'est la base qui le génère) et sans le Status (valeur par défaut InProgress).



namespace InterManagement.Application.Features.Trainees.DTOs
{
    public class CreateTraineeDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public string Theme { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}


































