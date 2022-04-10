using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using API.AuthHandlers;
using API.Extensions;
using API.Middleware;
using BLL.Jobs;
using BLL.Maps;
using BLL.Services;
using Common.Enums;
using Common.Filters;
using Common.Helpers;
using Common.Models;
using Common.Options;
using DAL.Data;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using AuthenticationSchemes = Common.Models.AuthenticationSchemes;

namespace API
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddCustomLogging(_env, Configuration);

            services.AddCustomConfigureOptions();

            services.AddCustomOptions(Configuration);

            services.AddControllers(options => { options.Filters.Add<HttpResponseExceptionFilter>(); });

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
                        TraceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier
                    };

                    foreach (var modelError in context.ModelState.Values.SelectMany(modelStateValue =>
                                 modelStateValue.Errors))
                        errorModelResult.Errors.Add(new ErrorModelResultEntry(ErrorType.ModelState,
                            modelError.ErrorMessage));

                    return new BadRequestObjectResult(errorModelResult);
                };
            });

            // Disable default object model validator
            // services.AddSingleton<IObjectModelValidator, CustomObjectModelValidator>();

            //Authentication
            {
                services.AddAuthentication()
                    .AddScheme<DefaultAuthenticationSchemeOptions, DefaultAuthenticationHandler>(
                        AuthenticationSchemes.Default, null!);
                services.AddAuthentication()
                    .AddScheme<JsonWebTokenAuthenticationSchemeOptions, JsonWebTokenAuthenticationHandler>(
                        AuthenticationSchemes.JsonWebToken, null!);
                services.AddAuthentication()
                    .AddScheme<JsonWebTokenAuthenticationSchemeOptions, JsonWebTokenExpiredAuthenticationHandler>(
                        AuthenticationSchemes.JsonWebTokenExpired, null!);
            }

            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddSignalR();

            services.AddCustomDbContext(Configuration);
            services.AddRepositories();
            services.AddServices();
            services.AddAdvancedServices();
            services.AddHandlers();
            // services.AddBackgroundServices();

            services.AddCustomHangfire(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            AppDbContext appDbContext,
            IBackgroundJobClient backgroundJobClient,
            IRecurringJobManager recurringJobManager,
            IHostApplicationLifetime hostApplicationLifetime
        )
        {
            Log.Logger.Information("Startup.Configure()");
            Log.Logger.Information($"EnvironmentName: {env.EnvironmentName}");

            if (env.IsDevelopment())
            {
                // app.UseDeveloperExceptionPage();

                Log.Logger.Information($"Add Swagger & SwaggerUI");
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));

                Log.Logger.Information($"Add HangfireDashboard");
                app.UseHangfireDashboard();
            }
            else
            {
                // app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate =
                    "[{httpContextTraceIdentifier}] {httpContextRequestProtocol} {httpContextRequestMethod} {httpContextRequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("httpContextTraceIdentifier",
                        Activity.Current?.Id ?? httpContext.TraceIdentifier);
                    diagnosticContext.Set("httpContextConnectionId", httpContext.Connection.Id);
                    diagnosticContext.Set("httpContextConnectionRemoteIpAddress",
                        httpContext.Connection.RemoteIpAddress);
                    diagnosticContext.Set("httpContextConnectionRemotePort", httpContext.Connection.RemotePort);
                    diagnosticContext.Set("httpContextRequestHost", httpContext.Request.Host);
                    diagnosticContext.Set("httpContextRequestPath", httpContext.Request.Path);
                    diagnosticContext.Set("httpContextRequestProtocol", httpContext.Request.Protocol);
                    diagnosticContext.Set("httpContextRequestIsHttps", httpContext.Request.IsHttps);
                    diagnosticContext.Set("httpContextRequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("httpContextRequestMethod", httpContext.Request.Method);
                    diagnosticContext.Set("httpContextRequestContentType", httpContext.Request.ContentType);
                    diagnosticContext.Set("httpContextRequestContentLength", httpContext.Request.ContentLength);
                    diagnosticContext.Set("httpContextRequestQueryString", httpContext.Request.QueryString);
                    diagnosticContext.Set("httpContextRequestQuery", httpContext.Request.Query);
                    diagnosticContext.Set("httpContextRequestHeaders", httpContext.Request.Headers);
                    diagnosticContext.Set("httpContextRequestCookies", httpContext.Request.Cookies);
                };
            });

            app.UseExceptionHandler("/Error");

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors();

            app.UseWebSockets();

            app.UseMiddleware<LastActivityMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });

            Log.Logger.Information($"Add/Update Recurring Jobs for Hangfire");
            
            recurringJobManager.AddOrUpdate<IJsonWebTokenJobs>(RecurringJobId.JsonWebTokenPurge,
                _ => _.PurgeAsync(hostApplicationLifetime.ApplicationStopping), Cron.Minutely);
            
            recurringJobManager.AddOrUpdate<IUserJobs>(RecurringJobId.UsersPurge,
                _ => _.PurgeAsync(hostApplicationLifetime.ApplicationStopping), Cron.Daily);
        }
    }
}