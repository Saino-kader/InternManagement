using InterManagement.Application.Features.Admins.DTOs;

namespace InterManagement.Client.Services;

public class AdminApiService
    : BaseApiService<AdminDto, CreateAdminDto, UpdateAdminDto>,
      IAdminApiService
{
    protected override string ResourcePath => "admin";

    public AdminApiService(HttpClient httpClient, ILogger<AdminApiService> logger)
        : base(httpClient, logger) { }

    // Rien à ajouter — le CRUD générique suffit
}