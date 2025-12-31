using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eOdsustva.SoftverskoInzenjerstvo.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 1. SEED ROLES (Tvoj postojeći kod)
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "42715495-8f6e-4625-89b3-8a1f76f7e274", Name = "Employee", NormalizedName = "EMPLOYEE", ConcurrencyStamp = "ROLE_EMPLOYEE_001" },
                new IdentityRole { Id = "0bff17a4-8768-48d7-a9ad-962a8ad7b7a0", Name = "Supervisor", NormalizedName = "SUPERVISOR", ConcurrencyStamp = "ROLE_SUPERVISOR_001" },
                new IdentityRole { Id = "b748e02c-457d-4326-ac2f-6d2868ed9337", Name = "Administrator", NormalizedName = "ADMINISTRATOR", ConcurrencyStamp = "ROLE_ADMIN_001" }
            );

            // 2. SEED DEPARTMENTS (Dodaj ovo ovde!)
            // Koristimo fiksne ID-eve da bi znali tačno koji je koji sektor
            builder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "IT" },
                new Department { Id = 2, Name = "HR" },
                new Department { Id = 3, Name = "Proizvodnja" },
                new Department { Id = 4, Name = "Finansije" },
                new Department { Id = 5, Name = "Marketing" },
                new Department { Id = 6, Name = "Logistika" }
            );
        }

        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveAllocation> LeaveAllocations { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<Department> Departments { get; set; }
    }
}