using InterManagement.Application.Features.WeeklyFollowUps.DTOs;
using InterManagement.Application.Features.WeeklyFollowUps.Import;
using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;
using InterManagement.Shared.Enums;

namespace InterManagement.Application.Features.WeeklyFollowUps.Commands.ImportWeeklyFollowUps
{
    public class ImportWeeklyFollowUpsHandler
    {
        private readonly IWeeklyFollowUpRepository _followUpRepository;
        private readonly IWeeklyFollowUpFileParser _fileParser;
        private readonly IPhaseRepository _phaseRepository;
        private readonly IWeekRepository _weekRepository;
        private readonly IAssignmentRepository _assignmentRepository;

        public ImportWeeklyFollowUpsHandler(
            IWeeklyFollowUpRepository followUpRepository,
            IWeeklyFollowUpFileParser fileParser,
            IPhaseRepository phaseRepository,
            IWeekRepository weekRepository,
            IAssignmentRepository assignmentRepository)
        {
            _followUpRepository = followUpRepository;
            _fileParser = fileParser;
            _phaseRepository = phaseRepository;
            _weekRepository = weekRepository;
            _assignmentRepository = assignmentRepository;
        }

        public async Task<ImportResultDto> Handle(ImportWeeklyFollowUpsCommand command)
        {
            var result = new ImportResultDto();
            var parsedRows = await _fileParser.ParseAsync(
                command.FileStream, command.FileExtension);
            result.TotalRowsRead = parsedRows.Count;

            // Collecte les suivis valides sans les sauvegarder ligne par
            // ligne, pour tout écrire en une seule transaction à la fin.
            var validFollowUps = new List<WeeklyFollowUp>();

            foreach (var row in parsedRows)
            {
                try
                {
                    var traineeId = await _followUpRepository
                        .FindTraineeIdByFullNameAsync(row.TraineeFullName);
                    if (traineeId == null)
                    {
                        result.Errors.Add(new ImportRowErrorDto
                        {
                            RowNumber = parsedRows.IndexOf(row) + 1,
                            Reason = $"Stagiaire introuvable : {row.TraineeFullName}"
                        });
                        continue;
                    }

                    var mentorId = await _followUpRepository
                        .FindMentorIdByFullNameAsync(row.MentorFullName);
                    if (mentorId == null)
                    {
                        result.Errors.Add(new ImportRowErrorDto
                        {
                            RowNumber = parsedRows.IndexOf(row) + 1,
                            Reason = $"Mentor introuvable : {row.MentorFullName}"
                        });
                        continue;
                    }

                    // L'assignment permet de retrouver la phase (et donc
                    // la semaine) qui relie ce stagiaire à ce mentor.
                    var assignments = await _assignmentRepository.GetByMentorAsync(mentorId.Value);
                    var assignment = assignments.FirstOrDefault(a => a.TraineeId == traineeId.Value && a.IsActive);
                    if (assignment == null)
                    {
                        result.Errors.Add(new ImportRowErrorDto
                        {
                            RowNumber = parsedRows.IndexOf(row) + 1,
                            Reason = $"Aucune assignment trouvée pour {row.TraineeFullName} avec mentor {row.MentorFullName}"
                        });
                        continue;
                    }

                    var phase = await _phaseRepository.GetByIdAsync(assignment.PhaseId);
                    if (phase == null)
                    {
                        result.Errors.Add(new ImportRowErrorDto
                        {
                            RowNumber = parsedRows.IndexOf(row) + 1,
                            Reason = $"Phase introuvable pour l'assignment de {row.TraineeFullName}"
                        });
                        continue;
                    }

                    var weeks = await _weekRepository.GetByPhaseAsync(phase.Id);
                    var week = weeks.FirstOrDefault(w => w.WeekNumber == row.WeekNumber);
                    if (week == null)
                    {
                        result.Errors.Add(new ImportRowErrorDto
                        {
                            RowNumber = parsedRows.IndexOf(row) + 1,
                            Reason = $"Semaine {row.WeekNumber} introuvable pour la phase du stagiaire {row.TraineeFullName}"
                        });
                        continue;
                    }

                    var existing = await _followUpRepository
                        .GetByTraineeAndWeekAsync(traineeId.Value, week.Id);
                    if (existing != null)
                    {
                        result.Errors.Add(new ImportRowErrorDto
                        {
                            RowNumber = parsedRows.IndexOf(row) + 1,
                            Reason = $"Suivi déjà existant pour {row.TraineeFullName}, semaine {row.WeekNumber}"
                        });
                        continue;
                    }

                    var weeklyFollowUp = new WeeklyFollowUp(
                        followUpDate: row.FollowUpDate,
                        comment: row.Comment,
                        traineeId: traineeId.Value,
                        mentorId: mentorId.Value,
                        weekId: week.Id,
                        courseName: row.CourseName,
                        appreciation: row.Appreciation);

                    if (!string.IsNullOrWhiteSpace(row.RawStatus))
                    {
                        if (row.RawStatus.Contains("Validated", StringComparison.OrdinalIgnoreCase))
                        {
                            weeklyFollowUp.Validated();
                        }
                        else if (row.RawStatus.Contains("Suspended", StringComparison.OrdinalIgnoreCase))
                        {
                            weeklyFollowUp.Suspended();
                        }
                    }

                    validFollowUps.Add(weeklyFollowUp);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new ImportRowErrorDto
                    {
                        RowNumber = parsedRows.IndexOf(row) + 1,
                        Reason = $"Erreur ligne {row.TraineeFullName}, semaine {row.WeekNumber} : {ex.Message}"
                    });
                }
            }

            // Une seule transaction pour toutes les lignes valides,
            // plutôt qu'un AddAsync par ligne.
            if (validFollowUps.Count > 0)
            {
                await _followUpRepository.AddRangeAsync(validFollowUps);
            }

            result.ErrorCount = result.Errors.Count;
            return result;
        }
    }
}
