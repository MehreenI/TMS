using TMS.Models;
using TMS.Models.ViewModels;

namespace TMS.Services
{
    public interface IApiService
    {
        // Auth endpoints
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<bool> ValidateTokenAsync(string token);
        
        // User endpoints
        Task<List<UserDtos>?> GetUsersAsync();
        Task<UserDtos?> GetUserAsync(int id);
        Task<UserDtos?> CreateUserAsync(CreateUserRequest request);
        Task<UserDtos?> UpdateUserAsync(int id, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ChangePasswordAsync(int id, ChangePasswordRequest request);
        
        // Task endpoints
        Task<List<TaskDtos>?> GetTasksAsync();
        Task<List<TaskDtos>?> GetMyTasksAsync();
        Task<TaskDtos?> GetTaskAsync(int id);
        Task<TaskDtos?> CreateTaskAsync(CreateTaskRequest request);
        Task<TaskDtos?> UpdateTaskAsync(int id, UpdateTaskRequest request);
        Task<bool> DeleteTaskAsync(int id);
        Task<TaskDtos?> UpdateTaskStatusAsync(int id, UpdateTaskStatusRequest request);
        Task<List<TaskDtos>?> GetDueSoonTasksAsync(int hours = 24);
        Task<DashboardStats?> GetDashboardStatsAsync();
        
        // Announcement endpoints
        Task<List<Models.AnnouncementData>?> GetAnnouncementsAsync();
        Task<Models.AnnouncementData?> GetAnnouncementAsync(int id);
        Task<Models.AnnouncementData?> CreateAnnouncementAsync(CreateAnnouncementRequest request);
        Task<Models.AnnouncementData?> UpdateAnnouncementAsync(int id, UpdateAnnouncementRequest request);
        Task<bool> DeleteAnnouncementAsync(int id);
        
        // Email template endpoints
        Task<bool> UpdateEmailTemplateAsync(string templateId, EmailTemplateUpdateRequest request);
        
        // Health endpoint
        Task<bool> CheckHealthAsync();
    }
}
