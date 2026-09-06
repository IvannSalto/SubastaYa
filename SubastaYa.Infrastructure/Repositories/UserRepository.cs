using Microsoft.EntityFrameworkCore;
using SubastaYa.Core.Entities;
using SubastaYa.Core.IRepositories;
using SubastaYa.Infrastructure.Data;
using System.Threading.Tasks;

namespace SubastaYa.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}