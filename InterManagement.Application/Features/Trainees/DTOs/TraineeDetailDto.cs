using InterManagement.Application.Features.Feedbacks.DTOs;
using InterManagement.Application.Features.Phases.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Shared.Enums;


namespace InterManagement.Application.Features.Trainees.DTOs;


public class TraineeDetailDto
{

    // IDENTIFIANT
    public int Id { get; set; }
    
    // IDENTITÉ
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    // INFORMATIONS ACADÉMIQUES
    public string University { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string Theme { get; set; } = string.Empty;
    
    // DATES
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    
    // STATUT
    public TraineeStatus  Status { get; set; }

    public bool IsActive { get;  set; } = true;

            // ── Relations 
        public ICollection<PhaseDto> Phases { get; set; } = [];
        // ← commenté car PhaseDto pas encore créé
        public ICollection<FeedbackDto> Feedbacks { get; set; } = [];

















































































































    // ==================================================
    // RELATIONS (SPÉCIFIQUE AU DÉTAIL)
    // ==================================================
    
    // ICollection<PhaseDto> Phases = liste des phases du stagiaire
    // Initialisée avec [] (collection vide par défaut)
    // = [] → syntaxe C# 12, équivalent à = new List<PhaseDto>()
    // 
    // Cette propriété est COMMENTÉE car :
    // 1. La classe PhaseDto n'existe pas encore
    // 2. L'entité Phase du Domain n'est pas encore complète
    // 3. Quand Phase sera prêt, on décommentera cette ligne
    //
    // public ICollection<PhaseDto> Phases { get; set; } = [];
    // ← commenté car PhaseDto pas encore créé
}

// ======================================================
// DIFFÉRENCE ENTRE TraineeDto ET TraineeDetailDto
// ======================================================

// TraineeDto (POUR LA LISTE) :
// - Utilisé pour GET /api/trainees (liste de tous les stagiaires)
// - Contient UNIQUEMENT les infos de base
// - NE contient PAS les relations (Phases, Evaluations, etc.)
// - Pourquoi ? Performance : ne pas charger trop de données pour une liste

// TraineeDetailDto (POUR LE DÉTAIL) :
// - Utilisé pour GET /api/trainees/{id} (détail d'un stagiaire)
// - Contient les infos de base + LES RELATIONS
// - Contiendra Phases, Evaluations, Feedbacks, Files, etc.
// - Pourquoi ? L'utilisateur veut voir TOUT sur une page détail

// ======================================================
// EXEMPLE DE CE QUE LE CLIENT RECEVRA (QUAND PHASE SERA PRÊT)
// ======================================================

// Requête GET /api/trainees/5
// Réponse JSON (quand Phases sera décommenté) :
// {
//   "id": 5,
//   "firstName": "Moussa",
//   "lastName": "Diallo",
//   "email": "moussa@email.com",
//   "university": "ESMT",
//   "specialty": "Informatique",
//   "theme": "Développement Web",
//   "startDate": "2025-01-06",
//   "endDate": "2025-04-27",
//   "status": 1,
//   "phases": [
//     {
//       "id": 10,
//       "phaseNumber": 1,
//       "title": "Découverte",
//       "startDate": "2025-01-06",
//       "endDate": "2025-02-02"
//     },
//     {
//       "id": 11,
//       "phaseNumber": 2,
//       "title": "Développement",
//       "startDate": "2025-02-03",
//       "endDate": "2025-03-02"
//     }
//   ]
// }

// ======================================================
// POURQUOI DEUX DTOS DIFFÉRENTS (LISTE VS DÉTAIL) ?
// ======================================================

// 1. PERFORMANCE :
//    - Liste : on charge SEULEMENT les champs de base
//    - Détail : on charge AUSSI les relations (Phases, Evaluations, etc.)
//    - Si on utilisait TraineeDetailDto pour la liste, on chargerait
//      des milliers de phases inutilement

// 2. TAILLE DE LA RÉPONSE :
//    - Liste : réponse légère (quelques champs)
//    - Détail : réponse plus lourde (toutes les informations)

// 3. RESPONSABILITÉ :
//    - TraineeDto → "ce qu'on voit dans un tableau"
//    - TraineeDetailDto → "ce qu'on voit sur une page détail"

// ======================================================
// FUTURES PROPRIÉTÉS À AJOUTER (QUAND LES ENTITÉS SERONT PRÊTES)
// ======================================================

// public ICollection<EvaluationDto> Evaluations { get; set; } = [];
// public ICollection<FeedbackDto> Feedbacks { get; set; } = [];
// public ICollection<InternFileDto> Files { get; set; } = [];
// public ICollection<WeeklyFollowUpDto> WeeklyFollowUps { get; set; } = [];

// ======================================================
// NOTE SUR L'INITIALISATION = []
// ======================================================

// = [] est une syntaxe C# 12 (.NET 8+)
// Cela signifie : "crée une collection vide au moment de l'initialisation"
// C'est équivalent à :
// public ICollection<PhaseDto> Phases { get; set; } = new List<PhaseDto>();

// Avantage : évite les NullReferenceException
// Si on n'initialise pas, la propriété vaudra null et plantera si on l'utilise