using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UserManagementAPI.Endpoints;
using UserManagementAPI.Middleware;
using UserManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration
    .GetSection(JwtBearerSettings.ConfigurationSection)
    .Get<JwtBearerSettings>() ?? new JwtBearerSettings();

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddValidation();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddOptions<JwtBearerSettings>()
    .Bind(builder.Configuration.GetSection(JwtBearerSettings.ConfigurationSection))
    .Validate(settings => !string.IsNullOrWhiteSpace(settings.Issuer), "JWT issuer must be configured.")
    .Validate(settings => !string.IsNullOrWhiteSpace(settings.Audience), "JWT audience must be configured.")
    .Validate(settings => Encoding.UTF8.GetByteCount(settings.SigningKey) >= 32,
        "JWT signing key must contain at least 32 UTF-8 bytes.")
    .ValidateOnStart();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey)),
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            RequireExpirationTime = true,
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthorization();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseMiddleware<RequestAuditMiddleware>();
app.UseAuthorization();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapUserEndpoints();

app.Run();