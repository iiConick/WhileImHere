using System.ComponentModel.DataAnnotations;

namespace WhileImHere.Models
{
    public class Location
    {
        public int LocationID { get; set; }
        [Required]
        [MaxLength(50)]
        public string? LocationName { get; set; }
        [Required]
        [Range(1,50)]
        public string? LocationRadius { get; set; }
        [Required]
        public int LocationPriority { get; set; }


        public List<Task>? Tasks { get; set; } 
      
        

    }
}
