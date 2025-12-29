using eOdsustva.SoftverskoInzenjerstvo.Common;
using eOdsustva.SoftverskoInzenjerstvo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace eOdsustva.SoftverskoInzenjerstvo.Controllers
{
    [Authorize]
    public class LeaveAllocationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaveAllocationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // HELPERS
        // =========================
        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        private async Task<int?> GetMyDepartmentIdAsync()
        {
            var userId = GetUserId();
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => u.DepartmentId)
                .FirstOrDefaultAsync();
        }

        private IQueryable<LeaveAllocation> ApplyScope(IQueryable<LeaveAllocation> query, int? myDeptId)
        {
            // Admin vidi sve
            if (User.IsInRole(Roles.Administrator))
                return query;

            // Supervisor vidi sve iz svog odeljenja
            if (User.IsInRole(Roles.Supervisor))
                return query.Where(x => x.Employee.DepartmentId == myDeptId);

            // Employee vidi samo svoje
            var userId = GetUserId();
            return query.Where(x => x.EmployeeId == userId);
        }

        private async Task PopulateDropDownsAsync(LeaveAllocation? model = null)
        {
            int? myDeptId = null;
            if (User.IsInRole(Roles.Supervisor) || !User.IsInRole(Roles.Administrator))
                myDeptId = await GetMyDepartmentIdAsync();

            // Employees dropdown: Admin = svi; Supervisor = samo iz svog odeljenja
            var usersQuery = _context.Users.AsNoTracking();

            if (User.IsInRole(Roles.Supervisor))
            {
                usersQuery = usersQuery.Where(u => u.DepartmentId == myDeptId);
            }
            else if (!User.IsInRole(Roles.Administrator))
            {
                // Ako običan employee ikad dođe do Create/Edit (ne bi trebalo), ograniči na sebe
                var userId = GetUserId();
                usersQuery = usersQuery.Where(u => u.Id == userId);
            }

            ViewData["EmployeeId"] = new SelectList(
                await usersQuery
                    .Select(u => new
                    {
                        u.Id,
                        FullName = u.FirstName + " " + u.LastName
                    })
                    .OrderBy(u => u.FullName)
                    .ToListAsync(),
                "Id",
                "FullName",
                model?.EmployeeId
            );

            ViewData["LeaveTypeId"] = new SelectList(
                await _context.LeaveTypes.AsNoTracking().OrderBy(x => x.Name).ToListAsync(),
                "Id",
                "Name",
                model?.LeaveTypeId
            );

            ViewData["PeriodId"] = new SelectList(
                await _context.Periods.AsNoTracking().OrderByDescending(p => p.Id).ToListAsync(),
                "Id",
                "Name", // ako je Year -> "Year"
                model?.PeriodId
            );
        }

        // =========================
        // GET: LeaveAllocations
        // =========================
        public async Task<IActionResult> Index()
        {
            var query = _context.LeaveAllocations
                .Include(l => l.Employee).ThenInclude(e => e.Department)
                .Include(l => l.LeaveType)
                .Include(l => l.Period)
                .AsQueryable();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1) Admin vidi sve - ništa ne filtriramo
            if (User.IsInRole(Roles.Administrator))
            {
                return View(await query.ToListAsync());
            }

            // 2) Supervisor vidi samo svoje odeljenje
            if (User.IsInRole(Roles.Supervisor))
            {
                var myDeptId = await _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => u.DepartmentId)
                    .FirstOrDefaultAsync();

                query = query.Where(x => x.Employee.DepartmentId == myDeptId);
                return View(await query.ToListAsync());
            }

            // 3) Employee vidi samo svoje (FK filter)
            query = query.Where(x => x.EmployeeId == userId);

            return View(await query.ToListAsync());
        }


        // =========================
        // GET: LeaveAllocations/Details/5
        // =========================
        public async Task<IActionResult> Details(int? id)
        {
            var query = _context.LeaveAllocations
                 .Include(l => l.Employee).ThenInclude(e => e.Department)
                 .Include(l => l.LeaveType)
                 .Include(l => l.Period)
                 .AsQueryable();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Admin - sve
            if (!User.IsInRole(Roles.Administrator))
            {
                if (User.IsInRole(Roles.Supervisor))
                {
                    var myDeptId = await _context.Users
                        .Where(u => u.Id == userId)
                        .Select(u => u.DepartmentId)
                        .FirstOrDefaultAsync();

                    query = query.Where(x => x.Employee.DepartmentId == myDeptId);
                }
                else
                {
                    query = query.Where(x => x.EmployeeId == userId);
                }
            }

            var leaveAllocation = await query.FirstOrDefaultAsync(m => m.Id == id);
            if (leaveAllocation == null) return NotFound();
            return View(leaveAllocation);

        }

        // =========================
        // CREATE (Admin or Supervisor)
        // =========================
        [Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
        public async Task<IActionResult> Create()
        {
            await PopulateDropDownsAsync();
            return View();
        }

        [Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LeaveTypeId,EmployeeId,PeriodId,Days,Id")] LeaveAllocation leaveAllocation)
        {
            // Supervisor ne sme da dodeli alokaciju zaposlenom iz drugog odeljenja
            if (User.IsInRole(Roles.Supervisor))
            {
                var myDeptId = await GetMyDepartmentIdAsync();
                var employeeDeptId = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.Id == leaveAllocation.EmployeeId)
                    .Select(u => u.DepartmentId)
                    .FirstOrDefaultAsync();

                if (employeeDeptId != myDeptId)
                {
                    return Forbid();
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(leaveAllocation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropDownsAsync(leaveAllocation);
            return View(leaveAllocation);
        }

        // =========================
        // EDIT (Admin or Supervisor)
        // =========================
        [Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // prvo učitaj sa Employee da možemo scope check
            var baseQuery = _context.LeaveAllocations
                .Include(x => x.Employee)
                .AsQueryable();

            var myDeptId = await GetMyDepartmentIdAsync();
            var scopedQuery = ApplyScope(baseQuery, myDeptId);

            var leaveAllocation = await scopedQuery.FirstOrDefaultAsync(x => x.Id == id);
            if (leaveAllocation == null) return NotFound(); // ili Forbid()

            await PopulateDropDownsAsync(leaveAllocation);
            return View(leaveAllocation);
        }

        [Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LeaveTypeId,EmployeeId,PeriodId,Days,Id")] LeaveAllocation leaveAllocation)
        {
            if (id != leaveAllocation.Id) return NotFound();

            // scope check (da ne može da edituje tuđe preko POST)
            var baseQuery = _context.LeaveAllocations
                .Include(x => x.Employee)
                .AsQueryable();

            var myDeptId = await GetMyDepartmentIdAsync();
            var scopedQuery = ApplyScope(baseQuery, myDeptId);

            var existing = await scopedQuery.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null) return Forbid();

            // Supervisor ne sme da promeni EmployeeId na drugu firmu/odeljenje
            if (User.IsInRole(Roles.Supervisor))
            {
                var employeeDeptId = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.Id == leaveAllocation.EmployeeId)
                    .Select(u => u.DepartmentId)
                    .FirstOrDefaultAsync();

                if (employeeDeptId != myDeptId)
                    return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leaveAllocation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveAllocationExists(leaveAllocation.Id))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await PopulateDropDownsAsync(leaveAllocation);
            return View(leaveAllocation);
        }

        // =========================
        // DELETE (Admin or Supervisor)
        // =========================
        [Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var baseQuery = _context.LeaveAllocations
                .Include(l => l.Employee).ThenInclude(e => e.Department)
                .Include(l => l.LeaveType)
                .Include(l => l.Period)
                .AsQueryable();

            var myDeptId = await GetMyDepartmentIdAsync();
            var query = ApplyScope(baseQuery, myDeptId);

            var leaveAllocation = await query.FirstOrDefaultAsync(m => m.Id == id);
            if (leaveAllocation == null) return NotFound(); // ili Forbid()

            return View(leaveAllocation);
        }

        [Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // scope check (POST)
            var baseQuery = _context.LeaveAllocations
                .Include(x => x.Employee)
                .AsQueryable();

            var myDeptId = await GetMyDepartmentIdAsync();
            var scopedQuery = ApplyScope(baseQuery, myDeptId);

            var leaveAllocation = await scopedQuery.FirstOrDefaultAsync(x => x.Id == id);
            if (leaveAllocation == null) return Forbid();

            _context.LeaveAllocations.Remove(leaveAllocation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool LeaveAllocationExists(int id)
        {
            return _context.LeaveAllocations.Any(e => e.Id == id);
        }
    }
}
