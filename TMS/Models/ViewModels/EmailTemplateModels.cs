namespace TMS.Models.ViewModels
{
	public class EmailTemplateUpdateRequest
	{
		public string Subject { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
	}
}
