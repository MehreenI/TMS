using System.ComponentModel.DataAnnotations;

namespace TMS.Models.ViewModels
{
    public class CreateAnnouncementRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;
    }

    public class UpdateAnnouncementRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
