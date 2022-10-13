using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WhileImHere.Models
{
    public class Task
    {
        public int TaskID { get; set; }
        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }
        [MaxLength(350)]
        public string? Description { get; set; }
        [Required]
        [MaxLength(70)]
        public string? TaskStreetAddress { get; set; }
        [Required]
        [Range(1, 50)]
        public int TaskRadius { get; set; }

        // FK for Parent Category
        [Display(Name = "Location")]
        public int LocationID { get; set; }

        public Location? Location { get; set; } 

    }
}
