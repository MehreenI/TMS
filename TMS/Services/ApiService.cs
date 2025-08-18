using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using TMS.Models;
using TMS.Models.ViewModels;

namespace TMS.Services
{
	public class ApiService : IApiService
	{
		private readonly HttpClient _httpClient;
		private readonly ILogger<ApiService> _logger;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ApiService(HttpClient httpClient, ILogger<ApiService> logger, IHttpContextAccessor httpContextAccessor)
		{
			_httpClient = httpClient;
			_logger = logger;
			_httpContextAccessor = httpContextAccessor;
		}

		private void AttachBearerIfPresent()
		{
			try
			{
				var token = _httpContextAccessor.HttpContext?.Session?.GetString("JWTToken");
				if (!string.IsNullOrWhiteSpace(token))
				{
					_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				}
			}
			catch { }
		}

		private async Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default)
		{
			try
			{
				AttachBearerIfPresent();
				var response = await _httpClient.GetAsync(endpoint, ct);
				if (response.IsSuccessStatusCode)
				{
					return await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
				}
				_logger.LogWarning("API GET failed: {Endpoint}, Status: {StatusCode}", endpoint, response.StatusCode);
				return default;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error calling API endpoint: {Endpoint}", endpoint);
				return default;
			}
		}

		private async Task<T?> SendJsonAsync<T>(HttpMethod method, string endpoint, object? data, CancellationToken ct = default)
		{
			try
			{
				AttachBearerIfPresent();
				using var request = new HttpRequestMessage(method, endpoint);
				if (data != null)
				{
					var json = JsonSerializer.Serialize(data);
					request.Content = new StringContent(json, Encoding.UTF8, "application/json");
				}
				var response = await _httpClient.SendAsync(request, ct);
				if (response.IsSuccessStatusCode)
				{
					if (typeof(T) == typeof(bool))
					{
						return (T)(object)true;
					}
					return await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
				}
				_logger.LogWarning("API {Method} failed: {Endpoint}, Status: {StatusCode}", method, endpoint, response.StatusCode);
				if (typeof(T) == typeof(bool)) return (T)(object)false;
				return default;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error calling API endpoint: {Endpoint}", endpoint);
				if (typeof(T) == typeof(bool)) return (T)(object)false;
				return default;
			}
		}

		// Auth endpoints
		public Task<AuthResponse?> LoginAsync(LoginRequest request) => SendJsonAsync<AuthResponse>(HttpMethod.Post, "/api/Auth/login", request);
		public async Task<bool> ValidateTokenAsync(string token)
		{
			try
			{
				using var req = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/validate-token");
				// Prefer the provided token explicitly
				req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
				var resp = await _httpClient.SendAsync(req);
				return resp.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error validating token");
				return false;
			}
		}

		// User endpoints
		public Task<List<UserDtos>?> GetUsersAsync() => GetAsync<List<UserDtos>>("/api/Users");
		public Task<UserDtos?> GetUserAsync(int id) => GetAsync<UserDtos>($"/api/Users/{id}");
		public Task<UserDtos?> CreateUserAsync(CreateUserRequest request) => SendJsonAsync<UserDtos>(HttpMethod.Post, "/api/Users", request);
		public Task<UserDtos?> UpdateUserAsync(int id, UpdateUserRequest request) => SendJsonAsync<UserDtos>(HttpMethod.Put, $"/api/Users/{id}", request);
		public Task<bool> DeleteUserAsync(int id) => SendJsonAsync<bool>(HttpMethod.Delete, $"/api/Users/{id}", null);
		public async Task<bool> ChangePasswordAsync(int id, ChangePasswordRequest request) => await SendJsonAsync<bool>(HttpMethod.Post, $"/api/Users/{id}/change-password", request);

		// Task endpoints
		public Task<List<TaskDtos>?> GetTasksAsync() => GetAsync<List<TaskDtos>>("/api/TaskItems");
		public Task<List<TaskDtos>?> GetMyTasksAsync() => GetAsync<List<TaskDtos>>("/api/TaskItems/my-tasks");
		public Task<TaskDtos?> GetTaskAsync(int id) => GetAsync<TaskDtos>($"/api/TaskItems/{id}");
		public Task<TaskDtos?> CreateTaskAsync(CreateTaskRequest request) => SendJsonAsync<TaskDtos>(HttpMethod.Post, "/api/TaskItems", request);
		public Task<TaskDtos?> UpdateTaskAsync(int id, UpdateTaskRequest request) => SendJsonAsync<TaskDtos>(HttpMethod.Put, $"/api/TaskItems/{id}", request);
		public Task<bool> DeleteTaskAsync(int id) => SendJsonAsync<bool>(HttpMethod.Delete, $"/api/TaskItems/{id}", null);
		public Task<TaskDtos?> UpdateTaskStatusAsync(int id, UpdateTaskStatusRequest request) => SendJsonAsync<TaskDtos>(HttpMethod.Patch, $"/api/TaskItems/{id}/status", request);
		public Task<List<TaskDtos>?> GetDueSoonTasksAsync(int hours = 24) => GetAsync<List<TaskDtos>>($"/api/TaskItems/due-soon?hours={hours}");
		public Task<DashboardStats?> GetDashboardStatsAsync() => GetAsync<DashboardStats>("/api/TaskItems/dashboard-stats");

		// Announcement endpoints
		public Task<List<Models.AnnouncementData>?> GetAnnouncementsAsync() => GetAsync<List<Models.AnnouncementData>>("/api/Announcements");
		public Task<Models.AnnouncementData?> GetAnnouncementAsync(int id) => GetAsync<Models.AnnouncementData>($"/api/Announcements/{id}");
		public Task<Models.AnnouncementData?> CreateAnnouncementAsync(CreateAnnouncementRequest request) => SendJsonAsync<Models.AnnouncementData>(HttpMethod.Post, "/api/Announcements", request);
		public Task<Models.AnnouncementData?> UpdateAnnouncementAsync(int id, UpdateAnnouncementRequest request) => SendJsonAsync<Models.AnnouncementData>(HttpMethod.Put, $"/api/Announcements/{id}", request);
		public Task<bool> DeleteAnnouncementAsync(int id) => SendJsonAsync<bool>(HttpMethod.Delete, $"/api/Announcements/{id}", null);

		// Email template endpoints
		public async Task<bool> UpdateEmailTemplateAsync(string templateId, EmailTemplateUpdateRequest request)
		{
			return await SendJsonAsync<bool>(HttpMethod.Put, $"/api/EmailTemplates/{templateId}", request);
		}

		// Health endpoint
		public async Task<bool> CheckHealthAsync()
		{
			try
			{
				var response = await _httpClient.GetAsync("/api/Health");
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error checking health");
				return false;
			}
		}
	}
}
