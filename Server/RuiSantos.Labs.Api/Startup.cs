using System.Reflection;
using Microsoft.OpenApi.Models;
using RuiSantos.Labs.Api.Core;

namespace RuiSantos.Labs.Api;

public class Startup
{
	public IConfiguration Configuration { get; }

	public Startup(IConfiguration configuration)
	{
		this.Configuration = configuration;
	}

	public void ConfigureServices(IServiceCollection services)
	{
        services.AddLogging(config => config.AddConsole());

        services.AddMemoryCache();
        services.AddControllers();
        services.AddHealthChecks();

        services.AddLabsGraphQL();
        services.AddLabsServices();
        services.AddLabsDynamoDb();

        services.AddSwaggerGen(options =>
        {
            options.OperationFilter<ReApplyOptionalRouteParameterOperationFilter>();

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Rui Santos Laboratory",
                Description = "A simple laboratory implementation by Rui Santos"
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.yaml", "Rui Santos Laboratory v1"));
        }

        app.UseRouting();
        
        app.UseAuthorization();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGraphQL();  // Make sure you have a method like this in your services
            endpoints.MapHealthChecks("/health");
        });
    }
}

