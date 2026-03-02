using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Interface
{
    public interface IEnvironment2DRepository
    {
        Task InsertAsync(Environment2D environment2D);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Environment2D>> SelectAsync();
        Task<Environment2D?> SelectAsync(Guid id);
        Task UpdateAsync(Environment2D environment2D);
    }
}