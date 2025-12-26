using eOdsustva.SoftverskoInzenjerstvo.Common;
using eOdsustva.SoftverskoInzenjerstvo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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

        // GET: LeaveAllocations
        public async Task<IActionResult> Index()
        {
            var query = _context.LeaveAllocations
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Period)
                .AsQueryable();

            // Ako korisnik NIJE Administrator → vidi samo svoje
            if (!User.IsInRole(Roles.Administrator))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                query = query.Where(x => x.EmployeeId == userId);
            }

            return View(await query.ToListAsync());
        }

        // GET: LeaveAllocations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var leaveAllocation = await _context.LeaveAllocations
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Period)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (leaveAllocation == null) return NotFound();

            return View(leaveAllocation);
        }

        // Helper: popuni dropdown-ove (da ne dupliraš kod svuda)
        private void PopulateDropDowns(LeaveAllocation? model = null)
        {
            ViewData["EmployeeId"] = new SelectList(
                _context.Users.AsNoTracking(),
                "Id",
                "Email", // ako nema Email, stavi "UserName"
                model?.EmployeeId
            );

            ViewData["LeaveTypeId"] = new SelectList(
                _context.LeaveTypes.AsNoTracking(),
                "Id",
                "Name",
                model?.LeaveTypeId
            );

            ViewData["PeriodId"] = new SelectList(
                _context.Periods.AsNoTracking(),
                "Id",
                "Name", // ako nema Name nego Year -> zameni sa "Year"
                model?.PeriodId
            );
        }

        // GET: LeaveAllocations/Create
        public IActionResult Create()
        {
            PopulateDropDowns();
            return View();
        }

        // POST: LeaveAllocations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LeaveTypeId,EmployeeId,PeriodId,Days,Id")] LeaveAllocation leaveAllocation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(leaveAllocation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateDropDowns(leaveAllocation);
            return View(leaveAllocation);
        }

        // GET: LeaveAllocations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var leaveAllocation = await _context.LeaveAllocations.FindAsync(id);
            if (leaveAllocation == null) return NotFound();

            PopulateDropDowns(leaveAllocation);
            return View(leaveAllocation);
        }

        // POST: LeaveAllocations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LeaveTypeId,EmployeeId,PeriodId,Days,Id")] LeaveAllocation leaveAllocation)
        {
            if (id != leaveAllocation.Id) return NotFound();

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

            PopulateDropDowns(leaveAllocation);
            return View(leaveAllocation);
        }

        // GET: LeaveAllocations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var leaveAllocation = await _context.LeaveAllocations
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Period)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (leaveAllocation == null) return NotFound();

            return View(leaveAllocation);
        }

        // POST: LeaveAllocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leaveAllocation = await _context.LeaveAllocations.FindAsync(id);
            if (leaveAllocation != null)
            {
                _context.LeaveAllocations.Remove(leaveAllocation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeaveAllocationExists(int id)
        {
            return _context.LeaveAllocations.Any(e => e.Id == id);
        }
    }
}
