using ClosedXML.Excel;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.ImportWeeklyFollowUps;

namespace InterManagement.Application.Features.WeeklyFollowUps.Import
{
    public class WeeklyFollowUpFileParser : IWeeklyFollowUpFileParser
    {
        public async Task<List<ParsedFollowUpRow>> ParseAsync(
            Stream fileStream, string fileExtension)
        {
            var rows = new List<ParsedFollowUpRow>();

            await Task.Run(() =>
            {
                using var workbook = new XLWorkbook(fileStream);
                var sheet = workbook.Worksheet(1); // Première feuille

                // Parcourir les lignes (commencer à la ligne 2 pour ignorer l'en-tête)
                int rowCount = sheet.LastRowUsed()?.RowNumber() ?? 0;

                for (int row = 2; row <= rowCount; row++)
                {
                    var traineeName = sheet.Cell(row, 1).GetString().Trim();
                    if (string.IsNullOrWhiteSpace(traineeName)) continue;

                    rows.Add(new ParsedFollowUpRow
                    {
                        TraineeFullName = traineeName,
                        MentorFullName = sheet.Cell(row, 2).GetString().Trim(),
                        FollowUpDate = ParseDateOnly(sheet.Cell(row, 3).GetString()),
                        WeekNumber = ParseInt(sheet.Cell(row, 4).GetString()),
                        CourseName = sheet.Cell(row, 5).GetString().Trim(),
                        Appreciation = sheet.Cell(row, 6).GetString().Trim(),
                        Comment = sheet.Cell(row, 7).GetString().Trim(),
                        RawStatus = sheet.Cell(row, 8).GetString().Trim()
                    });
                }
            });

            return rows;
        }

        private static DateOnly ParseDateOnly(string value)
        {
            if (DateOnly.TryParse(value, out var result))
                return result;

            // Fallback : essayer de parser depuis DateTime
            if (DateTime.TryParse(value, out var dt))
                return DateOnly.FromDateTime(dt);

            return DateOnly.MinValue;
        }

        private static int ParseInt(string value)
        {
            // Nettoie "Semaine 3" → 3
            var cleaned = System.Text.RegularExpressions.Regex.Replace(value, @"[^\d]", "");
            int.TryParse(cleaned, out var result);
            return result;
        }
    }
}



















/*

// ======================================================
// IMPORTATIONS (USING)
// ======================================================

using ClosedXML.Excel;
// Bibliothèque pour lire et écrire des fichiers Excel
// Permet d'ouvrir un fichier .xlsx et de lire son contenu

using InterManagement.Application.Features.WeeklyFollowUps.Commands.ImportWeeklyFollowUps;
// Importe ParsedFollowUpRow (la classe qui représente une ligne du fichier)

// ======================================================
// NAMESPACE
// ======================================================

namespace InterManagement.Application.Features.WeeklyFollowUps.Import;
// Déclare l'espace de noms (dossier) : Application/Features/WeeklyFollowUps/Import/

// ======================================================
// CLASSE PARSER DE FICHIER EXCEL
// ======================================================

public class WeeklyFollowUpFileParser : IWeeklyFollowUpFileParser
// public → accessible partout
// class → définit une classe
// WeeklyFollowUpFileParser → nom de la classe
// : IWeeklyFollowUpFileParser → implémente l'interface

// ======================================================
// MÉTHODE PRINCIPALE : ParseAsync
// ======================================================

public async Task<List<ParsedFollowUpRow>> ParseAsync(
    Stream fileStream, string fileExtension)
// async → méthode asynchrone
// Task<List<ParsedFollowUpRow>> → retourne une LISTE de lignes parsées
// Stream fileStream → le fichier uploadé (en mémoire)
// string fileExtension → l'extension (".xlsx", ".xls", ".csv")

// ======================================================
// ÉTAPE 1 : CRÉER LA LISTE DE RÉSULTAT
// ======================================================

var rows = new List<ParsedFollowUpRow>();
// Crée une liste vide pour stocker les lignes parsées

// ======================================================
// ÉTAPE 2 : LIRE LE FICHIER EXCEL
// ======================================================

await Task.Run(() =>
// Task.Run → exécute le code dans un thread séparé (pour ne pas bloquer)
// await → attend que la lecture soit terminée
{
    using var workbook = new XLWorkbook(fileStream);
    // XLWorkbook → ouvre le fichier Excel
    // using → ferme automatiquement le fichier à la fin
    
    var sheet = workbook.Worksheet(1);
    // Prend la PREMIÈRE feuille du classeur (index 1)
    // Si le fichier a plusieurs feuilles, on prend la première

// ======================================================
// ÉTAPE 3 : DÉTERMINER LE NOMBRE DE LIGNES
// ======================================================

int rowCount = sheet.LastRowUsed()?.RowNumber() ?? 0;
// sheet.LastRowUsed() → trouve la dernière ligne qui contient des données
// ?.RowNumber() → si la dernière ligne existe, prend son numéro
// ?? 0 → si aucune ligne, retourne 0

// Exemple : si le fichier a 24 lignes de données, rowCount = 24

// ======================================================
// ÉTAPE 4 : PARCOURIR CHAQUE LIGNE
// ======================================================

for (int row = 2; row <= rowCount; row++)
// row = 2 → on commence à la ligne 2 (ligne 1 = en-têtes)
// row <= rowCount → on parcourt jusqu'à la dernière ligne
// row++ → on passe à la ligne suivante

// ======================================================
// ÉTAPE 5 : LIRE LE NOM DU STAGIAIRE
// ======================================================

var traineeName = sheet.Cell(row, 1).GetString().Trim();
// sheet.Cell(row, 1) → cellule à la ligne X, colonne 1 (colonne A)
// .GetString() → lit le contenu de la cellule comme texte
// .Trim() → enlève les espaces avant et après

if (string.IsNullOrWhiteSpace(traineeName)) continue;
// Si le nom du stagiaire est vide → on saute cette ligne
// car une ligne sans stagiaire n'est pas valide

// ======================================================
// ÉTAPE 6 : CRÉER UN OBJET POUR CETTE LIGNE
// ======================================================

rows.Add(new ParsedFollowUpRow
{
    TraineeFullName = traineeName,  // Colonne A : "Moussa Diallo"
    MentorFullName = sheet.Cell(row, 2).GetString().Trim(),  // Colonne B : "Sophie Martin"
    FollowUpDate = ParseDateOnly(sheet.Cell(row, 3).GetString()),  // Colonne C : "2026-06-23"
    WeekNumber = ParseInt(sheet.Cell(row, 4).GetString()),  // Colonne D : "Semaine 1" → 1
    CourseName = sheet.Cell(row, 5).GetString().Trim(),  // Colonne E : "Introduction C#"
    Appreciation = sheet.Cell(row, 6).GetString().Trim(),  // Colonne F : "Bien"
    Comment = sheet.Cell(row, 7).GetString().Trim(),  // Colonne G : "Bon travail"
    RawStatus = sheet.Cell(row, 8).GetString().Trim()  // Colonne H : "Done"
});
// Ajoute la ligne à la liste

// ======================================================
// MÉTHODE : ParseDateOnly (CONVERTIR UNE DATE)
// ======================================================

private static DateOnly ParseDateOnly(string value)
// static → méthode partagée (pas besoin d'instance)
// DateOnly → retourne une date (sans heure)
// string value → la date en texte

{
    if (DateOnly.TryParse(value, out var result))
        return result;
    // Si la chaîne est déjà au format DateOnly (ex: "2026-06-23") → retourne directement

    if (DateTime.TryParse(value, out var dt))
        return DateOnly.FromDateTime(dt);
    // Si la chaîne est au format DateTime (ex: "2026-06-23 00:00:00") → convertit en DateOnly

    return DateOnly.MinValue;
    // Si aucun format ne fonctionne → retourne une date vide (0001-01-01)
}

// Exemples :
// ParseDateOnly("2026-06-23") → 2026-06-23
// ParseDateOnly("2026-06-23 00:00:00") → 2026-06-23
// ParseDateOnly("") → 0001-01-01

// ======================================================
// MÉTHODE : ParseInt (EXTRAIRE UN NOMBRE D'UN TEXTE)
// ======================================================

private static int ParseInt(string value)
// static → méthode partagée
// int → retourne un nombre entier
// string value → le texte contenant un nombre

{
    var cleaned = System.Text.RegularExpressions.Regex.Replace(value, @"[^\d]", "");
    // Supprime TOUS les caractères qui ne sont PAS des chiffres
    // Regex.Replace(value, @"[^\d]", "") → garde seulement les chiffres
    
    // Exemples :
    // "Semaine 1" → "1"
    // "Semaine 3" → "3"
    // "5" → "5"
    // "" → ""

    int.TryParse(cleaned, out var result);
    // Tente de convertir la chaîne nettoyée en nombre
    // Si ça réussit → result = le nombre
    // Si ça échoue → result = 0

    return result;
}

// ======================================================
// EXEMPLE D'EXÉCUTION
// ======================================================

// Fichier Excel importé :
// ┌─────────────────┬──────────────┬────────────┬──────────┬──────────────┬──────────────┬──────────────┬────────────┐
// │ Stagiaire       │ Mentor       │ Date       │ Semaine  │ Cours        │ Appreciation │ Commentaire  │ Statut     │
// ├─────────────────┼──────────────┼────────────┼──────────┼──────────────┼──────────────┼──────────────┼────────────┤
// │ Moussa Diallo   │ Sophie Martin│ 2026-06-23 │ Semaine 1│ Introduction │ Bien         │ Bon travail  │ Done       │
// │ Hassan Billo    │ Ali Abass    │ 2026-06-23 │ Semaine 3│ ASP.NET      │ Très bien    │ Continue     │ Validé     │
// └─────────────────┴──────────────┴────────────┴──────────┴──────────────┴──────────────┴──────────────┴────────────┘

// rows après parsing :
// [
//   {
//     TraineeFullName: "Moussa Diallo",
//     MentorFullName: "Sophie Martin",
//     FollowUpDate: 2026-06-23,
//     WeekNumber: 1,
//     CourseName: "Introduction",
//     Appreciation: "Bien",
//     Comment: "Bon travail",
//     RawStatus: "Done"
//   },
//   {
//     TraineeFullName: "Hassan Billo",
//     MentorFullName: "Ali Abass",
//     FollowUpDate: 2026-06-23,
//     WeekNumber: 3,
//     CourseName: "ASP.NET",
//     Appreciation: "Très bien",
//     Comment: "Continue",
//     RawStatus: "Validé"
//   }
// ]

// ======================================================
// RÉSUMÉ
// ======================================================

// 1. Lit le fichier Excel uploadé
// 2. Ignore la ligne d'en-tête (ligne 1)
// 3. Pour chaque ligne de données, lit les 8 colonnes
// 4. Convertit les valeurs (date, semaine, etc.)
// 5. Retourne une liste de ParsedFollowUpRow
// 6. Cette liste sera utilisée par le Handler pour créer les WeeklyFollowUp en base

*/
