using InterManagement.Domain.Entities;

namespace InterManagement.Domain.Repositories
{
    public interface IMentorRepository : IBaseRepository<Mentor>
    {
        // Admin → liste par département
        Task<IEnumerable<Mentor>> GetByDepartmentAsync(string department);

        // Admin → vérifier email existant
        Task<bool> EmailExistsAsync(string email);

        // Admin → mentors avec nombre de stagiaires
        Task<IEnumerable<Mentor>> GetAllWithTraineeCountAsync();
        
        // Mentor → liste de ses stagiaires
        //Task<IEnumerable<Mentor>> GetAssignmentTraineesAsync();

        //Task<int> CountAssignedTraineesAsync(int mentorId);
         Task<int> CountActiveAsync();  // ← AJOUTE CETTE LIGNE SI ELLE MANQUE

    }
}




/*

// Domain/Repositories/IMentorRepository.cs
using InterManagement.Domain.Entities;

namespace InterManagement.Domain.Repositories
{
    public interface IMentorRepository : IBaseRepository<Mentor>
    {
        Task<IEnumerable<Mentor>> GetByDepartmentAsync(string department);
        Task<bool> EmailExistsAsync(string email);
        Task<IEnumerable<Mentor>> GetAllWithTraineeCountAsync();
        Task<int> CountActiveAsync();  // ← AJOUTE CETTE LIGNE SI ELLE MANQUE
    }
}

*/
// page Mentor:
    //nombre des stagiaires assignés à un mentor
    // Tableau des informations de ces stagiaires (Stagiaire 	Phase 	Semaine 	Cours 	Date début 	Date fin 	Statut)
    // Action (statut, num Phase, num semaine, commentaire et date message)

    //Admin: vérifier email existant
    //Admin: nombre mentors  actifs
    //Admin: filtre par nom des mentors

    //Après :     
    // Envoyer un feedback Mentor
    // Historique des feedbacks  Mentor


// Page Admin:
