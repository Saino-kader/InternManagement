using InterManagement.Application.Features.Feedbacks.DTOs;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Feedbacks.Commands.UpdateFeedback
{
    public class UpdateFeedbackHandler
    {
        private readonly IFeedbackRepository _repository;

        public UpdateFeedbackHandler(IFeedbackRepository repository)
        {
            _repository = repository;
        }

        public async Task<FeedbackDto> Handle(
            UpdateFeedbackCommand command)
        {
            // 1. Chercher le feedback
            var feedback = await _repository
                .GetByIdAsync(command.Id);
            if (feedback == null)
                throw new FeedbackNotFoundException(command.Id);

            // 2. Modifier via méthode métier
            feedback.UpdateMessage(command.Data.Message);

            // 3. Sauvegarder
            await _repository.UpdateAsync(feedback);

            // 4. Retourner DTO
            return new FeedbackDto
            {
                Id          = feedback.Id,
                Message     = feedback.Message,
                SentAt      = feedback.SentAt,
                TraineeId   = feedback.TraineeId,
                TraineeName = feedback.Trainee != null
                    ? $"{feedback.Trainee.FirstName} {feedback.Trainee.LastName}"
                    : string.Empty,
                MentorId    = feedback.MentorId,
                MentorName  = feedback.Mentor != null
                    ? $"{feedback.Mentor.FirstName} {feedback.Mentor.LastName}"
                    : string.Empty
            };
        }
    }
}
