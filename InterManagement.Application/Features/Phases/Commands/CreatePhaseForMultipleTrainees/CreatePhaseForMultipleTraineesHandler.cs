using InterManagement.Domain.Entities;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Phases.Commands.CreatePhaseForMultipleTrainees
{
    public class CreatePhaseForMultipleTraineesHandler
    {
        private readonly IPhaseRepository _phaseRepository;
        private readonly IWeekRepository _weekRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly ITraineeRepository _traineeRepository;
        private readonly IMentorRepository _mentorRepository;

        public CreatePhaseForMultipleTraineesHandler(
            IPhaseRepository phaseRepository,
            IWeekRepository weekRepository,
            IAssignmentRepository assignmentRepository,
            ITraineeRepository traineeRepository,
            IMentorRepository mentorRepository)
        {
            _phaseRepository = phaseRepository;
            _weekRepository = weekRepository;
            _assignmentRepository = assignmentRepository;
            _traineeRepository = traineeRepository;
            _mentorRepository = mentorRepository;
        }

        public async Task<List<int>> Handle(CreatePhaseForMultipleTraineesCommand command)
        {
            var data = command.Data;

            var mentor = await _mentorRepository.GetByIdAsync(data.MentorId);
            if (mentor == null)
                throw new MentorNotFoundException(data.MentorId);

            var createdPhaseIds = new List<int>();

            foreach (var traineeId in data.TraineeIds)
            {
                var trainee = await _traineeRepository.GetByIdAsync(traineeId);
                if (trainee == null)
                    throw new TraineeNotFoundException(traineeId);

                var phase = new Phase(
                    data.PhaseNumber,
                    data.Title,
                    data.StartDate,
                    data.EndDate,
                    traineeId
                );
                await _phaseRepository.AddAsync(phase);

                foreach (var weekPlan in data.Weeks)
                {
                    var week = new Week(
                        weekPlan.WeekNumber,
                        weekPlan.Course,
                        weekPlan.StartDate,
                        weekPlan.EndDate,
                        phase.Id
                    );
                    await _weekRepository.AddAsync(week);
                }

                var assignment = new Assignment(traineeId, data.MentorId, phase.Id);
                await _assignmentRepository.AddAsync(assignment);

                createdPhaseIds.Add(phase.Id);
            }

            return createdPhaseIds;
        }
    }
}
