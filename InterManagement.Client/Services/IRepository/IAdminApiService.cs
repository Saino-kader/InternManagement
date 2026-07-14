using InterManagement.Application.Features.Admins.DTOs;

namespace InterManagement.Client.Services;

public interface IAdminApiService
    : IBaseApiService<AdminDto, CreateAdminDto, UpdateAdminDto>
{
    // Pas de méthode spéciale — Admin n'a que le CRUD de base
}
