namespace TMS.Models.ViewModels
{
    public class CreateTaskRequest
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
        public string Priority { get; set; } = "Medium";
        public int? AssignedUserId { get; set; }
    }

    public class UpdateTaskRequest
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
        public string Priority { get; set; } = "Medium";
        public int? AssignedUserId { get; set; }
    }

    public class UpdateTaskStatusRequest
    {
        public required string Status { get; set; }
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
