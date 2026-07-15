// Application/Features/Feedbacks/Queries/GetFeedbackById/GetFeedbackByIdHandler.cs
using InterManagement.Application.Features.Feedbacks.DTOs;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Feedbacks.Queries.GetFeedbackById
{
    public class GetFeedbackByIdHandler
    {
        private readonly IFeedbackRepository _repository;

        public GetFeedbackByIdHandler(IFeedbackRepository repository)
        {
            _repository = repository;
        }

        public async Task<FeedbackDto> Handle(GetFeedbackByIdQuery query)
        {
            var feedback = await _repository.GetByIdAsync(query.Id);
            if (feedback == null)
                throw new FeedbackNotFoundException(query.Id);

            // Détermine le type et le nom de l'émetteur
            // PRIORITÉ : TraineeId (stagiaire qui envoie) > MentorId (mentor qui envoie)
            string senderType;
            string senderName;

            if (feedback.TraineeId.HasValue && feedback.Trainee != null)
            {
                senderType = "Stagiaire";
                senderName = $"{feedback.Trainee.FirstName} {feedback.Trainee.LastName}";
            }
            else if (feedback.MentorId.HasValue && feedback.Mentor != null)
            {
                senderType = "Mentor";
                senderName = $"{feedback.Mentor.FirstName} {feedback.Mentor.LastName}";
            }
            else
            {
                senderType = "Inconnu";
                senderName = "Utilisateur inconnu";
            }

            return new FeedbackDto
            {
                Id          = feedback.Id,
                Message     = FeedbackMessageCleaner.Clean(feedback.Message),
                SentAt      = feedback.SentAt,
                SenderType  = senderType,
                SenderName  = senderName,
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
