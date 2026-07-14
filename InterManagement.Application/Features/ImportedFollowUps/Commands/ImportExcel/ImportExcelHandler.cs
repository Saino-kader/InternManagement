// Application/Features/ImportedFollowUps/Commands/ImportExcel/ImportExcelHandler.cs
//
// Ce Handler lit le fichier Excel et stocke les données dans la
// table imported_followups, SANS lien avec les autres tables
// (Trainee, Mentor, Week...). Les colonnes attendues correspondent
// exactement au modèle téléchargeable (downloadTemplate() dans
// suivi.js) : Stagiaire | Mentor | Date | Semaine | Cours |
// Appréciation | Commentaire | Statut.
// Chaque ligne est validée avant d'être stockée ; une ligne
// invalide (date illisible, semaine absente/non numérique...) est
// ignorée et signalée dans les erreurs, jamais enregistrée telle
// quelle.

using ClosedXML.Excel;
using InterManagement.Application.Features.ImportedFollowUps.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.ImportedFollowUps.Commands.ImportExcel
{
    public class ImportExcelCommand
    {
        public Stream FileStream      { get; set; } = null!;
        public string FileExtension   { get; set; } = string.Empty;
    }

    public class ImportExcelHandler
    {
        private readonly IImportedFollowUpRepository _repository;

        public ImportExcelHandler(IImportedFollowUpRepository repository)
        {
            _repository = repository;
        }

        public async Task<ImportedFollowUpResultDto> Handle(ImportExcelCommand command)
        {
            var result = new ImportedFollowUpResultDto
            {
                // BatchId unique pour identifier cet import
                BatchId = $"IMPORT-{DateTime.UtcNow:yyyyMMdd-HHmmss}"
            };

            var rows = new List<ImportedFollowUp>();

            try
            {
                using var workbook = new XLWorkbook(command.FileStream);
                var sheet = workbook.Worksheet(1);

                int rowCount = sheet.LastRowUsed()?.RowNumber() ?? 0;
                result.TotalRows = rowCount - 1; // sans l'en-tête

                // Commence à la ligne 2 (ligne 1 = en-tête)
                // Colonnes : Stagiaire | Mentor | Date | Semaine | Cours |
                // Appréciation | Commentaire | Statut
                for (int rowNum = 2; rowNum <= rowCount; rowNum++)
                {
                    // Colonne 1 — Stagiaire (obligatoire)
                    var stagiaire = sheet.Cell(rowNum, 1).GetString().Trim();
                    if (string.IsNullOrWhiteSpace(stagiaire))
                    {
                        result.ErrorCount++;
                        result.Errors.Add($"Ligne {rowNum} : nom du stagiaire vide — ignorée");
                        continue;
                    }

                    // Colonne 2 — Mentor (peut être vide)
                    var mentor = sheet.Cell(rowNum, 2).GetString().Trim();
                    if (string.IsNullOrWhiteSpace(mentor)) mentor = "—";

                    // Colonne 3 — Date (obligatoire et valide)
                    var dateStr = sheet.Cell(rowNum, 3).GetString().Trim();
                    if (!TryParseDate(dateStr, out var date))
                    {
                        result.ErrorCount++;
                        result.Errors.Add($"Ligne {rowNum} : date invalide (\"{dateStr}\") — ignorée");
                        continue;
                    }

                    // Colonne 4 — Semaine (obligatoire, entier positif)
                    var semaineStr = sheet.Cell(rowNum, 4).GetString().Trim();
                    if (!TryParseWeekNumber(semaineStr, out var weekNumber))
                    {
                        result.ErrorCount++;
                        result.Errors.Add($"Ligne {rowNum} : numéro de semaine invalide (\"{semaineStr}\") — ignorée");
                        continue;
                    }

                    // Colonne 5 — Cours
                    var cours = sheet.Cell(rowNum, 5).GetString().Trim();
                    if (string.IsNullOrWhiteSpace(cours)) cours = "—";

                    // Colonne 6 — Appréciation
                    var appreciation = sheet.Cell(rowNum, 6).GetString().Trim();
                    if (string.IsNullOrWhiteSpace(appreciation)) appreciation = "—";

                    // Colonne 7 — Commentaire (peut être vide)
                    var commentaire = sheet.Cell(rowNum, 7).GetString().Trim();

                    // Colonne 8 — Statut
                    var statut = sheet.Cell(rowNum, 8).GetString().Trim();
                    if (string.IsNullOrWhiteSpace(statut)) statut = "—";

                    try
                    {
                        var followUp = new ImportedFollowUp(
                            stagiaire:    stagiaire,
                            mentor:       mentor,
                            date:         date,
                            weekNumber:   weekNumber,
                            cours:        cours,
                            appreciation: appreciation,
                            commentaire:  commentaire,
                            statut:       statut,
                            batchId:      result.BatchId
                        );

                        rows.Add(followUp);
                        result.SuccessCount++;
                    }
                    catch (DomainException ex)
                    {
                        result.ErrorCount++;
                        result.Errors.Add($"Ligne {rowNum} : {ex.Message} — ignorée");
                    }
                }

                // Sauvegarde tout en une seule opération
                if (rows.Any())
                    await _repository.AddRangeAsync(rows);
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Erreur de lecture du fichier : {ex.Message}");
            }

            return result;
        }

        // ── Parseurs / validateurs ─────────────────────────────────────

        private static bool TryParseDate(string value, out DateOnly date)
        {
            if (DateOnly.TryParse(value, out date)) return true;
            if (DateTime.TryParse(value, out var dt))
            {
                date = DateOnly.FromDateTime(dt);
                return true;
            }
            date = default;
            return false;
        }

        // "Semaine 1" → 1, "1" → 1, "Week 3" → 3 — doit être un entier positif
        private static bool TryParseWeekNumber(string value, out int weekNumber)
        {
            var cleaned = System.Text.RegularExpressions.Regex
                .Replace(value, @"[^\d]", "");

            if (int.TryParse(cleaned, out weekNumber) && weekNumber > 0)
                return true;

            weekNumber = 0;
            return false;
        }
    }
}
