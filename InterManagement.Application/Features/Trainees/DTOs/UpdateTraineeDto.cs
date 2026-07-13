using InterManagement.Domain.Entities;
using InterManagement.Shared.Enums;

namespace InterManagement.Application.Features.Trainees.DTOs;

public class UpdateTraineeDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    public string University { get; set; } = string.Empty;
    
    public string Specialty { get; set; } = string.Empty;
    public string Theme { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }                      
    public DateOnly EndDate { get; set; }
    public TraineeStatus Status { get; set; }
    public bool IsActive { get; set; } = true;
}
