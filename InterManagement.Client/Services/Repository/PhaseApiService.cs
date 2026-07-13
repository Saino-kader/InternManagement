// Services/Repository/PhaseApiService.cs
using System.Net.Http.Json;
using InterManagement.Application.Features.Phases.Commands.CreatePhaseForMultipleTrainees;
using InterManagement.Application.Features.Phases.DTOs;

namespace InterManagement.Client.Services;

public class PhaseApiService
    : BaseApiService<PhaseDto, CreatePhaseDto, UpdatePhaseDto>,
      IPhaseApiService
{
    protected override string ResourcePath => "phase";

    public PhaseApiService(HttpClient httpClient, ILogger<PhaseApiService> logger)
        : base(httpClient, logger) { }

    public async Task<List<PhaseDto>> GetByTraineeAsync(int traineeId)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<PhaseDto>>(
                $"{ResourcePath}?traineeId={traineeId}", JsonOptions);
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching phases for trainee {TraineeId}", traineeId);
            return [];
        }
    }

    public async Task<PhaseDetailDto?> GetDetailsAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PhaseDetailDto>(
                $"{ResourcePath}/{id}", JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching phase details for id {Id}", id);
            return null;
        }
    }

    public async Task<List<int>?> CreateForMultipleTraineesAsync(
        CreatePhaseForMultipleTraineesDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{ResourcePath}/multi-create", dto, JsonOptions);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<int>>(JsonOptions);

            _logger.LogWarning("CreateForMultipleTrainees failed with {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating phase for multiple trainees");
            return null;
        }
    }
}



/*
// ──────────────────────────────────────────────────────────────────────────────
// 1. IMPORTS NÉCESSAIRES
// ──────────────────────────────────────────────────────────────────────────────

using System.Net.Http.Json;
// ↑ Fournit les méthodes d'extension pour JSON :
//    GetFromJsonAsync, PostAsJsonAsync, PutAsJsonAsync
//    Permet de sérialiser/désérialiser automatiquement

using InterManagement.Application.Features.Phases.Commands.CreatePhaseForMultipleTrainees;
// ↑ Importe le DTO pour la création multiple de phases
//    CreatePhaseForMultipleTraineesDto = contient la liste des stagiaires

using InterManagement.Application.Features.Phases.DTOs;
// ↑ Importe les DTOs des phases :
//    PhaseDto         = version simplifiée (pour les listes)
//    PhaseDetailDto   = version complète (avec semaines, assignments)
//    CreatePhaseDto   = pour la création
//    UpdatePhaseDto   = pour la mise à jour

// ──────────────────────────────────────────────────────────────────────────────
// 2. NAMESPACE
// ──────────────────────────────────────────────────────────────────────────────

namespace InterManagement.Client.Services;

// ──────────────────────────────────────────────────────────────────────────────
// 3. CLASSE SERVICE
// ──────────────────────────────────────────────────────────────────────────────

public class PhaseApiService
    : BaseApiService<PhaseDto, CreatePhaseDto, UpdatePhaseDto>,
    // ↑ Hérite des méthodes CRUD de base :
    //   - GetAllAsync()   → GET  /api/phase
    //   - GetByIdAsync()  → GET  /api/phase/{id}
    //   - CreateAsync()   → POST /api/phase
    //   - UpdateAsync()   → PUT  /api/phase/{id}
    //   - DeleteAsync()   → DELETE /api/phase/{id}

      IPhaseApiService
    // ↑ Implémente l'interface pour les méthodes spécifiques

{
    // ──────────────────────────────────────────────────────────────────────────
    // 4. CHEMIN DE BASE DE L'API
    // ──────────────────────────────────────────────────────────────────────────

    protected override string ResourcePath => "phase";
    // ↑ Définit l'URL de base : "phase" → /api/phase
    // ↑ override = remplace la propriété abstraite de BaseApiService

    // ──────────────────────────────────────────────────────────────────────────
    // 5. CONSTRUCTEUR
    // ──────────────────────────────────────────────────────────────────────────

    public PhaseApiService(HttpClient httpClient, ILogger<PhaseApiService> logger)
        : base(httpClient, logger)
        // ↑ Passe HttpClient et Logger à la classe de base
        // ↑ ILogger<PhaseApiService> = logger typé pour ce service

    {
        // Le constructeur ne fait rien d'autre car tout est dans la classe de base
    }

    // ──────────────────────────────────────────────────────────────────────────
    // 6. MÉTHODE : RÉCUPÉRER LES PHASES D'UN STAGIAIRE
    // ──────────────────────────────────────────────────────────────────────────

    public async Task<List<PhaseDto>> GetByTraineeAsync(int traineeId)
    // ↑ async = exécution asynchrone
    // ↑ Task<List<PhaseDto>> = retourne une liste de phases (version simplifiée)
    // ↑ traineeId = ID du stagiaire dont on veut les phases

    {
        try
        // ↑ try/catch = capture les erreurs pour ne pas faire planter l'appli

        {
            var result = await _httpClient.GetFromJsonAsync<List<PhaseDto>>(
                $"{ResourcePath}?traineeId={traineeId}", JsonOptions);
            // ↑ GetFromJsonAsync = envoie une requête GET et désérialise la réponse
            // ↑ $"{ResourcePath}?traineeId={traineeId}" = "phase?traineeId=5"
            // ↑ URL complète : GET /api/phase?traineeId=5
            // ↑ JsonOptions = configuration JSON (camelCase, insensitive)
            // ↑ List<PhaseDto> = type attendu en retour

            return result ?? [];
            // ↑ ?? = null-coalescing operator
            // ↑ Si result est null, retourne une liste vide []
            //    Évite les NullReferenceException
        }
        catch (Exception ex)
        // ↑ Capture TOUTES les exceptions (réseau, sérialisation, etc.)

        {
            _logger.LogError(ex, "Error fetching phases for trainee {TraineeId}", traineeId);
            // ↑ LogError = journalise l'erreur
            // ↑ {TraineeId} = placeholder pour l'ID du stagiaire
            //    Dans les logs : "Error fetching phases for trainee 5"

            return [];
            // ↑ Retourne une liste vide plutôt que de planter
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // 7. MÉTHODE : RÉCUPÉRER LES DÉTAILS D'UNE PHASE
    // ──────────────────────────────────────────────────────────────────────────

    public async Task<PhaseDetailDto?> GetDetailsAsync(int id)
    // ↑ PhaseDetailDto? = retourne PhaseDetailDto ou null
    // ↑ id = ID de la phase à récupérer

    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PhaseDetailDto>(
                $"{ResourcePath}/{id}", JsonOptions);
            // ↑ URL complète : GET /api/phase/{id}
            // ↑ PhaseDetailDto = version complète incluant :
            //    - Semaines (Weeks)
            //    - Assignments (stagiaires assignés)
            //    - Trainee (informations du stagiaire)
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching phase details for id {Id}", id);
            return null;
            // ↑ Retourne null si la phase n'existe pas ou en cas d'erreur
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // 8. MÉTHODE : CRÉER DES PHASES POUR PLUSIEURS STAGIAIRES
    // ──────────────────────────────────────────────────────────────────────────

    public async Task<List<int>?> CreateForMultipleTraineesAsync(
        CreatePhaseForMultipleTraineesDto dto)
    // ↑ List<int>? = retourne la liste des IDs créés ou null
    // ↑ dto = contient les données de la phase + la liste des stagiaires

    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{ResourcePath}/multi-create", dto, JsonOptions);
            // ↑ PostAsJsonAsync = envoie une requête POST avec le DTO en JSON
            // ↑ URL : POST /api/phase/multi-create
            // ↑ dto = sérialisé automatiquement en JSON dans le corps
            // ↑ Exemple de corps : { "phaseNumber": 1, "traineeIds": [5, 7, 9], ... }

            if (response.IsSuccessStatusCode)
            // ↑ IsSuccessStatusCode = vérifie si le statut HTTP est 2xx
            //    Exemple : 200 OK, 201 Created, 204 No Content

                return await response.Content.ReadFromJsonAsync<List<int>>(JsonOptions);
            // ↑ ReadFromJsonAsync = lit le corps de la réponse et le convertit
            // ↑ Retourne la liste des IDs des phases créées

            _logger.LogWarning("CreateForMultipleTrainees failed with {StatusCode}", response.StatusCode);
            // ↑ LogWarning = avertissement (pas une erreur critique)
            // ↑ Journalise le code de statut HTTP

            return null;
            // ↑ Retourne null si la création échoue

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating phase for multiple trainees");
            // ↑ Journalise l'erreur sans paramètre spécifique

            return null;
        }
    }
}





*/