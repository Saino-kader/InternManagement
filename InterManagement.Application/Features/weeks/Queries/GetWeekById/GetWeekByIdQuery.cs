namespace InterManagement.Application.Features.Weeks.Queries.GetWeekById
{
    public class GetWeekByIdQuery
    {
        public int Id { get; set; }

        public GetWeekByIdQuery(int id)
        {
            Id = id;
        }
    }
}