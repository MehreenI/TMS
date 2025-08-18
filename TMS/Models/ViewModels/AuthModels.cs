namespace TMS.Models.ViewModels
{
    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
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
        public required string Token { get; set; }
    }

    public class ValidateTokenResponse
    {
        public bool IsValid { get; set; }
        public UserDtos? User { get; set; }
        public string? Message { get; set; }
    }
}
