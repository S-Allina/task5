using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Users.Application.DTO;
using Users.Application.Interfaces;
using Users.Domain.Entity;
using Users.Infrastructure;
using Users.Infrastructure.DataAccess.Data;
using Users.Infrastructure.Services;
using Users.Presentation.API.Middleware;
using Users.Presentation.Constants;
using static Org.BouncyCastle.Math.EC.ECCurve;
using static System.Net.WebRequestMethods;

namespace Users.Presentation.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;

            var emailSettings = new EmailSettings();
            configuration.GetSection("EmailSettings").Bind(emailSettings);
            builder.Services.AddSingleton(emailSettings);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(o =>
            {
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireDigit = false;
                o.Password.RequiredLength = 1;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.SignIn.RequireConfirmedAccount = false;
                o.User.RequireUniqueEmail = true;
                o.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.WithOrigins("https://s-allina.github.io/theapp", "https://s-allina.github.io", "https://s-allina.github.io/theapp/#/theapp")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                });
            });
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(3);
                options.SlidingExpiration = true;
                options.LoginPath = "/api/identity/login";
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
            builder.Services.Configure<JsonOptions>(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            builder.Services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(3));
            builder.Services.AddHttpContextAccessor();
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddAutoMapper(typeof(UserMappingProfile).Assembly);
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IRegistrationService, RegistrationService>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(SwaggerConstants.SwaggerVersion, new OpenApiInfo
                {
                    Title = AuthPolicyConstants.Title,
                    Version = SwaggerConstants.SwaggerVersion,
                    Description = AuthPolicyConstants.Description
                });

                c.AddSecurityDefinition(SwaggerConstants.Scheme, new OpenApiSecurityScheme
                {
                    Name = SwaggerConstants.Name,
                    Type = SwaggerConstants.Type,
                    Scheme = SwaggerConstants.Scheme,
                    BearerFormat = SwaggerConstants.BearerFormat,
                    In = SwaggerConstants.In,
                    Description = SwaggerConstants.Description
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = SwaggerConstants.TypeReference,
                            Id = SwaggerConstants.Scheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
            });

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            app.UseMiddleware<UserStatusMiddleware>();
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
