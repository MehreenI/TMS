namespace TMS.Models.ViewModels
{
    public class UserDtos
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public int TaskCount { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();
        public string? ProfileImagePath { get; set; }
        public string Status => LastLogin.HasValue ? "Active" : "Inactive";
    }
}
