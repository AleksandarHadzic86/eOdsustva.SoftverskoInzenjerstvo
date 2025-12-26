using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eOdsustva.SoftverskoInzenjerstvo.Data
{
    public class LeaveType: BaseEntity
    {   
        public string Name { get; set; }
        public int NumberOfDays { get; set; }
    }
}
