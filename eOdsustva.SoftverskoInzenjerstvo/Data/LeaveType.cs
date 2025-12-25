using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eOdsustva.SoftverskoInzenjerstvo.Data
{
    public class LeaveType
    {
        
        public int Id { get; set; }

        public string Name { get; set; }

        public int NumberOfDays { get; set; }
    }
}
