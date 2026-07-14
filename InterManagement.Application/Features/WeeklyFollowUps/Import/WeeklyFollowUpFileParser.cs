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
