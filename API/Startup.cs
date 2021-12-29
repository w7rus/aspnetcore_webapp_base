using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using API.Configuration;
using API.Middleware;
using BLL.Handlers;
using BLL.Services;
using Common.Helpers;
using Common.Models;
using Common.Options;
using DAL.Data;
using DAL.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

namespace API
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Logger
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Logger(_ =>
                {
                    _.MinimumLevel.Error()
                        .WriteTo.File(
                            Path.Combine(Directory.GetCurrentDirectory(), "Logs",
                                $"log_error_{DateTime.UtcNow:yyyy_mm_dd}.log"),
                            LogEventLevel.Error, rollingInterval: RollingInterval.Day);
                });


            if (_env.IsDevelopment())
            {
                loggerConfiguration
                    .WriteTo.Logger(_ =>
                    {
                        _.MinimumLevel.Information()
                            .WriteTo.Console()
                            .WriteTo.File(
                                Path.Combine(Directory.GetCurrentDirectory(), "Logs",
                                    $"log_debug_{DateTime.UtcNow:yyyy_mm_dd}.log"),
                                rollingInterval: RollingInterval.Day);
                    });
            }

            Log.Logger = loggerConfiguration.CreateLogger();

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            services.AddSingleton(Log.Logger);

            //IOptions<>
            {
                services.AddOptions();
                services.Configure<GoogleReCaptchaV2Options>(
                    Configuration.GetSection(nameof(GoogleReCaptchaV2Options)));
                services.Configure<JsonWebTokenOptions>(Configuration.GetSection(nameof(JsonWebTokenOptions)));
                services.Configure<RefreshTokenOptions>(Configuration.GetSection(nameof(RefreshTokenOptions)));
            }

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API",
                    Version = "v1",
                    Description = "An API of ASP.NET Core Web Application Base",
                    Contact = new OpenApiContact
                    {
                        Name = "Grigory Bragin",
                        Url = new Uri("https://t.me/w7rus"),
                        Email = "bragingrigory@gmail.com"
                    }
                });

                c.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Scheme = "Bearer",
                        Name = "Authorization"
                    });

                c.OperationFilter<AuthorizeCheckOperationFilter>();
                c.EnableAnnotations();
            });

            services.AddHttpContextAccessor();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errorModelResult = new ErrorModelResult
                    {
                        Errors = new List<KeyValuePair<string, string>>()
                    };

                    foreach (var modelError in context.ModelState.Values.SelectMany(modelStateValue =>
                                 modelStateValue.Errors))
                        errorModelResult.Errors.Add(new(Localize.ErrorType.ModelState,
                            modelError.ErrorMessage));

                    return new BadRequestObjectResult(errorModelResult);
                };
            });

            // Disable default object model validator
            // services.AddSingleton<IObjectModelValidator, CustomObjectModelValidator>();

            //Authentication
            {
                services.AddSingleton<IConfigureOptions<AuthenticationOptions>, ConfigureAuthenticationOptions>();
                services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

                // //TODO: Probably useless since there is AuthorizeJsonWebToken & AuthorizeExpiredJsonWebToken attributes
                // services.AddAuthentication()
                //     .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, null!);
            }

            //DbContext
            {
                services.AddScoped<AppDbContext>();
                services.AddScoped<IAppDbContextAction, AppDbContextAction>();

                services.AddDbContext<AppDbContext>(options =>
                {
                    options
                        .UseNpgsql(Configuration.GetConnectionString("Default"),
                            _ => _.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery))
                        .UseLazyLoadingProxies();
                });
            }

            //DI
            {
                //Repositories
                services.AddScoped<IJsonWebTokenRepository, JsonWebTokenRepository>();
                services.AddScoped<IPermissionRepository, PermissionRepository>();
                services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
                services.AddScoped<IUserGroupPermissionValueRepository, UserGroupPermissionValueRepository>();
                services.AddScoped<IUserGroupRepository, UserGroupRepository>();
                services.AddScoped<IUserProfileRepository, UserProfileRepository>();
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IUserToGroupMappingRepository, UserToGroupMappingRepository>();

                //Services
                services.AddScoped<IJsonWebTokenService, JsonWebTokenService>();
                services.AddScoped<IPermissionService, PermissionService>();
                services.AddScoped<IRefreshTokenService, RefreshTokenService>();
                services.AddScoped<IUserGroupPermissionValueService, UserGroupPermissionValueService>();
                services.AddScoped<IUserProfileService, UserProfileService>();
                services.AddScoped<IUserService, UserService>();

                //Handlers
                services.AddScoped<IAuthHandler, AuthHandler>();

                //Background Services
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext appDbContext)
        {
            Log.Logger.Debug("Configure!");

            appDbContext.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // app.UseAuthentication();
            // app.UseAuthorization();

            app.UseMiddleware<AuthMiddleware>();

            app.UseCors();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}