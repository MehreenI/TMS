namespace TMS.Models.ViewModels
{
    public class UserApiResponseDto
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public string Role { get; set; } = "User";
        public string? ProfileImagePath { get; set; }
        public int? TaskCount { get; set; }
    }

    public class UserDtos
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public string? Password { get; set; }
        public string Role { get; set; } = "User";
        public string? ProfileImagePath { get; set; }
        public int? TaskCount { get; set; }

    }
}
