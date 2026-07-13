namespace InterManagement.Application.Features.WeeklyFollowUps.Queries.ExportWeeklyFollowUpsByTrainee
{
    /// <summary>
    /// Résultat retourné : le fichier ET son nom (pour le téléchargement)
    /// </summary>
    public class ExportFileResult
    {
        public byte[] FileBytes { get; set; } = [];
        public string FileName { get; set; } = string.Empty;
    }
}
