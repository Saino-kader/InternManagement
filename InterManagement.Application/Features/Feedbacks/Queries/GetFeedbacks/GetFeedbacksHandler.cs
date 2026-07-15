// Application/Features/Feedbacks/Queries/GetFeedbacks/GetFeedbacksHandler.cs
using InterManagement.Application.Features.Feedbacks.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Feedbacks.Queries.GetFeedbacks
{
    public class GetFeedbacksHandler
    {
        private readonly IFeedbackRepository _repository;

        public GetFeedbacksHandler(IFeedbackRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<FeedbackDto>> Handle(GetFeedbacksQuery query)
        {
            IEnumerable<Feedback> feedbacks;

            if (query.TraineeId.HasValue && query.Count.HasValue)
                feedbacks = await _repository.GetRecentFeedbacksAsync(
                    query.TraineeId.Value, query.Count.Value);

            else if (query.TraineeId.HasValue)
                feedbacks = await _repository.GetByTraineeAsync(
                    query.TraineeId.Value);

            else if (query.MentorId.HasValue)
                feedbacks = await _repository.GetByMentorAsync(
                    query.MentorId.Value);

            else
                feedbacks = await _repository.GetAllAsync();

            return feedbacks.Select(BuildDto);
        }

        // ── Construit le DTO depuis l'entité ─────────────────────────
        private static FeedbackDto BuildDto(Feedback f)
        {
            // Détermine le type et le nom de l'émetteur
            // PRIORITÉ : TraineeId (stagiaire qui envoie) > MentorId (mentor qui envoie)
            string senderType;
            string senderName;

            if (f.TraineeId.HasValue && f.Trainee != null)
            {
                senderType = "Stagiaire";
                senderName = $"{f.Trainee.FirstName} {f.Trainee.LastName}";
            }
            else if (f.MentorId.HasValue && f.Mentor != null)
            {
                senderType = "Mentor";
                senderName = $"{f.Mentor.FirstName} {f.Mentor.LastName}";
            }
            else
            {
                senderType = "Inconnu";
                senderName = "Utilisateur inconnu";
            }

            return new FeedbackDto
            {
                Id          = f.Id,
                Message     = FeedbackMessageCleaner.Clean(f.Message),
                SentAt      = f.SentAt,
                SenderType  = senderType,
                SenderName  = senderName,
                TraineeId   = f.TraineeId,
                TraineeName = f.Trainee != null
                    ? $"{f.Trainee.FirstName} {f.Trainee.LastName}"
                    : string.Empty,
                MentorId    = f.MentorId,
                MentorName  = f.Mentor != null
                    ? $"{f.Mentor.FirstName} {f.Mentor.LastName}"
                    : string.Empty
            };
        }
    }
}