using System;
using System.Collections.Generic;
using System.Linq;
using API.Extensions;
using API.Middleware;
using Common.Helpers;
using Common.Models;
using DAL.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;

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
            services.AddCustomLogging(Log.Logger, _env);

            services.AddCustomConfigureOptions();

            services.AddCustomOptions(Configuration);

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
                // //TODO: Probably useless since there is AuthorizeJsonWebToken & AuthorizeExpiredJsonWebToken attributes
                // services.AddAuthentication()
                //     .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, null!);
            }

            services.AddCustomDbContext(Configuration);
            services.AddRepositories();
            services.AddServices();
            services.AddHandlers();
            services.AddBackgroundServices();
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
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            // app.UseAuthentication();
            // app.UseAuthorization();

            app.UseMiddleware<AuthMiddleware>();

            app.UseCors();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}