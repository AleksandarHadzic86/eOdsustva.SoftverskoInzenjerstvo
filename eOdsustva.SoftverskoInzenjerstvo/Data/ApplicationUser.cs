using Microsoft.AspNetCore.Identity;

namespace eOdsustva.SoftverskoInzenjerstvo.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }

        public int? DepartmentId { get; set; }   
        public Department Department { get; set; }
    }
}
