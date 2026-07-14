namespace InterManagement.Application.Features.WeeklyFollowUps.DTOs
{
        public class ImportResultDto
    {
        public int TotalRowsRead { get; set; }
        public int SuccessCount { get; set; }   
        public int ErrorCount { get; set; }      
        public List<ImportRowErrorDto> Errors { get; set; } = [];
    }

    public class ImportRowErrorDto
    {
        public int RowNumber { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
