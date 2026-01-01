using AutoMapper;
using AutoMapper.QueryableExtensions;
using eOdsustva.SoftverskoInzenjerstvo.Common;
using eOdsustva.SoftverskoInzenjerstvo.Data;
using eOdsustva.SoftverskoInzenjerstvo.Models; // ako su VM ovde; prilagodi namespace!
using eOdsustva.SoftverskoInzenjerstvo.Models.LeaveAllocation;
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
        private readonly IMapper _mapper;

        public LeaveAllocationsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        private bool IsAdmin => User.IsInRole(Roles.Administrator);
        private bool IsSupervisor => User.IsInRole(Roles.Supervisor);

        private Task<int> GetMyDepartmentIdAsync()
            => _context.Users.AsNoTracking()
                .Where(u => u.Id == UserId)
                .Select(u => u.DepartmentId)
                .FirstOrDefaultAsync();

        private async Task<int?> GetDepartmenttIfNeededAsync()
            => IsAdmin ? null : await GetMyDepartmentIdAsync();

        private IQueryable<LeaveAllocation> BaseQuery(bool tracking = false)
        {
            var q = _context.LeaveAllocations
                .Include(l => l.Employee).ThenInclude(e => e.Department)
                .Include(l => l.LeaveType)
                .Include(l => l.Period)
                .AsQueryable();

            return tracking ? q : q.AsNoTracking();
        }

        private IQueryable<LeaveAllocation> ApplyScope(IQueryable<LeaveAllocation> query, int? myDeptId)
        {
            if (IsAdmin) return query;

            if (IsSupervisor)
                return query.Where(x => x.Employee.DepartmentId == myDeptId);

            return query.Where(x => x.EmployeeId == UserId);
        }

        private async Task EnsureSupervisorSameDepartmentOrForbidAsync(string employeeId)
        {
            if (!IsSupervisor) return;

            var myDeptId = await GetMyDepartmentIdAsync();

            var employeeDeptId = await _context.Users.AsNoTracking()
                .Where(u => u.Id == employeeId)
                .Select(u => u.DepartmentId)
                .FirstOrDefaultAsync();

            if (employeeDeptId != myDeptId)
                throw new UnauthorizedAccessException();
        }

        private async Task PopulateDropDownsAsync(
            string? selectedEmployeeId = null,
            int? selectedLeaveTypeId = null,
            int? selectedPeriodId = null)
        {
            int? myDeptId = await GetDepartmenttIfNeededAsync();

      
            var usersQuery = _context.Users.AsNoTracking();

            if (IsSupervisor)
                usersQuery = usersQuery.Where(u => u.DepartmentId == myDeptId);
            else if (!IsAdmin)
                usersQuery = usersQuery.Where(u => u.Id == UserId);

            var employees = await usersQuery
                .Select(u => new { u.Id, FullName = u.FirstName + " " + u.LastName })
                .OrderBy(x => x.FullName)
                .ToListAsync();

            ViewData["EmployeeId"] = new SelectList(employees, "Id", "FullName", selectedEmployeeId);

            // LeaveTypes dropdown
            var leaveTypes = await _context.LeaveTypes.AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();

            ViewData["LeaveTypeId"] = new SelectList(leaveTypes, "Id", "Name", selectedLeaveTypeId);

            // Periods dropdown
            var periods = await _context.Periods.AsNoTracking()
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            ViewData["PeriodId"] = new SelectList(periods, "Id", "Name", selectedPeriodId);
        }


        public async Task<IActionResult> Index()
        {
            var myDeptId = await GetDepartmenttIfNeededAsync();

            var data = await ApplyScope(BaseQuery(tracking: false), myDeptId)
                .ProjectTo<LeaveAllocationListVM>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return View(data);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();

            var myDeptId = await GetDepartmenttIfNeededAsync();

            var vm = await ApplyScope(BaseQuery(tracking: false), myDeptId)
                .Where(x => x.Id == id)
                .ProjectTo<LeaveAllocationDetailsVM>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (vm is null) return NotFound();

            return View(vm);
        }

        [Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
        public async Task<IActionResult> Create()
        {
            await PopulateDropDownsAsync();
            return View(new LeaveAllocationCreateVM());
        }

        [Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveAllocationCreateVM vm)
        {
            try
            {
                await EnsureSupervisorSameDepartmentOrForbidAsync(vm.EmployeeId);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropDownsAsync(vm.EmployeeId, vm.LeaveTypeId, vm.PeriodId);
                return View(vm);
            }

            var entity = _mapper.Map<LeaveAllocation>(vm);
            _context.LeaveAllocations.Add(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            var myDeptId = await GetDepartmenttIfNeededAsync();

            var entity = await ApplyScope(BaseQuery(tracking: true), myDeptId)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null) return NotFound();

            var vm = _mapper.Map<LeaveAllocationEditVM>(entity);

            await PopulateDropDownsAsync(vm.EmployeeId, vm.LeaveTypeId, vm.PeriodId);

            return View(vm);
        }

        [Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LeaveAllocationEditVM vm)
        {
            if (id != vm.Id) return NotFound();

            var myDeptId = await GetDepartmenttIfNeededAsync();

            var entity = await ApplyScope(BaseQuery(tracking: true), myDeptId)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null) return Forbid();

            try
            {
                await EnsureSupervisorSameDepartmentOrForbidAsync(vm.EmployeeId);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropDownsAsync(vm.EmployeeId, vm.LeaveTypeId, vm.PeriodId);
                return View(vm);
            }

            _mapper.Map(vm, entity);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var myDeptId = await GetDepartmenttIfNeededAsync();

            var vm = await ApplyScope(BaseQuery(tracking: false), myDeptId)
                .Where(x => x.Id == id)
                .ProjectTo<LeaveAllocationDeleteVM>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (vm is null) return NotFound();

            return View(vm);
        }

        [Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var myDeptId = await GetDepartmenttIfNeededAsync();

            var entity = await ApplyScope(
                    _context.LeaveAllocations.Include(x => x.Employee),
                    myDeptId
                )
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null) return Forbid();

            _context.LeaveAllocations.Remove(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
