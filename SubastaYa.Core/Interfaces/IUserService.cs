using SubastaYa.Core.Entities;
using System.Threading.Tasks;

namespace SubastaYa.Core.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterAsync(User user, string password);
        Task<string> LoginAsync(string email, string password);
    }
}
