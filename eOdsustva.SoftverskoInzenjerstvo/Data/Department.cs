using System.ComponentModel.DataAnnotations;

namespace eOdsustva.SoftverskoInzenjerstvo.Data
{
    public class Department : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }

}
