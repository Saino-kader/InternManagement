using InterManagement.Application.Features.WeeklyFollowUps.DTOs;

namespace InterManagement.Application.Features.WeeklyFollowUps.Commands.ImportWeeklyFollowUps
{
    public class ImportWeeklyFollowUpsCommand
    {
        public Stream FileStream { get; set; }
        public string FileExtension { get; set; }

        public ImportWeeklyFollowUpsCommand(Stream fileStream, string fileExtension)
        {
            FileStream = fileStream;
            FileExtension = fileExtension;
        }
    }

    public class ParsedFollowUpRow
    {
        public string TraineeFullName { get; set; } = string.Empty;
        public string MentorFullName { get; set; } = string.Empty;
        public DateOnly FollowUpDate { get; set; }
        public int WeekNumber { get; set; }
        public int WeekId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string Appreciation { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string RawStatus { get; set; } = string.Empty;
    }

}
