namespace InterManagement.Application.Features.Trainees.Queries.GetTraineeById
{
    public class GetTraineeByIdQuery
    {
        public int Id { get; set; }  
        
        public GetTraineeByIdQuery(int id)
        {
            Id = id;
        }
    }
}
