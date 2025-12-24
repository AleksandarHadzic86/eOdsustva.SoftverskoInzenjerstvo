using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eOdsustva.SoftverskoInzenjerstvo.Data
{
    public class LeaveType
    {
        
        public int Id { get; set; }

        [Display(Name = "Naziv")]
        public string Name { get; set; }

        [Display(Name = "Broj dana")]   
        public int NumberOfDays { get; set; }
    }
}
