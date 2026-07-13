namespace InterManagement.Application.Features.WeeklyFollowUps.Queries.ExportWeeklyFollowUpsByTrainee
{
    public class ExportWeeklyFollowUpsByTraineeQuery
    {
        public int TraineeId { get; set; }

        public ExportWeeklyFollowUpsByTraineeQuery(int traineeId)
        {
            TraineeId = traineeId;
        }
    }
}
