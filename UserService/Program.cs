using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UserService;

var builder = WebApplication.CreateBuilder(args);
var identityServerConfig = builder.Configuration.GetSection("IdentityServerConfig");

builder.Services.AddControllers();
builder.Services.Configure<IdentityServerConfig>(identityServerConfig);
builder.Services.AddHttpClient();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = identityServerConfig["IdentityServerUrl"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("APIAccess", policy =>
    {
        policy.RequireClaim("scope", "userService");
    });
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "Hello from user service!");

app.Run();