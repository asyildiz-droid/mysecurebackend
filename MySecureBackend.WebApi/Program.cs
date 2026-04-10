using Microsoft.OpenApi;
using MySecureBackend.WebApi.Interface;
using MySecureBackend.WebApi.Repositories;
using MySecureBackend.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ✅ FIX VOOR RENDER DEMO: Forceer expliciet IPv4 IP-adres (0.0.0.0) 
// Hierdoor zal Render's ping hem altijd kunnen vinden!
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Register MVC controllers for handling HTTP requests.
builder.Services.AddControllers();

// Retrieve the SQL connection string from configuration.
var sqlConnectionString = builder.Configuration.GetValue<string>("SqlConnectionString");

// CORS toevoegen voor Unity
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
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

// Onze Services & Repositories registreren (gekoppeld aan Postgres!)
builder.Services.AddTransient<PasswordService>();
builder.Services.AddSingleton<IUserRepository>(sp => new UserRepository(sqlConnectionString!));
builder.Services.AddTransient<IEnvironment2DRepository, SqlEnvironment2DRepository>(o => new SqlEnvironment2DRepository(sqlConnectionString!));
builder.Services.AddTransient<IObject2DRepository, SqlObject2DRepository>(o => new SqlObject2DRepository(sqlConnectionString!));

// NU BOUWEN WE PAS DE APP
var app = builder.Build();

// Register OpenAPI/Swagger endpoints.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MySecureBackend API v1");
        options.RoutePrefix = "swagger";
    });
}
else
{
    // ✅ GEFIXT: MapGet EN MapMethods(HEAD) zorgen dat Render de API ping 100% succesvol doorkrijgt
    app.MapGet("/", () => "API is Live! 🚀");
    app.MapMethods("/", new[] { "HEAD" }, () => "API is Live! 🚀");
}

app.UseCors("AllowUnity");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();