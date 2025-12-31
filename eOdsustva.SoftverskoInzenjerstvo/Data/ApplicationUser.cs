using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace eOdsustva.SoftverskoInzenjerstvo.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
