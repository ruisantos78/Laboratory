using System.Reflection;
using Microsoft.OpenApi.Models;
using RuiSantos.Labs.Api.Core;
using RuiSantos.Labs.Core;
using RuiSantos.Labs.Data.Dynamodb;
using RuiSantos.Labs.GraphQL;

namespace RuiSantos.Labs.Api;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(config => config.AddConsole());

        services.AddMemoryCache();
        services.AddControllers();
        services.AddHealthChecks();
        
        services.AddLabsServices();
        services.AddLabsGraphql();
        services.AddLabsDynamoDb(Configuration);

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


        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins(Configuration["LABS_ALLOWED_ORIGINS"] ?? string.Empty)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.yaml", "Rui Santos Laboratory v1"));
        }

        app.UseCors();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGraphQL();
            endpoints.MapHealthChecks("/health");
        });
    }
}

