using eOdsustva.SoftverskoInzenjerstvo.Common;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace eOdsustva.SoftverskoInzenjerstvo.Services
{
    public class UserScopeService
    {
        private readonly ApplicationDbContext _context;

        public UserScopeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int?> GetMyDepartmentIdAsync(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => u.DepartmentId)
                .FirstOrDefaultAsync();
        }

        public IQueryable<ApplicationUser> FilterUsersVisibleTo(ClaimsPrincipal user, IQueryable<ApplicationUser> q, int? myDeptId)
        {
            if (user.IsInRole("Administrator"))
                return q;

            if (user.IsInRole(Roles.Supervisor))
                return q.Where(u => u.DepartmentId == myDeptId);

            var myId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return q.Where(u => u.Id == myId);
        }
    }
}
