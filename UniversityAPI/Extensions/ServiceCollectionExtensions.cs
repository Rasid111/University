using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityAPI.EntityFramework;
using UniversityAPI.Models;
using UniversityAPI.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UniversityAPI.Repositories;

namespace UniversityAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<DegreeRepository>();
            services.AddScoped<FacultyRepository>();
            services.AddScoped<GroupRepository>();
            services.AddScoped<MajorRepository>();
            services.AddScoped<QuestionRepository>();
            services.AddScoped<StudentProfileRepository>();
            services.AddScoped<SubjectRepository>();
            services.AddScoped<TestRepository>();
            services.AddScoped<UserRepository>();
            services.AddScoped<TeacherProfileRepository>();
            services.AddScoped<GradeRepository>();
        }
        public static void AddUniversityDbContext(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddDbContext<UniversityDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("UniversityConnectionString"));
            });

        }
        public static void AddAspNetIdentity(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<UniversityDbContext>();
        }
        public static void InitAuth(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("Jwt");
            serviceCollection.Configure<JwtOptions>(jwtSection);

            serviceCollection.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    var jwtOptions = jwtSection.Get<JwtOptions>();

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "University",

                        ValidateAudience = true,
                        ValidAudience = "User",

                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(jwtOptions?.KeyInBytes)
                    };
                });
        }
        public static void InitSwagger(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "University",
                    Version = "v1",
                });

                options.AddSecurityDefinition(
                    name: JwtBearerDefaults.AuthenticationScheme,
                    securityScheme: new OpenApiSecurityScheme()
                    {
                        Description = "Input yout JWT token here:",
                        In = ParameterLocation.Header,
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                    });

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement() {
                        {
                            new OpenApiSecurityScheme() {
                                Reference = new OpenApiReference() {
                                    Id = JwtBearerDefaults.AuthenticationScheme,
                                    Type = ReferenceType.SecurityScheme
                                }
                            },
                            Array.Empty<string>()
                        }
                    }
                );
            });
        }
        public static void InitCors(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddCors(options =>
            {
                options.AddPolicy("University", policyBuilder =>
                {
                    policyBuilder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }
    }
}
