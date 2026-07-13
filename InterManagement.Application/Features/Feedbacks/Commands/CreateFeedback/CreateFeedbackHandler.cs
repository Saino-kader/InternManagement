// Application/Features/Feedbacks/Commands/CreateFeedback/CreateFeedbackHandler.cs
using InterManagement.Application.Features.Feedbacks.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Feedbacks.Commands.CreateFeedback
{
    public class CreateFeedbackHandler
    {
        private readonly IFeedbackRepository _repository;
        private readonly ITraineeRepository  _traineeRepository;
        private readonly IMentorRepository   _mentorRepository;

        public CreateFeedbackHandler(
            IFeedbackRepository feedbackRepository,
            ITraineeRepository  traineeRepository,
            IMentorRepository   mentorRepository)
        {
            _repository        = feedbackRepository;
            _traineeRepository = traineeRepository;
            _mentorRepository  = mentorRepository;
        }

        public async Task<FeedbackDto> Handle(CreateFeedbackCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Data.Message))
                throw new DomainException("Le message est obligatoire");

            if (command.Data.TraineeId == null && command.Data.MentorId == null)
                throw new DomainException(
                    "TraineeId ou MentorId est obligatoire");

            // Vérifie le Trainee si fourni
            string traineeName = string.Empty;
            if (command.Data.TraineeId.HasValue)
            {
                var trainee = await _traineeRepository
                    .GetByIdAsync(command.Data.TraineeId.Value);
                if (trainee == null)
                    throw new TraineeNotFoundException(command.Data.TraineeId.Value);
                traineeName = $"{trainee.FirstName} {trainee.LastName}";
            }

            // Vérifie le Mentor si fourni
            string mentorName = string.Empty;
            if (command.Data.MentorId.HasValue)
            {
                var mentor = await _mentorRepository
                    .GetByIdAsync(command.Data.MentorId.Value);
                if (mentor == null)
                    throw new MentorNotFoundException(command.Data.MentorId.Value);
                mentorName = $"{mentor.FirstName} {mentor.LastName}";
            }

            // Crée le Feedback avec les IDs nullables
            var feedback = new Feedback(
                command.Data.Message,
                command.Data.TraineeId,
                command.Data.MentorId
            );

            await _repository.AddAsync(feedback);

            // Détermine qui est l'émetteur
            // PRIORITÉ : TraineeId (stagiaire qui envoie) > MentorId (mentor qui envoie)
            var senderType = command.Data.TraineeId.HasValue ? "Stagiaire" : "Mentor";
            var senderName = command.Data.TraineeId.HasValue ? traineeName : mentorName;

            return new FeedbackDto
            {
                Id          = feedback.Id,
                Message     = feedback.Message,
                SentAt      = feedback.SentAt,
                SenderType  = senderType,
                SenderName  = senderName,
                TraineeId   = feedback.TraineeId,
                TraineeName = traineeName,
                MentorId    = feedback.MentorId,
                MentorName  = mentorName
            };
        }
    }
}