using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eOdsustva.SoftverskoInzenjerstvo.Data
{
    public class LeaveType: BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(1, 365)]
        public int NumberOfDays { get; set; }
    }
}
