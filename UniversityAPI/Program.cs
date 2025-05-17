using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using UniversityAPI.EntityFramework;
using UniversityAPI.Extensions;
using UniversityAPI.Models;
using UniversityAPI.Services;

namespace UniversityAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add controllers with reference loop fix
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });

            // Swagger/OpenAPI support
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.InitSwagger();

            // Add database and Identity
            builder.Services.AddUniversityDbContext(builder.Configuration);
            builder.Services.AddAspNetIdentity();

            // Add custom repositories
            builder.Services.AddRepositories();

            // Add authentication and CORS
            builder.Services.InitAuth(builder.Configuration);
            builder.Services.InitCors();

            // Add BlobService for file uploads
            builder.Services.AddSingleton<BlobService>();

            var app = builder.Build();

            // Apply pending migrations and seed roles
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<UniversityDbContext>();
                db.Database.Migrate();

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                await roleManager.CreateAsync(new IdentityRole(UserRoleDefaults.User));
                await roleManager.CreateAsync(new IdentityRole(UserRoleDefaults.Admin));
                await roleManager.CreateAsync(new IdentityRole("Teacher"));
                await roleManager.CreateAsync(new IdentityRole("Student"));
            }

            // Middleware pipeline
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("University");

            app.MapControllers();
            app.Run();
        }
    }
}
