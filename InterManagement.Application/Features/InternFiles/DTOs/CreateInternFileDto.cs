namespace InterManagement.Application.Features.InternFiles.DTOs
{
    public class CreateInternFileDto
    {
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public int TraineeId { get; set; }
    }
}