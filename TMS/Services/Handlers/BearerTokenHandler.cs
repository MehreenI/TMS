using System.Net.Http.Headers;

namespace TMS.Services.Handlers
{
	public class BearerTokenHandler : DelegatingHandler
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public BearerTokenHandler(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var token = _httpContextAccessor.HttpContext?.Session?.GetString("JWTToken");
			if (!string.IsNullOrWhiteSpace(token))
			{
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			}
			return await base.SendAsync(request, cancellationToken);
		}
	}
}
