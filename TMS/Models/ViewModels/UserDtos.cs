using System.ComponentModel.DataAnnotations;

namespace TMS.Models.ViewModels
{
    public class UserApiResponseDto
    {
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = "User";
        public string? ProfileImagePath { get; set; }
        public int? TaskCount { get; set; }
    }

    public class UserDtos
    {
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public string Role { get; set; } = "User";
        public string? ProfileImagePath { get; set; }
        public int? TaskCount { get; set; }

    }
}
