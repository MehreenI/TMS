namespace TMS.Models.ViewModels
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public UserDtos? User { get; set; }
        public string? Message { get; set; }
    }
}
