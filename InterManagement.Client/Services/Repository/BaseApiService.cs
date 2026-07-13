/*

// ──────────────────────────────────────────────────────────────────────────────
// 1. IMPORTS NÉCESSAIRES
// ──────────────────────────────────────────────────────────────────────────────

using System.Net.Http.Json;   // ← Fournit les méthodes d'extension pour JSON :
                               //    GetFromJsonAsync, PostAsJsonAsync, PutAsJsonAsync
                               //    Permet de sérialiser/désérialiser automatiquement

using System.Text.Json;       // ← Fournit JsonSerializerOptions pour configurer
                               //    la sérialisation JSON

// ──────────────────────────────────────────────────────────────────────────────
// 2. NAMESPACE
// ──────────────────────────────────────────────────────────────────────────────

namespace InterManagement.Client.Services;

// ──────────────────────────────────────────────────────────────────────────────
// 3. CLASSE ABSTRAITE GÉNÉRIQUE
// ──────────────────────────────────────────────────────────────────────────────

public abstract class BaseApiService<TDto, TCreateDto, TUpdateDto>
    // ↑ abstract = ne peut pas être instanciée directement, doit être héritée
    // ↑ TDto      = type de l'objet retourné (ex: WeeklyFollowUpDto)
    // ↑ TCreateDto = type de l'objet envoyé pour la création
    // ↑ TUpdateDto = type de l'objet envoyé pour la mise à jour

    : IBaseApiService<TDto, TCreateDto, TUpdateDto>
    // ↑ Implémente l'interface pour les méthodes CRUD

{
    // ──────────────────────────────────────────────────────────────────────────
    // 4. CHAMPS PROTÉGÉS (accessibles dans les classes filles)
    // ──────────────────────────────────────────────────────────────────────────

    protected readonly HttpClient _httpClient;
    // ↑ readonly = ne peut être modifié après le constructeur
    // ↑ HttpClient = client HTTP qui envoie les requêtes à l'API
    // ↑ protected = accessible dans les classes filles (ex: WeeklyFollowUpApiService)

    protected readonly ILogger _logger;
    // ↑ ILogger = pour journaliser les erreurs et les warnings
    // ↑ Permet de tracer ce qui se passe (debugging)

    protected abstract string ResourcePath { get; }
    // ↑ abstract = doit être implémenté par la classe fille
    // ↑ Définit le chemin de l'API (ex: "weeklyfollowup")

    // ──────────────────────────────────────────────────────────────────────────
    // 5. CONFIGURATION JSON (STATIQUE)
    // ──────────────────────────────────────────────────────────────────────────

    protected static readonly JsonSerializerOptions JsonOptions = new()
    // ↑ static = partagé par toutes les instances de la classe
    // ↑ readonly = ne peut être modifié après initialisation

    {
        PropertyNameCaseInsensitive = true,
        // ↑ Ignore la casse : "traineeId" correspond à "TraineeId"
        //    Permet d'accepter à la fois camelCase et PascalCase

        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        // ↑ Convertit les noms en camelCase : "TraineeName" → "traineeName"
        //    Respecte la convention JSON standard
    };

    // ──────────────────────────────────────────────────────────────────────────
    // 6. CONSTRUCTEUR
    // ──────────────────────────────────────────────────────────────────────────

    protected BaseApiService(HttpClient httpClient, ILogger logger)
    // ↑ protected = appelé uniquement par les classes filles
    // ↑ Reçoit HttpClient et Logger depuis l'injection de dépendances

    {
        _httpClient = httpClient;   // ← Stocke le client HTTP
        _logger = logger;           // ← Stocke le logger
    }

    // ──────────────────────────────────────────────────────────────────────────
    // 7. MÉTHODE : GET ALL
    // ──────────────────────────────────────────────────────────────────────────

    public async Task<List<TDto>> GetAllAsync()
    // ↑ async = exécution asynchrone (ne bloque pas le thread)
    // ↑ Task<List<TDto>> = retourne une liste d'objets de type TDto

    {
        try
        // ↑ try/catch = capture toutes les erreurs pour ne pas faire planter l'appli

        {
            var result = await _httpClient.GetFromJsonAsync<List<TDto>>(ResourcePath, JsonOptions);
            // ↑ await = attend la réponse de l'API
            // ↑ GetFromJsonAsync = envoie une requête GET et convertit la réponse en objet
            // ↑ List<TDto> = le type attendu en retour
            // ↑ ResourcePath = "weeklyfollowup"
            // ↑ JsonOptions = configuration pour la désérialisation

            return result ?? [];
            // ↑ ?? = null-coalescing operator
            // ↑ Si result est null, retourne une liste vide []
            //    Évite de retourner null et les NullReferenceException

        }
        catch (Exception ex)
        // ↑ Capture TOUTES les exceptions (réseau, sérialisation, etc.)

        {
            _logger.LogError(ex, "Error fetching all {Resource}", typeof(TDto).Name);
            // ↑ LogError = journalise l'erreur
            // ↑ {Resource} = placeholders pour le nom du type (ex: "WeeklyFollowUpDto")

            return [];
            // ↑ Retourne une liste vide plutôt que de planter
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // 8. MÉTHODE : GET BY ID
    // ──────────────────────────────────────────────────────────────────────────

    public async Task<TDto?> GetByIdAsync(int id)
    // ↑ TDto? = retourne TDto ou null
    // ↑ id = identifiant de l'objet

    {
        try
        {
            return await _httpClient.GetFromJsonAsync<TDto>($"{ResourcePath}/{id}", JsonOptions);
            // ↑ Envoie GET /api/weeklyfollowup/5
            // ↑ $"{ResourcePath}/{id}" = interpolation de chaîne
            //    Exemple : "weeklyfollowup" + "/" + 5 = "weeklyfollowup/5"
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching {Resource} with id {Id}", typeof(TDto).Name, id);
            return default;
            // ↑ default = retourne null pour les types référence
            //    Équivalent à return null;
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // 9. MÉTHODE : CREATE
    // ──────────────────────────────────────────────────────────────────────────

    public async Task<TDto?> CreateAsync(TCreateDto dto)
    // ↑ TCreateDto = type envoyé pour la création (ex: CreateWeeklyFollowUpDto)

    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(ResourcePath, dto, JsonOptions);
            // ↑ PostAsJsonAsync = envoie une requête POST avec le DTO en JSON
            // ↑ ResourcePath = "weeklyfollowup"
            // ↑ dto = l'objet à envoyer (sérialisé automatiquement en JSON)

            if (response.IsSuccessStatusCode)
            // ↑ IsSuccessStatusCode = vérifie si le statut HTTP est 2xx (200-299)
            //    Exemple: 200 (OK), 201 (Created), 204 (No Content)

                return await response.Content.ReadFromJsonAsync<TDto>(JsonOptions);
            // ↑ ReadFromJsonAsync = lit le corps de la réponse et le convertit en TDto
            //    Retourne l'objet créé avec son ID généré

            _logger.LogWarning("Create {Resource} failed with {StatusCode}", typeof(TDto).Name, response.StatusCode);
            // ↑ LogWarning = avertissement (pas une erreur critique)

            return default;
            // ↑ Retourne null si la création échoue

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating {Resource}", typeof(TDto).Name);
            return default;
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // 10. MÉTHODE : UPDATE
    // ──────────────────────────────────────────────────────────────────────────

    public async Task<TDto?> UpdateAsync(int id, TUpdateDto dto)
    // ↑ TUpdateDto = type envoyé pour la mise à jour (ex: UpdateWeeklyFollowUpDto)
    // ↑ id = identifiant de l'objet à modifier

    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{ResourcePath}/{id}", dto, JsonOptions);
            // ↑ PutAsJsonAsync = envoie une requête PUT avec le DTO en JSON
            // ↑ PUT /api/weeklyfollowup/5 (modification complète)
            //    Différence avec PATCH : PUT remplace tout, PATCH modifie partiellement

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<TDto>(JsonOptions);

            _logger.LogWarning("Update {Resource}/{Id} failed with {StatusCode}", typeof(TDto).Name, id, response.StatusCode);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating {Resource} {Id}", typeof(TDto).Name, id);
            return default;
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // 11. MÉTHODE : DELETE
    // ──────────────────────────────────────────────────────────────────────────

    public async Task<bool> DeleteAsync(int id)
    // ↑ bool = retourne true si succès, false si échec

    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{ResourcePath}/{id}");
            // ↑ DeleteAsync = envoie une requête DELETE
            // ↑ DELETE /api/weeklyfollowup/5

            return response.IsSuccessStatusCode;
            // ↑ true si 2xx, false sinon
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting {Resource} {Id}", typeof(TDto).Name, id);
            return false;
        }
    }
}


📊 Tableau récapitulatif des méthodes CRUD
Méthode	HTTP	URL	Rôle	Retour
GetAllAsync()	GET	/api/{ResourcePath}	Récupère tous les éléments	List<TDto>
GetByIdAsync(id)	GET	/api/{ResourcePath}/{id}	Récupère un élément par ID	TDto?
CreateAsync(dto)	POST	/api/{ResourcePath}	Crée un nouvel élément	TDto?
UpdateAsync(id, dto)	PUT	/api/{ResourcePath}/{id}	Met à jour un élément	TDto?
DeleteAsync(id)	DELETE	/api/{ResourcePath}/{id}	Supprime un élément	bool


🔑 Termes essentiels expliqués
Terme	Explication
HttpClient	Client HTTP qui envoie des requêtes à l'API
HttpResponseMessage	Réponse du serveur (statut, contenu, en-têtes)
Async/Await	Exécution asynchrone = ne bloque pas le thread
Try/Catch	Capture les erreurs pour les gérer proprement
Task	Représente une opération asynchrone
JsonSerializerOptions	Configuration de la sérialisation JSON
PropertyNameCaseInsensitive	Ignore la casse des noms de propriétés
JsonNamingPolicy.CamelCase	Convertit en camelCase (ex: "TraineeName" → "traineeName")
Null-coalescing operator (??)	Retourne une valeur par défaut si null
ResourcePath	Chemin de l'API (ex: "weeklyfollowup")
Status Code	Code HTTP (200 = OK, 404 = Not Found, 500 = Erreur serveur)




*/



/*using System.Net.Http.Json;
using System.Text.Json;

namespace InterManagement.Client.Services;

public abstract class BaseApiService<TDto, TCreateDto, TUpdateDto>
    : IBaseApiService<TDto, TCreateDto, TUpdateDto>
{
    protected readonly HttpClient _httpClient;
    protected readonly ILogger _logger;
    protected abstract string ResourcePath { get; }

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    protected BaseApiService(HttpClient httpClient, ILogger logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<TDto>> GetAllAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<TDto>>(ResourcePath, JsonOptions);
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all {Resource}", typeof(TDto).Name);
            return [];
        }
    }

    public async Task<TDto?> GetByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<TDto>($"{ResourcePath}/{id}", JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching {Resource} with id {Id}", typeof(TDto).Name, id);
            return default;
        }
    }

    public async Task<TDto?> CreateAsync(TCreateDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(ResourcePath, dto, JsonOptions);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<TDto>(JsonOptions);

            _logger.LogWarning("Create {Resource} failed with {StatusCode}", typeof(TDto).Name, response.StatusCode);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating {Resource}", typeof(TDto).Name);
            return default;
        }
    }

    public async Task<TDto?> UpdateAsync(int id, TUpdateDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{ResourcePath}/{id}", dto, JsonOptions);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<TDto>(JsonOptions);

            _logger.LogWarning("Update {Resource}/{Id} failed with {StatusCode}", typeof(TDto).Name, id, response.StatusCode);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating {Resource} {Id}", typeof(TDto).Name, id);
            return default;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{ResourcePath}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting {Resource} {Id}", typeof(TDto).Name, id);
            return false;
        }
    }
}




*/



// ======================================================
// IMPORTATIONS (USING)
// ======================================================

// Pour convertir les objets C# en JSON et vice-versa (HttpClient)
using System.Net.Http.Json;

// Pour configurer la sérialisation JSON (camelCase, insensible à la casse)
using System.Text.Json;

// ======================================================
// NAMESPACE
// ======================================================
namespace InterManagement.Client.Services;

// ======================================================
// CLASSE ABSTRAITE DE BASE POUR APPELER L'API
// ======================================================

// public abstract → accessible partout, mais on ne peut pas créer d'instance directement
// class BaseApiService<TDto, TCreateDto, TUpdateDto> → classe générique avec 3 types
// : IBaseApiService<TDto, TCreateDto, TUpdateDto> → implémente le contrat
public abstract class BaseApiService<TDto, TCreateDto, TUpdateDto>
    : IBaseApiService<TDto, TCreateDto, TUpdateDto>
{
    // ==================================================
    // CHAMPS (VARIABLES DE CLASSE)
    // ==================================================
    
    // protected → accessible par les classes filles (TraineeApiService)
    // readonly → assigné une fois dans le constructeur, jamais modifié
    // HttpClient → l'outil qui envoie les requêtes HTTP
    protected readonly HttpClient _httpClient;
    
    // protected → accessible par les classes filles
    // ILogger → outil pour écrire des logs (dans la console)
    protected readonly ILogger _logger;
    
    // protected abstract → la classe fille DOIT définir cette propriété
    // string ResourcePath → le nom de la ressource (ex: "trainee")
    protected abstract string ResourcePath { get; }

    // ==================================================
    // CONFIGURATION JSON (STATIQUE)
    // ==================================================
    
    // protected static → partagé par TOUTES les instances de toutes les classes filles
    // JsonSerializerOptions → options pour convertir JSON ↔ C#
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        // PropertyNameCaseInsensitive = true
        // → "firstName" et "FirstName" sont traités pareil
        // → évite les erreurs de casse
        PropertyNameCaseInsensitive = true,
        
        // PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        // → C# utilise PascalCase : FirstName
        // → JSON utilise camelCase : firstName
        // → cette option fait la conversion automatique
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // ==================================================
    // CONSTRUCTEUR
    // ==================================================
    
    // protected → seule les classes filles peuvent l'appeler
    // HttpClient httpClient → le client HTTP (injecté)
    // ILogger logger → le logger (injecté)
    protected BaseApiService(HttpClient httpClient, ILogger logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    // ==================================================
    // MÉTHODE : GET ALL (RÉCUPÉRER TOUS)
    // ==================================================
    
    // public → accessible partout
    // async Task<List<TDto>> → asynchrone, retourne une liste de DTO
    public async Task<List<TDto>> GetAllAsync()
    {
        try
        {
            // Envoie GET http://serveur/api/trainee
            // Récupère le JSON, le convertit en List<TDto>
            var result = await _httpClient.GetFromJsonAsync<List<TDto>>(ResourcePath, JsonOptions);
            
            // result ?? [] → si result est null, retourne une liste vide
            return result ?? [];
        }
        catch (Exception ex)
        {
            // En cas d'erreur (API hors ligne, etc.)
            // Écrit l'erreur dans la console
            _logger.LogError(ex, "Error fetching all {Resource}", typeof(TDto).Name);
            // Retourne une liste vide (pas null, pour éviter les erreurs dans la vue)
            return [];
        }
    }

    // ==================================================
    // MÉTHODE : GET BY ID (RÉCUPÉRER UN SEUL)
    // ==================================================
    
    public async Task<TDto?> GetByIdAsync(int id)
    {
        try
        {
            // Envoie GET http://serveur/api/trainee/5
            // Récupère le JSON, le convertit en TDto
            return await _httpClient.GetFromJsonAsync<TDto>($"{ResourcePath}/{id}", JsonOptions);
            // Retourne null si l'API retourne 404
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching {Resource} with id {Id}", typeof(TDto).Name, id);
            // return default → retourne null pour les types référence
            return default;
        }
    }

    // ==================================================
    // MÉTHODE : CREATE (CRÉER)
    // ==================================================
    
    public async Task<TDto?> CreateAsync(TCreateDto dto)
    {
        try
        {
            // Envoie POST http://serveur/api/trainee
            // Le DTO est converti en JSON et envoyé dans le corps de la requête
            var response = await _httpClient.PostAsJsonAsync(ResourcePath, dto, JsonOptions);
            
            // IsSuccessStatusCode = true si le code HTTP est 2xx (200, 201, 204...)
            if (response.IsSuccessStatusCode)
                // Lit le JSON de la réponse et le convertit en TDto
                return await response.Content.ReadFromJsonAsync<TDto>(JsonOptions);

            // Si l'API retourne une erreur (400, 404, 500...)
            _logger.LogWarning("Create {Resource} failed with {StatusCode}", typeof(TDto).Name, response.StatusCode);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating {Resource}", typeof(TDto).Name);
            return default;
        }
    }

    // ==================================================
    // MÉTHODE : UPDATE (MODIFIER)
    // ==================================================
    
    public async Task<TDto?> UpdateAsync(int id, TUpdateDto dto)
    {
        try
        {
            // Envoie PUT http://serveur/api/trainee/5
            // Le DTO est converti en JSON et envoyé dans le corps
            var response = await _httpClient.PutAsJsonAsync($"{ResourcePath}/{id}", dto, JsonOptions);
            
            if (response.IsSuccessStatusCode)
            {
                // Beaucoup d'endpoints renvoient 204 NoContent après update.
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent ||
                    response.Content.Headers.ContentLength == 0)
                    return await GetByIdAsync(id);

                return await response.Content.ReadFromJsonAsync<TDto>(JsonOptions);
            }

            _logger.LogWarning("Update {Resource}/{Id} failed with {StatusCode}", typeof(TDto).Name, id, response.StatusCode);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating {Resource} {Id}", typeof(TDto).Name, id);
            return default;
        }
    }

    // ==================================================
    // MÉTHODE : DELETE (SUPPRIMER)
    // ==================================================
    
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            // Envoie DELETE http://serveur/api/trainee/5
            var response = await _httpClient.DeleteAsync($"{ResourcePath}/{id}");
            
            // Retourne true si la suppression a réussi (204 NoContent ou 200 OK)
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting {Resource} {Id}", typeof(TDto).Name, id);
            return false;
        }
    }
}

// ======================================================
// RÉSUMÉ DES MÉTHODES ET LEURS URLS
// ======================================================

// ResourcePath = "trainee"

// GetAllAsync()     → GET    http://serveur/api/trainee
// GetByIdAsync(5)   → GET    http://serveur/api/trainee/5
// CreateAsync(dto)  → POST   http://serveur/api/trainee (body = dto en JSON)
// UpdateAsync(5, dto) → PUT  http://serveur/api/trainee/5 (body = dto en JSON)
// DeleteAsync(5)    → DELETE http://serveur/api/trainee/5



