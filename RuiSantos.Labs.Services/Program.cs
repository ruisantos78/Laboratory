using RuiSantos.Labs.Core;
using RuiSantos.Labs.Repository.DynamoDb;
using RuiSantos.Labs.Services.Commands;
using RuiSantos.Labs.Services.Endpoints;

namespace RuiSantos.Labs.Services;

public abstract class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policyBuilder =>
            {
                policyBuilder.WithOrigins(builder.Configuration["LABS_ALLOWED_ORIGINS"] ?? string.Empty)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        
        builder.Services.AddLabsDynamoDb(builder.Configuration);

        builder.Services.AddTransient<IDoctorsCommandFactory, DoctorsCommandFactory>();
        
        var app = builder.Build();
        
        app.MapDoctorsEndpoints();

        if (app.Environment.IsDevelopment())
        {
            await app.EnsureTablesExistAsync();
        }
        
        await app.RunAsync();
    }
}