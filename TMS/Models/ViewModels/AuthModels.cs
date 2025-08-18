using System.ComponentModel.DataAnnotations;

namespace TMS.Models.ViewModels
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public UserDtos? User { get; set; }
        public string? Message { get; set; }
    }

    public class ValidateTokenRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }

    public class ValidateTokenResponse
    {
        public bool IsValid { get; set; }
        public UserDtos? User { get; set; }
        public string? Message { get; set; }
    }
}
