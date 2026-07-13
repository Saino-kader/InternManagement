using InterManagement.Application.Features.WeeklyFollowUps.Export;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.WeeklyFollowUps.Queries.ExportWeeklyFollowUpsByTrainee
{
    public class ExportWeeklyFollowUpsByTraineeHandler
    {
        private readonly IWeeklyFollowUpRepository _followUpRepository;
        private readonly ITraineeRepository _traineeRepository;
        private readonly IWeeklyFollowUpExportService _exportService;

        public ExportWeeklyFollowUpsByTraineeHandler(
            IWeeklyFollowUpRepository followUpRepository,
            ITraineeRepository traineeRepository,
            IWeeklyFollowUpExportService exportService)
        {
            _followUpRepository = followUpRepository;
            _traineeRepository = traineeRepository;
            _exportService = exportService;
        }

        public async Task<ExportFileResult> Handle(
            ExportWeeklyFollowUpsByTraineeQuery query)
        {
            // 1. Vérifier que le stagiaire existe
            var trainee = await _traineeRepository.GetByIdAsync(query.TraineeId);
            if (trainee == null)
                throw new TraineeNotFoundException(query.TraineeId);

            var fullName = $"{trainee.FirstName} {trainee.LastName}";

            // 2. Charger tous les suivis de CE stagiaire uniquement
            var followUps = await _followUpRepository
                .GetByTraineeForExportAsync(query.TraineeId);

            // 3. Transformer en lignes pour le fichier Excel
            var exportRows = followUps.Select(f => new ExportRowData
            {
                TraineeName = fullName,
                MentorName = $"{f.Mentor.FirstName} {f.Mentor.LastName}",
                FollowUpDate = f.FollowUpDate,
                WeekNumber = f.Week.WeekNumber,
                Course = f.CourseName,
                Appreciation = f.Appreciation,
                Comment = f.Comment,
                Status = f.Status.ToString()
            });

            // 4. Générer le fichier Excel
            var fileBytes = _exportService.GenerateExcel(fullName, exportRows);

            // 5. Construire le nom du fichier exactement comme ton modèle :
            //    suivi_Moussa_ISSA_2025-01-12.xlsx
            var safeFirstName = trainee.FirstName.Replace(" ", "_");
            var safeLastName = trainee.LastName.Replace(" ", "_");
            var dateStamp = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var fileName = $"suivi_{safeFirstName}_{safeLastName}_{dateStamp}.xlsx";

            return new ExportFileResult
            {
                FileBytes = fileBytes,
                FileName = fileName
            };
        }
    }
}
