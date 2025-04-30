using Microsoft.EntityFrameworkCore;
using FreelanceBirga.Core.Interfaces;

namespace FreelanceBirga.Core.Services
{
    public class RoleCheckerService : IRoleChecker
    {
        private readonly AppDbContext _context;

        public RoleCheckerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UserHasChatAccessAsync(int userId)
        {
            return await _context.Executors.AnyAsync(e => e.UserID == userId) ||
                   await _context.Customers.AnyAsync(c => c.UserID == userId);
        }
    }
}