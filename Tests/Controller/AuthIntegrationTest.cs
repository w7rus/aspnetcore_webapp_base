using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;
using API.Controllers;
using API.Extensions;
using API.Middleware;
using Common.Models;
using DAL.Data;
using DTO.Models.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Tests.Handlers;

public class AuthIntegrationTest
{
    [Fact]
    public async Task AuthSignUp()
    {
        using var host = await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder.UseTestServer();

                webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.Local.json", optional: true, reloadOnChange: true);

                    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                    config.AddUserSecrets(appAssembly, true);

                    config.AddEnvironmentVariables();
                });

                webBuilder.ConfigureServices((webHostBuilder, services) =>
                    {
                        services.AddCustomConfigureOptions();

                        services.AddCustomOptions(webHostBuilder.Configuration);

                        services.AddControllers();
                        services.AddMvc().AddApplicationPart(typeof(AuthController).Assembly);

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
                        services.AddSingleton<IObjectModelValidator, CustomObjectModelValidator>();

                        services.AddDbContextTest(webHostBuilder.Configuration);
                        services.AddRepositories();
                        services.AddServices();
                        services.AddHandlers();
                    })
                    .Configure(app =>
                    {
                        var appDbContext = app.ApplicationServices.GetRequiredService<AppDbContext>();

                        appDbContext.Database.EnsureDeleted();
                        appDbContext.Database.Migrate();

                        // var hostApplicationLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
                        // hostApplicationLifetime.ApplicationStopping.Register(() =>
                        // {
                        //     appDbContext.Database.CloseConnection();
                        // });

                        app.UseDeveloperExceptionPage();

                        app.UseRouting();

                        app.UseMiddleware<AuthMiddleware>();

                        app.UseCors();

                        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
                    });
            })
            .StartAsync();

        var model = new AuthSignUp
        {
            Email = "test123@email.com",
            Password = "12345678",
            Username = "test123"
        };

        var model2 = new AuthSignUp
        {
            Email = "test123@email.com",
            Password = "12345678",
            Username = "test123"
        };

        var json = JsonContent.Create(model);
        var json2 = JsonContent.Create(model2);

        var server = host.GetTestServer();

        var testHttpClient = server.CreateClient();
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/Auth/signup")
        {
            Content = json
        };
        var requestMessage2 = new HttpRequestMessage(HttpMethod.Post, "/Auth/signup")
        {
            Content = json2
        };

        var result = await testHttpClient.SendAsync(requestMessage);
        var result2 = await testHttpClient.SendAsync(requestMessage2);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, result2.StatusCode);
    }
}