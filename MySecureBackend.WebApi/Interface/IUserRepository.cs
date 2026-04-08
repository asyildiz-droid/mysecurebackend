using MySecureBackend.WebApi.Models; 

namespace MySecureBackend.WebApi.Interface
{
    public interface IUserRepository 
    {
        Task<User?> GetByUserName(string username);
        Task Create(User user);
    }
}
