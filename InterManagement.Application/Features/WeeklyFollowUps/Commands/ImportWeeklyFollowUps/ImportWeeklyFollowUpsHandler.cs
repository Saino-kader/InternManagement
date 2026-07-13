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

    // 📦 Collecter les entités valides dans une liste
    var validFollowUps = new List<WeeklyFollowUp>();

    // ✅ BOUCLE QUI COLLECTE SEULEMENT (pas de sauvegarde)
    foreach (var row in parsedRows)
    {
        try
        {
            // Trouver le TraineeId
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

            // Trouver le MentorId
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

            // Trouver l'assignment qui lie le stagiaire au mentor
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

            // Trouver la phase correspondante
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

            // Trouver la semaine correspondante avec le WeekNumber
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

            // Vérifier si le suivi existe déjà
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

            // Créer l'entité
            var weeklyFollowUp = new WeeklyFollowUp(
                followUpDate: row.FollowUpDate,
                comment: row.Comment,
                traineeId: traineeId.Value,
                mentorId: mentorId.Value,
                weekId: week.Id,
                courseName: row.CourseName,
                appreciation: row.Appreciation);

            // Appliquer le statut
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

            // 📦 Ajouter à la liste (pas encore en BD)
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

    // ✅ SAUVEGARDER TOUT D'UN COUP EN UNE SEULE TRANSACTION
    if (validFollowUps.Count > 0)
    {
        await _followUpRepository.AddRangeAsync(validFollowUps);
    }

    result.ErrorCount = result.Errors.Count;
    return result;
}


/*
        public async Task<ImportResultDto> Handle(ImportWeeklyFollowUpsCommand command)
        {
            var result = new ImportResultDto();

            // 1. Parser le fichier
            var parsedRows = await _fileParser.ParseAsync(
                command.FileStream, command.FileExtension);

            result.TotalRowsRead = parsedRows.Count;

            // 2. Pour chaque ligne parsée, créer le WeeklyFollowUp
            foreach (var row in parsedRows)
            {
                try
                {
                    // Trouver le TraineeId
                    var traineeId = await _followUpRepository
                        .FindTraineeIdByFullNameAsync(row.TraineeFullName);
                    if (traineeId == null)
                    {
                        result.Errors.Add(new ImportRowErrorDto
                        {
                            RowNumber = parsedRows.IndexOf(row) + 1,
                            Reason = $"Stagiaire introuvable : {row.TraineeFullName}"
                        });
                        continue; // Skip cette ligne
                    }

                    // Trouver le MentorId
                    var mentorId = await _followUpRepository
                        .FindMentorIdByFullNameAsync(row.MentorFullName);
                    if (mentorId == null)
                    {
                        result.Errors.Add(new ImportRowErrorDto
                        {
                            RowNumber = parsedRows.IndexOf(row) + 1,
                            Reason = $"Mentor introuvable : {row.MentorFullName}"
                        });
                        continue; // Skip cette ligne
                    }

                    // Vérifier si le suivi existe déjà (éviter les doublons)
                    var existing = await _followUpRepository
                        .GetByTraineeAndWeekAsync(traineeId.Value, row.WeekId);
                    if (existing != null)
                    {
                        result.Errors.Add(new ImportRowErrorDto
                        {
                            RowNumber = parsedRows.IndexOf(row) + 1,
                            Reason = $"Suivi déjà existant pour {row.TraineeFullName}, semaine {row.WeekNumber}"
                        });
                        continue; // Skip cette ligne
                    }

                    // Créer l'entité
                    var weeklyFollowUp = new WeeklyFollowUp(
                            followUpDate: row.FollowUpDate,
                            comment: row.Comment,
                            traineeId: traineeId.Value,
                            mentorId: mentorId.Value,
                            weekId: row.WeekId,
                        courseName: row.CourseName,
                        appreciation: row.Appreciation);

                    // Appliquer le statut s'il est spécifié
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

                    // Enregistrer en base de données
                    await _followUpRepository.AddAsync(weeklyFollowUp);
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

            return result;
        }*/
    }
}



