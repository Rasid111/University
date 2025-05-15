
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.Configuration;
using System.Data;
using UniversityAPI.Database;

namespace UniversityAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            string connectionString = builder.Configuration.GetConnectionString("University") ?? throw new InvalidConfigurationException();
            string accountsConnectionString = builder.Configuration.GetConnectionString("UniversityAccounts") ?? throw new InvalidConfigurationException();

            builder.Services.AddDbContext<UniversityDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            var serviceScopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var r_ExamDbContext = scope.ServiceProvider.GetService<UniversityDbContext>();
                r_ExamDbContext?.Database.Migrate();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
