using System.ComponentModel.DataAnnotations;

namespace TMS.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public DateTime? Deadline { get; set; }
        
        [Required]
        public string Priority { get; set; } = "Medium"; // Low, Medium, High
        
        [Required]
        public string Status { get; set; } = "ToDo"; // ToDo, InProgress, Done
        
        public int? AssignedUserId { get; set; }
        
        public string? AssignedUserName { get; set; }
        
        public int CreatedByUserId { get; set; }
        
        public string? CreatedByUserName { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
