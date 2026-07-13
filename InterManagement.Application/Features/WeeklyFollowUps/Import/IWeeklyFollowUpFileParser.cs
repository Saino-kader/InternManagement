using InterManagement.Application.Features.WeeklyFollowUps.Commands.ImportWeeklyFollowUps;

namespace InterManagement.Application.Features.WeeklyFollowUps.Import
{
    public interface IWeeklyFollowUpFileParser
    {
        /// <summary>
        /// Parse un fichier Excel (.xlsx) contenant 1 à plusieurs stagiaires
        /// et retourne les lignes parsées.
        /// </summary>
        Task<List<ParsedFollowUpRow>> ParseAsync(Stream fileStream, string fileExtension);
    }
}
