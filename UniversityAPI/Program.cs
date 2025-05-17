using Microsoft.Extensions.Configuration;
using UniversityAPI.Models;
using UniversityAPI.Extensions;
using UniversityAPI.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
namespace UniversityAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddRepositories();
            builder.Services.AddUniversityDbContext(builder.Configuration);
            builder.Services.AddAspNetIdentity();
            builder.Services.InitAuth(builder.Configuration);
            builder.Services.InitSwagger();
            builder.Services.InitCors();

            var app = builder.Build();

            var serviceScopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = serviceScopeFactory.CreateScope();
            {
                var universityDbContext = scope.ServiceProvider.GetRequiredService<UniversityDbContext>();
                universityDbContext.Database.Migrate();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                await roleManager.CreateAsync(new IdentityRole(UserRoleDefaults.User));
                await roleManager.CreateAsync(new IdentityRole(UserRoleDefaults.Admin));
            }

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.UseCors("University");
            app.Run();
        }
    }
}
