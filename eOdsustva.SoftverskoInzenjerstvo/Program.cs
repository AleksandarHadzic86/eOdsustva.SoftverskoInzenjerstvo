using AutoMapper;
using eOdsustva.SoftverskoInzenjerstvo.Data;
using eOdsustva.SoftverskoInzenjerstvo.MappingProfile;
using eOdsustva.SoftverskoInzenjerstvo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// AutoMapper (clean way)
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfile>());

// Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<UserScopeService>();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// ✅ Seed: Departments + Roles + Admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await context.Database.MigrateAsync(); 


    var departmentNames = new[]
    {
        "IT",
        "HR",
        "Proizvodnja",
        "Finansije",
        "Marketing",
        "Logistika"
    };

    foreach (var name in departmentNames)
    {
        if (!await context.Departments.AnyAsync(d => d.Name == name))
        {
            context.Departments.Add(new Department { Name = name });
        }
    }
    await context.SaveChangesAsync();


    var adminRole = "Administrator";
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    var adminEmail = "aleksandarhadzic1986@gmail.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        var itDept = await context.Departments.FirstOrDefaultAsync(d => d.Name == "IT");
        if (itDept == null)
            throw new Exception("Department 'IT' not found. Seeding departments failed.");

        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FirstName = "Aleksandar",
            LastName = "Hadžić",
            DepartmentId = itDept.Id
        };

        var result = await userManager.CreateAsync(adminUser, "Admin123!");

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await userManager.AddToRoleAsync(adminUser, adminRole);
    }
}

app.Run();

