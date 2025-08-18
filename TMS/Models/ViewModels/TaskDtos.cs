using System.ComponentModel.DataAnnotations;

namespace TMS.Models.ViewModels
{
    public class TaskDtos
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = "To Do"; // To Do, In Progress, Done
        public string Priority { get; set; } = "Medium"; // Low, Medium, High
        public int? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
        public int CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
    }
}
