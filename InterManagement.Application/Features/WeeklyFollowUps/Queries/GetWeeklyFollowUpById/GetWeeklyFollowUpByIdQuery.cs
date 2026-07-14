namespace InterManagement.Application.Features.WeeklyFollowUps.Queries.GetWeeklyFollowUpById
{
    public class GetWeeklyFollowUpByIdQuery
    {
        public int Id { get; set; }

        public GetWeeklyFollowUpByIdQuery(int id)
        {
            Id = id;
        }
    }
}
