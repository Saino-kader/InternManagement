// Application/Features/Feedbacks/Queries/GetFeedbacks/GetFeedbacksQuery.cs
namespace InterManagement.Application.Features.Feedbacks.Queries.GetFeedbacks
{
    public class GetFeedbacksQuery
    {
        public int? TraineeId { get; set; }
        public int? MentorId { get; set; }  // ← AJOUTÉ
        public int? Count { get; set; }
    }
}