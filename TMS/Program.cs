using TMS.Middleware;
using TMS.Services;
// beginner-optimized: no options class or custom handlers

var builder = WebApplication.CreateBuilder(args);

// beginner-optimized: read config inline when needed

// Add services
builder.Services.AddRazorPages(options =>
{
	options.Conventions.AddPageRoute("/Home/Index", "/");
});

// For a Razor Pages app, controllers are optional; keep only if you have MVC controllers
// builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

// Antiforgery
builder.Services.AddAntiforgery(opts =>
{
	opts.HeaderName = "X-CSRF-TOKEN";
});

// Simple CORS policy (adjust per environment)
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
	{
		policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
	});
});

// Session
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("SessionTimeout", 30));
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Simple typed HttpClient
builder.Services.AddHttpClient<IApiService, ApiService>(http =>
{
	var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001";
	var timeout = builder.Configuration.GetValue<int>("ApiSettings:Timeout", 30);
	http.BaseAddress = new Uri(baseUrl);
	http.Timeout = TimeSpan.FromSeconds(timeout);
});

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddLogging();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseSession();

app.UseAuthMiddleware();

app.UseAuthorization();

app.MapRazorPages();
// app.MapControllers();

app.Run();
