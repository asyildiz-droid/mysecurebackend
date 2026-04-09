using Microsoft.OpenApi;
using MySecureBackend.WebApi.Interface;
using MySecureBackend.WebApi.Repositories;
using MySecureBackend.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Dwing de poort af vóórdat er ook maar IETS anders gebouwd wordt.
// Dit voorkomt dat Render een "Port scan timeout" foutmelding geeft.
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
builder.WebHost.UseUrls($"http://*:{port}");

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

// Register authorization services
builder.Services.AddAuthorization();

// Register IHttpContextAccessor for accessing HTTP context in services.
builder.Services.AddHttpContextAccessor();

// Onze Services & Repositories registreren (gekoppeld aan Postgres!)
builder.Services.AddTransient<PasswordService>();
builder.Services.AddSingleton<IUserRepository>(sp => new UserRepository(sqlConnectionString!));
builder.Services.AddTransient<IEnvironment2DRepository, SqlEnvironment2DRepository>(o => new SqlEnvironment2DRepository(sqlConnectionString!));
builder.Services.AddTransient<IObject2DRepository, SqlObject2DRepository>(o => new SqlObject2DRepository(sqlConnectionString!));

// NU BOULWEN WE PAS DE APP
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
    var buildTimeStamp = System.IO.File.GetCreationTime(System.Reflection.Assembly.GetExecutingAssembly().Location);
    string currentHealthMessage = $"The API is up 🚀 | Connection string found: {(sqlConnectionStringFound ? "✅" : "❌")} | Build timestamp: {buildTimeStamp}";

    app.MapGet("/", () => currentHealthMessage);
}

// ✅ NIEUW: CORS middleware
app.UseCors("AllowUnity");

// Enforce HTTPS for all requests.
app.UseHttpsRedirection();

// Enable authorization middleware.
app.UseAuthorization();

// Register all controller endpoints for the application.
app.MapControllers();

app.Run();