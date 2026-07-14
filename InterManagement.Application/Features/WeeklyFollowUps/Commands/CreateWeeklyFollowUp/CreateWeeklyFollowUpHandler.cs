using InterManagement.Application.Features.WeeklyFollowUps.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;


namespace InterManagement.Application.Features.WeeklyFollowUps.Commands.CreateWeeklyFollowUp
{
    public class CreateWeeklyFollowUpHandler
    {
        private readonly IWeeklyFollowUpRepository _repository;
        private readonly ITraineeRepository _traineeRepository;
        private readonly IMentorRepository _mentorRepository;
        private readonly IWeekRepository _weekRepository;

        public CreateWeeklyFollowUpHandler(
            IWeeklyFollowUpRepository repository,
            ITraineeRepository traineeRepository,
            IMentorRepository mentorRepository,
            IWeekRepository weekRepository)
        {
            _repository = repository;
            _traineeRepository = traineeRepository;
            _mentorRepository = mentorRepository;
            _weekRepository = weekRepository;
        }

        public async Task<WeeklyFollowUpDto> Handle(
            CreateWeeklyFollowUpCommand command)
        {
            var trainee = await _traineeRepository.GetByIdAsync(command.Data.TraineeId);
            if (trainee == null)
                throw new TraineeNotFoundException(command.Data.TraineeId);

            var mentor = await _mentorRepository.GetByIdAsync(command.Data.MentorId);
            if (mentor == null)
                throw new MentorNotFoundException(command.Data.MentorId);

            var week = await _weekRepository.GetByIdAsync(command.Data.WeekId);
            if (week == null)
                throw new DomainException("Semaine introuvable");

            var exists = await _repository
                .GetByTraineeAndWeekAsync(
                    command.Data.TraineeId,
                    command.Data.WeekId);

            if (exists != null)
                throw new WeeklyFollowUpAlreadyExistsException(
                    command.Data.TraineeId,
                    command.Data.WeekId);

            var followUp = new WeeklyFollowUp(
                command.Data.FollowUpDate,
                command.Data.Comment,
                command.Data.TraineeId,
                command.Data.MentorId,
                command.Data.WeekId,
                command.Data.CourseName,
                command.Data.Appreciation
            );

            await _repository.AddAsync(followUp);

            return new WeeklyFollowUpDto
            {
                Id           = followUp.Id,
                FollowUpDate = followUp.FollowUpDate,
                Status       = followUp.Status,
                Comment      = followUp.Comment,
                CourseName   = followUp.CourseName,
                Appreciation = followUp.Appreciation,
                WeekNumber   = week.WeekNumber,
                WeekId       = followUp.WeekId,
                TraineeId    = followUp.TraineeId,
                TraineeName  = $"{trainee.FirstName} {trainee.LastName}",
                MentorId     = followUp.MentorId,
                MentorName   = $"{mentor.FirstName} {mentor.LastName}"
            };
        }
    }
}
