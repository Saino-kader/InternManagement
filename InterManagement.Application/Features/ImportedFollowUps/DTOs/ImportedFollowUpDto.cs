// Application/Features/ImportedFollowUps/DTOs/ImportedFollowUpDto.cs
namespace InterManagement.Application.Features.ImportedFollowUps.DTOs
{
    public class ImportedFollowUpDto
    {
        public int Id { get; set; }

        // Données brutes du fichier Excel
        public string Stagiaire    { get; set; } = string.Empty;
        public string Mentor       { get; set; } = string.Empty;
        public DateOnly Date       { get; set; }
        public int WeekNumber      { get; set; }
        public string Cours        { get; set; } = string.Empty;
        public string Appreciation { get; set; } = string.Empty;
        public string Commentaire  { get; set; } = string.Empty;
        public string Statut       { get; set; } = string.Empty;

        // Métadonnées
        public DateTime ImportedAt { get; set; }
        public string BatchId      { get; set; } = string.Empty;
    }

    public class UpdateImportedFollowUpDto
    {
        public string Cours        { get; set; } = string.Empty;
        public string Appreciation { get; set; } = string.Empty;
        public string Commentaire  { get; set; } = string.Empty;
        public string Statut       { get; set; } = string.Empty;
    }

    public class ImportedFollowUpResultDto
    {
        public int TotalRows    { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount   { get; set; }
        public string BatchId   { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = [];
    }
}
