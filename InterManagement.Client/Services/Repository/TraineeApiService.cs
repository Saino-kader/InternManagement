/*
using System.Net.Http.Json;
using InterManagement.Application.Features.Trainees.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Shared.Enums;

namespace InterManagement.Client.Services;

public class TraineeApiService
    : BaseApiService<TraineeDto, CreateTraineeDto, UpdateTraineeDto>,
      ITraineeApiService
{
    protected override string ResourcePath => "trainee";

    public TraineeApiService(HttpClient httpClient, ILogger<TraineeApiService> logger)
        : base(httpClient, logger) { }

    public async Task<List<TraineeDto>> GetActiveAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<TraineeDto>>(
                $"{ResourcePath}/active", JsonOptions);
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching active trainees");
            return [];
        }
    }

    public async Task<List<TraineeDto>> GetByStatusAsync(TraineeStatus status)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<TraineeDto>>(
                $"{ResourcePath}?status={(int)status}", JsonOptions);
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching trainees by status {Status}", status);
            return [];
        }
    }

    public async Task<TraineeDetailDto?> GetDetailsAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<TraineeDetailDto>(
                $"{ResourcePath}/{id}/details", JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching trainee details for id {Id}", id);
            return null;
        }
    }
}

*/







































































// ======================================================
// IMPORTATIONS (USING)
// ======================================================

// Pour les méthodes d'extension HttpClient (GetFromJsonAsync, PostAsJsonAsync, etc.)
using System.Net.Http.Json;

// Pour les DTOs (TraineeDto, CreateTraineeDto, UpdateTraineeDto, TraineeDetailDto)
using InterManagement.Application.Features.Trainees.DTOs;

// Pour l'enum TraineeStatus (InProgress, Completed, Suspended)
using InterManagement.Domain.Entities;
using InterManagement.Shared.Enums;

// ======================================================
// NAMESPACE
// ======================================================
namespace InterManagement.Client.Services;

// ======================================================
// CLASSE CONCRÈTE POUR APPELER L'API TRAINEE
// ======================================================

// public → accessible partout
// class TraineeApiService → nom de la classe
// : BaseApiService<TraineeDto, CreateTraineeDto, UpdateTraineeDto>
//   → HÉRITE de BaseApiService avec les types déjà précisés
//   → Donc elle a déjà les 5 méthodes CRUD (GetAllAsync, GetByIdAsync, etc.)
// , ITraineeApiService → implémente aussi l'interface spécifique
//   → Donc elle DOIT ajouter GetActiveAsync, GetByStatusAsync, GetDetailsAsync
public class TraineeApiService
    : BaseApiService<TraineeDto, CreateTraineeDto, UpdateTraineeDto>,
      ITraineeApiService
{
    // ==================================================
    // PROPRIÉTÉ ResourcePath (OBLIGATOIRE pour BaseApiService)
    // ==================================================
    
    // protected override → on remplace la propriété abstraite de BaseApiService
    // string ResourcePath → le nom de la ressource dans l'URL
    // => "trainee" → expression body (raccourci pour { get { return "trainee"; } })
    // Résultat : toutes les URLs commenceront par "trainee"
    protected override string ResourcePath => "trainee";

    // ==================================================
    // CONSTRUCTEUR
    // ==================================================
    
    // public → accessible par l'injection de dépendances
    // HttpClient httpClient → le client HTTP (injecté par Program.cs)
    // ILogger<TraineeApiService> logger → le logger spécifique à cette classe
    // : base(httpClient, logger) → appelle le constructeur de BaseApiService
    public TraineeApiService(HttpClient httpClient, ILogger<TraineeApiService> logger)
        : base(httpClient, logger) { }
        
    // ==================================================
    // MÉTHODE : GET ACTIVE
    // ==================================================
    
    // public → accessible par le Controller
    // async Task<List<TraineeDto>> → asynchrone, retourne une liste de TraineeDto
    // GetActiveAsync() → nom de la méthode
    public async Task<List<TraineeDto>> GetActiveAsync()
    {
        try
        {
            // Envoie GET http://serveur/api/trainee/active
            // Récupère le JSON, le convertit en List<TraineeDto>
            var result = await _httpClient.GetFromJsonAsync<List<TraineeDto>>(
                $"{ResourcePath}/active",  // "trainee/active"
                JsonOptions);               // Options de conversion JSON
            
            // result ?? [] → si result est null, retourne liste vide
            return result ?? [];
        }
        catch (Exception ex)
        {
            // En cas d'erreur (API hors ligne, etc.)
            _logger.LogError(ex, "Error fetching active trainees");
            return [];  // Retourne liste vide (pas null)
        }
    }

    // ==================================================
    // MÉTHODE : GET BY STATUS
    // ==================================================
    
    // TraineeStatus status → paramètre : le statut (InProgress, Completed, Suspended)
    public async Task<List<TraineeDto>> GetByStatusAsync(TraineeStatus status)
    {
        try
        {
            // (int)status → convertit l'enum en nombre
            // InProgress = 0, Completed = 1, Suspended = 2
            // $"{ResourcePath}?status={(int)status}" → "trainee?status=0"
            // Envoie GET http://serveur/api/trainee?status=0
            var result = await _httpClient.GetFromJsonAsync<List<TraineeDto>>(
                $"{ResourcePath}?status={(int)status}",  // "trainee?status=0"
                JsonOptions);
            
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching trainees by status {Status}", status);
            return [];
        }
    }

    // ==================================================
    // MÉTHODE : GET DETAILS (DÉTAIL COMPLET AVEC PHASES)
    // ==================================================
    
    // Task<TraineeDetailDto?> → retourne le détail complet ou null
    // int id → l'identifiant du stagiaire
    public async Task<TraineeDetailDto?> GetDetailsAsync(int id)
    {
        try
        {
            // $"{ResourcePath}/{id}/details" → "trainee/5/details"
            // Envoie GET http://serveur/api/trainee/5/details
            // Retourne TraineeDetailDto (avec les phases)
            return await _httpClient.GetFromJsonAsync<TraineeDetailDto>(
                $"{ResourcePath}/{id}/details", JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching trainee details for id {Id}", id);
            return null;  // Retourne null si erreur
        }
    }
}

// ======================================================
// RÉSUMÉ DES URLS UTILISÉES
// ======================================================

// ResourcePath = "trainee"
// _httpClient.BaseAddress = "http://localhost:5001/api/"

// GetAllAsync()           → GET  http://localhost:5001/api/trainee
// GetByIdAsync(5)         → GET  http://localhost:5001/api/trainee/5
// CreateAsync(dto)        → POST http://localhost:5001/api/trainee
// UpdateAsync(5, dto)     → PUT  http://localhost:5001/api/trainee/5
// DeleteAsync(5)          → DELETE http://localhost:5001/api/trainee/5
// GetActiveAsync()        → GET  http://localhost:5001/api/trainee/active
// GetByStatusAsync(0)     → GET  http://localhost:5001/api/trainee?status=0
// GetDetailsAsync(5)      → GET  http://localhost:5001/api/trainee/5/details

// ======================================================
// CE QUE CETTE CLASSE APPORTE (HÉRITAGE)
// ======================================================

// Hérité de BaseApiService (gratuit, sans code à écrire) :
// - GetAllAsync()
// - GetByIdAsync()
// - CreateAsync()
// - UpdateAsync()
// - DeleteAsync()

// Écrit dans cette classe (méthodes spécifiques) :
// - GetActiveAsync()
// - GetByStatusAsync()
// - GetDetailsAsync()











// je vais que tu m'explique chaque ligne de code en commentaire, de façon claire et precise et terre à terre en un seule bloc
// et comment je vais explique ça à mon mentor clairement
