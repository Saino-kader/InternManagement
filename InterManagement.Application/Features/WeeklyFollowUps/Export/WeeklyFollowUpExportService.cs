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
