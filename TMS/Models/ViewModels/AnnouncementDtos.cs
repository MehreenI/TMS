using System.ComponentModel.DataAnnotations;

namespace TMS.Models.ViewModels
{
    public class AnnouncementDtos
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public int CreatedByUserId { get; set; }
        
        public string? CreatedByUserName { get; set; }
    }
}
