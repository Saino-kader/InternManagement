// Application/Features/ImportedFollowUps/Commands/ImportExcel/ImportExcelHandler.cs
//
// Ce Handler lit le fichier Excel et stocke les données
// DIRECTEMENT dans la table imported_followups
// SANS aucune vérification (stagiaire existe ou pas)
// SANS lien avec les autres tables
// Toujours réussi tant que le fichier est lisible.

using ClosedXML.Excel;
using InterManagement.Application.Features.ImportedFollowUps.DTOs;
using InterManagement.Domain.Entities;
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

                    // Colonne 3 — Date
                    var dateStr = sheet.Cell(rowNum, 3).GetString().Trim();
                    var date = ParseDate(dateStr);

                    // Colonne 4 — Phase (numéro)
                    var phaseStr = sheet.Cell(rowNum, 4).GetString().Trim();
                    var phaseNumber = ParseInt(phaseStr);

                    // Colonne 5 — Semaine (ex: "Semaine 1" → 1)
                    var semaineStr = sheet.Cell(rowNum, 5).GetString().Trim();
                    var weekNumber = ParseWeekNumber(semaineStr);

                    // Colonne 6 — Cours
                    var cours = sheet.Cell(rowNum, 6).GetString().Trim();
                    if (string.IsNullOrWhiteSpace(cours)) cours = "—";

                    // Colonne 7 — Appréciation
                    var appreciation = sheet.Cell(rowNum, 7).GetString().Trim();
                    if (string.IsNullOrWhiteSpace(appreciation)) appreciation = "—";

                    // Colonne 8 — Commentaire (peut être vide)
                    var commentaire = sheet.Cell(rowNum, 8).GetString().Trim();

                    // Colonne 9 — Statut
                    var statut = sheet.Cell(rowNum, 9).GetString().Trim();
                    if (string.IsNullOrWhiteSpace(statut)) statut = "—";

                    // Crée l'entité — stockage texte brut, sans vérification
                    var followUp = new ImportedFollowUp(
                        stagiaire:    stagiaire,
                        mentor:       mentor,
                        date:         date,
                        phaseNumber:  phaseNumber,
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

        // ── Parseurs ─────────────────────────────────────────────────

        private static DateOnly ParseDate(string value)
        {
            if (DateOnly.TryParse(value, out var d)) return d;
            if (DateTime.TryParse(value, out var dt)) return DateOnly.FromDateTime(dt);
            return DateOnly.FromDateTime(DateTime.Today);
        }

        private static int ParseInt(string value)
        {
            var cleaned = System.Text.RegularExpressions.Regex
                .Replace(value, @"[^\d]", "");
            int.TryParse(cleaned, out var result);
            return result;
        }

        private static int ParseWeekNumber(string value)
        {
            // "Semaine 1" → 1, "1" → 1, "Week 3" → 3
            return ParseInt(value);
        }
    }
}
