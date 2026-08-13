using System.Text;
using AssignmentManagementSystem.API.Middleware;
using AssignmentManagementSystem.Infrastructure;
using AssignmentManagementSystem.Infrastructure.Data;
using AssignmentManagementSystem.Infrastructure.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder=WebApplication.CreateBuilder(args);

Log.Logger=new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).Enrich.FromLogContext().WriteTo.Console().CreateLogger();
builder.Host.UseSerilog();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o=>{
    o.SwaggerDoc("v1",new OpenApiInfo{Title="Assignment Management System API",Version="v1"});
    o.AddSecurityDefinition("Bearer",new OpenApiSecurityScheme{Name="Authorization",Type=SecuritySchemeType.Http,Scheme="bearer",BearerFormat="JWT",In=ParameterLocation.Header,Description="Bearer {token}"});
    o.AddSecurityRequirement(new OpenApiSecurityRequirement{{new OpenApiSecurityScheme{Reference=new OpenApiReference{Type=ReferenceType.SecurityScheme,Id="Bearer"}} ,Array.Empty<string>()}});
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddInfrastructure(builder.Configuration);

var key=builder.Configuration["Jwt:Key"]??throw new InvalidOperationException("Jwt:Key is missing.");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt");

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwt["Issuer"],
                ValidAudience = jwt["Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwt["Key"]!))
            };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
var app=builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();
if(app.Environment.IsDevelopment()){app.UseSwagger();app.UseSwaggerUI();}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using(var scope=app.Services.CreateScope())
{
    var db=scope.ServiceProvider.GetRequiredService<MongoContext>();
    await MongoIndexes.CreateAsync(db);
    await scope.ServiceProvider.GetRequiredService<DatabaseSeeder>().SeedAsync();
}
app.Run();
public partial class Program{}