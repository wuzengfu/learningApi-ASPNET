using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningAPI.Models
{
    public class Tutorial
    {
        public int Id { get; set; }
        
        [Required, MinLength(3), MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        
        [Required, MinLength(3), MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        
        public int UserId { get; set; }
        
        public User? User { get; set; }
    }
}