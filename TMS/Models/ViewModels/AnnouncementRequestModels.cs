namespace TMS.Models.ViewModels
{
    public class CreateAnnouncementRequest
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
    }

    public class UpdateAnnouncementRequest
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
    }
}
