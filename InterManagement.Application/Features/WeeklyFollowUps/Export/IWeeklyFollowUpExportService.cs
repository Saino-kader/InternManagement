namespace InterManagement.Application.Features.WeeklyFollowUps.Export
{
    public interface IWeeklyFollowUpExportService
    {
        /// <summary>
        /// Génère le fichier Excel pour UN SEUL stagiaire
        /// </summary>
        byte[] GenerateExcel(string traineeFullName, IEnumerable<ExportRowData> rows);
    }

    public class ExportRowData
    {
        public string TraineeName { get; set; } = string.Empty;
        public string MentorName { get; set; } = string.Empty;
        public DateOnly FollowUpDate { get; set; }
        public int WeekNumber { get; set; }
        public string Course { get; set; } = string.Empty;
        public string Appreciation { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
