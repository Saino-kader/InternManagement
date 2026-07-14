using InterManagement.Application.Features.Admins.DTOs;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Admins.Queries.GetAdminById
{
    public class GetAdminByIdHandler
    {
        private readonly IAdminRepository _repository;

        public GetAdminByIdHandler(IAdminRepository repository)
        {
            _repository = repository;
        }

        public async Task<AdminDto> Handle(GetAdminByIdQuery query)
        {
            var admin = await _repository.GetByIdAsync(query.Id);
            if (admin == null)
                throw new AdminNotFoundException(query.Id);

            return new AdminDto
            {
                Id        = admin.Id,
                FirstName = admin.FirstName,
                LastName  = admin.LastName,
                Email     = admin.Email,
                IsActive  = admin.IsActive
            };
        }
    }
}
