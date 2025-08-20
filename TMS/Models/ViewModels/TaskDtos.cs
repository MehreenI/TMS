namespace TMS.Models.ViewModels
{
    public class TaskDtos
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? Deadline { get; set; }
        public string Status { get; set; } = "To Do";
        public string Priority { get; set; } = "Medium";
        public int? AssignedUserId { get; set; }
        public string? AssignedUserName { get; set; }
        public int CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
    }
}
