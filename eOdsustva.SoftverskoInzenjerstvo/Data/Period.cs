using System.ComponentModel.DataAnnotations;

namespace eOdsustva.SoftverskoInzenjerstvo.Data
{
    public class Period : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateOnly StartDate { get; set; } 

        [DataType(DataType.Date)]
        public DateOnly EndDate { get; set; }
    }
}
