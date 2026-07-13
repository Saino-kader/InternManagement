using InterManagement.Application.Features.WeeklyFollowUps.DTOs;

namespace InterManagement.Application.Features.WeeklyFollowUps.Commands.ImportWeeklyFollowUps
{
    public class ImportWeeklyFollowUpsCommand
    {
        public Stream FileStream { get; set; }
        public string FileExtension { get; set; }

        public ImportWeeklyFollowUpsCommand(Stream fileStream, string fileExtension)
        {
            FileStream = fileStream;
            FileExtension = fileExtension;
        }
    }

    public class ParsedFollowUpRow
    {
        public string TraineeFullName { get; set; } = string.Empty;
        public string MentorFullName { get; set; } = string.Empty;
        public DateOnly FollowUpDate { get; set; }
        public int WeekNumber { get; set; }
        public int WeekId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string Appreciation { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string RawStatus { get; set; } = string.Empty;
    }

}





























































































/*


// ======================================================
// IMPORTATION (USING)
// ======================================================

// Importe les DTOs du module WeeklyFollowUps (CreateWeeklyFollowUpDto, WeeklyFollowUpDto...)
// Cela permet d'utiliser ces DTOs dans cette classe si nécessaire
using InterManagement.Application.Features.WeeklyFollowUps.DTOs;

// ======================================================
// NAMESPACE
// ======================================================

// Déclare l'espace de noms (dossier) où se trouve ce fichier
// Correspond au chemin : Application/Features/WeeklyFollowUps/Commands/ImportWeeklyFollowUps/
namespace InterManagement.Application.Features.WeeklyFollowUps.Commands.ImportWeeklyFollowUps;

// ======================================================
// CLASSE COMMAND POUR IMPORTER DES SUIVIS HEBDOMADAIRES
// ======================================================

// public → accessible par le Controller et le Handler
// class ImportWeeklyFollowUpsCommand → nom de la commande
// Une Command = une ENVELOPPE qui transporte les données vers le Handler
public class ImportWeeklyFollowUpsCommand
{
    // public → accessible de l'extérieur
    // Stream → un flux de données binaires (le fichier Excel/CSV en mémoire)
    // FileStream → nom de la propriété
    // { get; set; } → on peut lire et écrire
    public Stream FileStream { get; set; }
    
    // public → accessible de l'extérieur
    // string → le type de fichier (".xlsx", ".csv", ".xls")
    // FileExtension → nom de la propriété
    // { get; set; } → on peut lire et écrire
    public string FileExtension { get; set; }

    // ==================================================
    // CONSTRUCTEUR
    // ==================================================
    
    // public → accessible de l'extérieur
    // ImportWeeklyFollowUpsCommand → nom du constructeur (identique à la classe)
    // Stream fileStream → paramètre : le flux du fichier uploadé
    // string fileExtension → paramètre : l'extension du fichier
    public ImportWeeklyFollowUpsCommand(Stream fileStream, string fileExtension)
    {
        // Stocke le paramètre fileStream dans la propriété FileStream
        FileStream = fileStream;
        // Stocke le paramètre fileExtension dans la propriété FileExtension
        FileExtension = fileExtension;
    }
}

// ======================================================
// CLASSE POUR UNE LIGNE DE SUIVI EXTRAITE DU FICHIER
// ======================================================

// public → accessible par le Handler
// class ParsedFollowUpRow → contient les données d'UNE ligne du fichier Excel
// Cette classe représente les données BRUTES avant validation et conversion en entité
public class ParsedFollowUpRow
{
    // public → accessible de l'extérieur
    // string → le nom complet du stagiaire (ex: "Moussa Diallo")
    // TraineeFullName → nom de la propriété
    // = string.Empty → valeur par défaut = chaîne vide (pas null)
    // Pourquoi ? Le fichier Excel contient des noms complets, pas des IDs
    public string TraineeFullName { get; set; } = string.Empty;
    
    // public → accessible de l'extérieur
    // string → le nom complet du mentor (ex: "Sophie Martin")
    // MentorFullName → nom de la propriété
    // = string.Empty → valeur par défaut = chaîne vide (pas null)
    // Pourquoi ? Le fichier Excel contient des noms complets, pas des IDs
    public string MentorFullName { get; set; } = string.Empty;
    
    // public → accessible de l'extérieur
    // DateOnly → seulement la date (pas l'heure)
    // FollowUpDate → la date du suivi
    // Pourquoi ? La date du suivi est une date (sans heure)
    public DateOnly FollowUpDate { get; set; }
    
    // public → accessible de l'extérieur
    // int → numéro de la semaine (1, 2, 3, 4...)
    // WeekNumber → le numéro de la semaine
    // Pourquoi ? Le suivi concerne une semaine spécifique
    public int WeekNumber { get; set; }
    
    // public → accessible de l'extérieur
    // string → le nom du cours (ex: "Les bases de C#")
    // CourseName → le nom du cours
    // = string.Empty → valeur par défaut = chaîne vide (pas null)
    public string CourseName { get; set; } = string.Empty;
    
    // public → accessible de l'extérieur
    // string → l'appréciation du mentor (ex: "Bien", "Très bien", "Passable")
    // Appreciation → l'appréciation
    // = string.Empty → valeur par défaut = chaîne vide (pas null)
    public string Appreciation { get; set; } = string.Empty;
    
    // public → accessible de l'extérieur
    // string → le commentaire du mentor
    // Comment → le commentaire
    // = string.Empty → valeur par défaut = chaîne vide (pas null)
    public string Comment { get; set; } = string.Empty;
    
    // public → accessible de l'extérieur
    // string → le statut BRUT lu dans le fichier (ex: "Done", "Validé", "Pas encore")
    // RawStatus → nom de la propriété
    // = string.Empty → valeur par défaut = chaîne vide (pas null)
    // Pourquoi ? On garde le texte original pour le convertir ensuite en enum
    public string RawStatus { get; set; } = string.Empty;
}

// ======================================================
// DTO POUR LE RÉSULTAT DE L'IMPORT
// ======================================================

// public → accessible par le Controller et le Client
// class ImportResultDto → contient les statistiques de l'import
// Ce DTO est retourné à l'Admin pour lui montrer le résultat de l'import
public class ImportResultDto
{
    // public → accessible de l'extérieur
    // int → nombre TOTAL de lignes lues dans le fichier
    // TotalRows → le nombre total de lignes
    // Pourquoi ? Pour savoir combien de lignes ont été lues
    public int TotalRows { get; set; }
    
    // public → accessible de l'extérieur
    // int → nombre de lignes IMPORTÉES avec succès (créées en base)
    // ImportedRows → le nombre de lignes importées
    // Pourquoi ? Pour savoir combien de suivis ont été créés
    public int ImportedRows { get; set; }
    
    // public → accessible de l'extérieur
    // List<string> → une liste de messages d'erreur
    // Errors → le nom de la propriété
    // = [] → initialise avec une liste vide (pas null)
    // Pourquoi ? Pour stocker les erreurs (ex: "Stagiaire introuvable", "Mentor introuvable")
    // L'Admin pourra voir les erreurs pour corriger le fichier
    public List<string> Errors { get; set; } = [];
}

// ======================================================
// FLUX COMPLET DE L'IMPORT
// ======================================================

// 1. L'Admin upload un fichier Excel depuis le dashboard
// 2. Le Controller reçoit le fichier et crée ImportWeeklyFollowUpsCommand
// 3. Le Handler Parse le fichier → obtient une liste de ParsedFollowUpRow
// 4. Pour chaque ligne : résout les noms en IDs (trouve TraineeId, MentorId, PhaseId)
// 5. Crée et sauvegarde les WeeklyFollowUp en base
// 6. Retourne ImportResultDto à l'Admin (TotalRows, ImportedRows, Errors)

// ======================================================
// CE QUE L'ADMIN VOIT APRÈS L'IMPORT
// ======================================================

// ImportResultDto retourné en JSON :
// {
//   "totalRows": 24,
//   "importedRows": 22,
//   "errors": [
//     "Ligne 5: Stagiaire 'Jean Dupont' introuvable",
//     "Ligne 10: Mentor 'ASMA' introuvable"
//   ]
// }

















*/
