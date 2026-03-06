using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi;
using MySecureBackend.WebApi.Interface;
using MySecureBackend.WebApi.Repositories;
using MySecureBackend.WebApi.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Register MVC controllers for handling HTTP requests.
builder.Services.AddControllers();

// Retrieve the SQL connection string from configuration.
var sqlConnectionString = builder.Configuration.GetValue<string>("SqlConnectionString");
var sqlConnectionStringFound = !string.IsNullOrWhiteSpace(sqlConnectionString);

// ✅ NIEUW: CORS toevoegen voor Unity
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUnity", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register OpenAPI/Swagger for API documentation and testing.
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MySecureBackend API",
        Version = "v1",
    });
});

builder.Services.Configure<RouteOptions>(o => o.LowercaseUrls = true);

// Register authorization services for securing endpoints.
builder.Services.AddAuthorization();

builder.Services.ConfigureApplicationCookie(Options =>
{
    Options.ExpireTimeSpan = TimeSpan.FromHours(8);
    Options.SlidingExpiration = true;
    Options.Cookie.SameSite = SameSiteMode.None; // ✅ NIEUW: Voor cross-origin requests
    Options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // ✅ NIEUW: Alleen HTTPS
});

// Register ASP.NET Core Identity with Dapper stores for user authentication and management.
builder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>()
.AddDapperStores(options =>
{
    options.ConnectionString = sqlConnectionString;
});

// Register IHttpContextAccessor for accessing HTTP context in services.
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IAuthenticationService, AspNetIdentityAuthenticationService>();

// Register application repositories.
builder.Services.AddTransient<IEnvironment2DRepository, SqlEnvironment2DRepository>(o => new SqlEnvironment2DRepository(sqlConnectionString!));
builder.Services.AddTransient<IObject2DRepository, SqlObject2DRepository>(o => new SqlObject2DRepository(sqlConnectionString!));

var app = builder.Build();

// Register OpenAPI/Swagger endpoints.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MySecureBackend API v1");
        options.RoutePrefix = "swagger";
        options.CacheLifetime = TimeSpan.Zero;

        if (!sqlConnectionStringFound)
            options.HeadContent = "<h1 align=\"center\">❌ SqlConnectionString not found ❌</h1>";
    });
}
else
{
    var buildTimeStamp = File.GetCreationTime(Assembly.GetExecutingAssembly().Location);
    string currentHealthMessage = $"The API is up 🚀 | Connection string found: {(sqlConnectionStringFound ? "✅" : "❌")} | Build timestamp: {buildTimeStamp}";

    app.MapGet("/", () => currentHealthMessage);
}

// ✅ NIEUW: CORS middleware VOOR authentication!
app.UseCors("AllowUnity");

// Enforce HTTPS for all requests.
app.UseHttpsRedirection();

// Enable authorization middleware.
app.UseAuthorization();

// Register Identity endpoints for account management.
app.MapGroup("/account").MapIdentityApi<IdentityUser>().WithTags("Account");

// Register all controller endpoints for the application.
app.MapControllers();

app.Run();