using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using API.Controllers;
using API.Extensions;
using API.Middleware;
using Common.Models;
using DAL.Data;
using DTO.Models.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Xunit;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

                    if (env.IsDevelopment())
                    {
                        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                        config.AddUserSecrets(appAssembly, true);
                    }

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

                        services.AddCustomDbContextInMemory();
                        services.AddRepositories();
                        services.AddServices();
                        services.AddHandlers();
                    })
                    .Configure(app =>
                    {
                        var appDbContext = app.ApplicationServices.GetRequiredService<AppDbContext>();
                        
                        appDbContext.Database.EnsureCreated();

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
        
        var server = host.GetTestServer();
        server.BaseAddress = new Uri("http://localhost:5000/");

        var testHttpClient = server.CreateClient();
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/Auth/test2");
        requestMessage.Content = new StringContent(JsonConvert.SerializeObject(model));
        requestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
        
        var result = await testHttpClient.SendAsync(requestMessage);

        // var content = JsonConvert.SerializeObject(model);
        // var contentBytes = Encoding.UTF8.GetBytes(content);

        // var json = JsonContent.Create(model);
        //
        // HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/Auth/test2")
        // {
        //     Content = json,
        // };
        //
        // postRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
        //
        // var response = await host.GetTestClient().SendAsync(postRequest);

        // var response = await host.GetTestClient().PostAsync("/Auth/signup",
        //     new StringContent(content, Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json));

        // var responseString = await response.Content.ReadAsStringAsync();

        // var context = await server.SendAsync(c =>
        // {
        //     c.Request.Method = HttpMethods.Get;
        //     c.Request.Path = "/Auth/test";
        // });
    }
}