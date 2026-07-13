// Application/Features/Assignments/Queries/GetAssignmentsByMentor/GetAssignmentsByMentorQuery.cs
namespace InterManagement.Application.Features.Assignments.Queries.GetAssignmentsByMentor
{
    public class GetAssignmentsByMentorQuery
    {
        public int MentorId { get; set; }

        public GetAssignmentsByMentorQuery(int mentorId)
        {
            MentorId = mentorId;
        }
    }
}