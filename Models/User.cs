using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LearningAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;
        
        [MaxLength(100), JsonIgnore]
        public string Password { get; set; } = string.Empty;
        
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        
        [JsonIgnore]
        public List<Tutorial>? Tutorials { get; set; }
    }
}