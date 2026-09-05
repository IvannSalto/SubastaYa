using SubastaYa.Core.Entities;
using SubastaYa.Core.IRepositories;
using System.Threading.Tasks;

namespace SubastaYa.Core.IRepositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
    }
}