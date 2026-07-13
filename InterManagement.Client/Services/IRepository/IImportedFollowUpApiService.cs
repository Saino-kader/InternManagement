// Services/IRepository/IImportedFollowUpApiService.cs
using InterManagement.Application.Features.ImportedFollowUps.DTOs;

namespace InterManagement.Client.Services;

public interface IImportedFollowUpApiService
{
    Task<List<ImportedFollowUpDto>> GetAllAsync();
    Task<ImportedFollowUpResultDto?> ImportAsync(Stream fileStream, string fileName);
    Task<ImportedFollowUpDto?> UpdateAsync(int id, UpdateImportedFollowUpDto dto);
    Task<bool> DeleteAsync(int id);
}
