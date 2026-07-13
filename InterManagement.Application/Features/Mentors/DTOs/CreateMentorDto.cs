namespace InterManagement.Application.Features.Mentors.DTOs
{
    public class CreateMentorDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
