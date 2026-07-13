using ClosedXML.Excel;

namespace InterManagement.Application.Features.WeeklyFollowUps.Export
{
    public class WeeklyFollowUpExportService : IWeeklyFollowUpExportService
    {
        public byte[] GenerateExcel(string traineeFullName, IEnumerable<ExportRowData> rows)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Suivi");

            // ── En-têtes — ORDRE EXACT DU MODÈLE FOURNI ────────────
            sheet.Cell(1, 1).Value = "Stagiaire";
            sheet.Cell(1, 2).Value = "Mentor";
            sheet.Cell(1, 3).Value = "Date";
            sheet.Cell(1, 4).Value = "Semaine";
            sheet.Cell(1, 5).Value = "Cours";
            sheet.Cell(1, 6).Value = "Appreciation";
            sheet.Cell(1, 7).Value = "Commentaire";
            sheet.Cell(1, 8).Value = "Statut";

            var headerRow = sheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e2242");
            headerRow.Style.Font.FontColor = XLColor.White;

            // ── Lignes de données — toutes les semaines de CE stagiaire ──
            int currentRow = 2;
            foreach (var row in rows)
            {
                sheet.Cell(currentRow, 1).Value = row.TraineeName;
                sheet.Cell(currentRow, 2).Value = row.MentorName;
                sheet.Cell(currentRow, 3).Value = row.FollowUpDate.ToString("yyyy-MM-dd");
                sheet.Cell(currentRow, 4).Value = $"Semaine {row.WeekNumber}";
                sheet.Cell(currentRow, 5).Value = row.Course;
                sheet.Cell(currentRow, 6).Value = row.Appreciation;
                sheet.Cell(currentRow, 7).Value = row.Comment;
                sheet.Cell(currentRow, 8).Value = row.Status;
                currentRow++;
            }

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}














/*

// ======================================================
// IMPORTATION (USING)
// ======================================================

using ClosedXML.Excel;
// ClosedXML = bibliothèque pour créer des fichiers Excel en C#
// Sans cette bibliothèque, impossible de générer des .xlsx

// ======================================================
// NAMESPACE
// ======================================================

namespace InterManagement.Application.Features.WeeklyFollowUps.Export;
// Déclare l'espace de noms (dossier) où se trouve ce fichier
// Correspond au chemin : Application/Features/WeeklyFollowUps/Export/

// ======================================================
// CLASSE SERVICE D'EXPORT EXCEL
// ======================================================

public class WeeklyFollowUpExportService : IWeeklyFollowUpExportService
// public → accessible partout
// class → définit une classe
// WeeklyFollowUpExportService → nom de la classe
// : IWeeklyFollowUpExportService → implémente l'interface

// ======================================================
// MÉTHODE : GenerateExcel
// ======================================================

public byte[] GenerateExcel(string traineeFullName, IEnumerable<ExportRowData> rows)
// byte[] → retourne un tableau d'octets (le fichier Excel)
// traineeFullName → nom complet du stagiaire (ex: "Moussa Diallo")
// rows → les données à écrire dans le fichier

// ======================================================
// ÉTAPE 1 : CRÉER LE CLASSEUR EXCEL
// ======================================================

using var workbook = new XLWorkbook();
// new XLWorkbook() → crée un nouveau fichier Excel vide en mémoire
// using var → ferme automatiquement le fichier à la fin (même en cas d'erreur)

var sheet = workbook.Worksheets.Add("Suivi");
// worksheet → une feuille de calcul (comme un onglet dans Excel)
// .Add("Suivi") → ajoute une feuille nommée "Suivi"
// Si on voulait plusieurs feuilles : .Add("Phase 1"), .Add("Phase 2")...

// ======================================================
// ÉTAPE 2 : ÉCRIRE LES EN-TÊTES
// ======================================================

sheet.Cell(1, 1).Value = "Stagiaire";
// sheet.Cell(ligne, colonne) → accède à une cellule
// (1, 1) = ligne 1, colonne 1 = cellule A1
// .Value = "Stagiaire" → écrit "Stagiaire" dans la cellule

sheet.Cell(1, 2).Value = "Mentor";      // Cellule B1
sheet.Cell(1, 3).Value = "Date";        // Cellule C1
sheet.Cell(1, 4).Value = "Semaine";     // Cellule D1
sheet.Cell(1, 5).Value = "Cours";       // Cellule E1
sheet.Cell(1, 6).Value = "Appreciation"; // Cellule F1
sheet.Cell(1, 7).Value = "Commentaire"; // Cellule G1
sheet.Cell(1, 8).Value = "Statut";      // Cellule H1

// ======================================================
// ÉTAPE 3 : STYLISER LA LIGNE D'EN-TÊTE
// ======================================================

var headerRow = sheet.Row(1);
// sheet.Row(1) → récupère TOUTE la ligne 1 (colonnes A à H)

headerRow.Style.Font.Bold = true;
// Met le texte en gras

headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e2242");
// Met un fond de couleur bleu foncé (#1e2242 = la couleur de ton site GABERA)

headerRow.Style.Font.FontColor = XLColor.White;
// Met le texte en blanc

// ======================================================
// ÉTAPE 4 : ÉCRIRE LES DONNÉES
// ======================================================

int currentRow = 2;
// On commence à la ligne 2 (la ligne 1 est déjà occupée par les en-têtes)

foreach (var row in rows)
// Pour CHAQUE ligne de données dans la liste
{
    // Écrire chaque valeur dans la colonne correspondante
    sheet.Cell(currentRow, 1).Value = row.TraineeName;  // Colonne A : Stagiaire
    sheet.Cell(currentRow, 2).Value = row.MentorName;   // Colonne B : Mentor
    sheet.Cell(currentRow, 3).Value = row.FollowUpDate.ToString("yyyy-MM-dd");
    // Colonne C : Date → format "2026-06-23" (comme dans le modèle)
    
    sheet.Cell(currentRow, 4).Value = $"Semaine {row.WeekNumber}";
    // Colonne D : Semaine → "Semaine 1", "Semaine 2", etc.
    
    sheet.Cell(currentRow, 5).Value = row.Course;       // Colonne E : Cours
    sheet.Cell(currentRow, 6).Value = row.Appreciation; // Colonne F : Appreciation
    sheet.Cell(currentRow, 7).Value = row.Comment;      // Colonne G : Commentaire
    sheet.Cell(currentRow, 8).Value = row.Status;       // Colonne H : Statut
    
    currentRow++;
    // Passe à la ligne suivante
}

// ======================================================
// ÉTAPE 5 : AJUSTER LA LARGEUR DES COLONNES
// ======================================================

sheet.Columns().AdjustToContents();
// Ajuste automatiquement la largeur des colonnes
// pour que le texte soit entièrement visible

// ======================================================
// ÉTAPE 6 : SAUVEGARDER EN MÉMOIRE
// ======================================================

using var stream = new MemoryStream();
// Crée un flux en mémoire (un tableau d'octets)

workbook.SaveAs(stream);
// Sauvegarde le fichier Excel dans le flux

return stream.ToArray();
// Convertit le flux en tableau d'octets (byte[]) et le retourne
// Ce tableau sera envoyé au navigateur pour téléchargement

// ======================================================
// CE QUE LE FICHIER EXCEL CONTIENT
// ======================================================

// ┌──────────────┬──────────────┬────────────┬──────────┬──────────────┬──────────────┬──────────────┬────────────┐
// │ Stagiaire    │ Mentor       │ Date       │ Semaine  │ Cours        │ Appreciation │ Commentaire  │ Statut     │
// ├──────────────┼──────────────┼────────────┼──────────┼──────────────┼──────────────┼──────────────┼────────────┤
// │ Moussa Diallo│ Sophie Martin│ 2026-06-23 │ Semaine 1│ Introduction │ Bien         │ Bon travail  │ Done       │
// │ Moussa Diallo│ Sophie Martin│ 2026-06-24 │ Semaine 2│ POO          │ Très bien    │ Continue     │ Done       │
// └──────────────┴──────────────┴────────────┴──────────┴──────────────┴──────────────┴──────────────┴────────────┘

// ======================================================
// RÉSUMÉ
// ======================================================

// 1. Crée un fichier Excel en mémoire (ne touche pas au disque)
// 2. Écrit les en-têtes avec style (bleu foncé, texte blanc)
// 3. Écrit toutes les lignes de données
// 4. Ajuste les colonnes
// 5. Retourne le fichier en byte[] (prêt à être téléchargé)













*/
