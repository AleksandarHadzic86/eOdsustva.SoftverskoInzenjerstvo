using System.ComponentModel.DataAnnotations;

namespace eOdsustva.SoftverskoInzenjerstvo.Data
{
    public class Period : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateOnly StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateOnly EndDate { get; set; }
    }
}
