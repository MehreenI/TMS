using System.ComponentModel.DataAnnotations;

namespace TMS.Models.ViewModels
{
    public class CreateTaskRequest
    {
        [Required(ErrorMessage = "Task title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Deadline is required")]
        public DateTime? Deadline { get; set; }
        
        [Required(ErrorMessage = "Priority is required")]
        public string Priority { get; set; } = "Medium";
        
        public int? AssignedUserId { get; set; }
    }

    public class UpdateTaskRequest
    {
        [Required(ErrorMessage = "Task title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Deadline is required")]
        public DateTime? Deadline { get; set; }
        
        [Required(ErrorMessage = "Priority is required")]
        public string Priority { get; set; } = "Medium";
        
        public int? assignedUserId { get; set; }
    }

    public class DashboardStats
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int DueTodayTasks { get; set; }
        public int DueTomorrowTasks { get; set; }
        public Dictionary<string, int> TasksByStatus { get; set; } = new();
        public Dictionary<string, int> TasksByPriority { get; set; } = new();
    }
}
