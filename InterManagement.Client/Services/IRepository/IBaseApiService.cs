namespace InterManagement.Client.Services;

public interface IBaseApiService<TDto, TCreateDto, TUpdateDto>
{
    Task<List<TDto>> GetAllAsync();
    Task<TDto?> GetByIdAsync(int id);
    Task<TDto?> CreateAsync(TCreateDto dto);
    Task<TDto?> UpdateAsync(int id, TUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
