namespace InterManagement.Application.Features.Assignments.Queries.GetMentorAssignments
{
    public class GetMentorAssignmentsQuery
    {
        public int MentorId { get; set; }

        public GetMentorAssignmentsQuery(int mentorId)
        {
            MentorId = mentorId;
        }
    }
}
