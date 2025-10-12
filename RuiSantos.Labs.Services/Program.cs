using RuiSantos.Labs.Core;
using RuiSantos.Labs.Data.Dynamodb;
using RuiSantos.Labs.Services.Commands;
using RuiSantos.Labs.Services.Endpoints;

namespace RuiSantos.Labs.Services;

public abstract class Program
{
    public static void Main(string[] args)
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
        
        builder.Services.AddLabsServices();
        builder.Services.AddLabsDynamoDb(builder.Configuration);

        builder.Services.AddTransient<IDoctorsCommandFactory, DoctorsCommandFactory>();
        
        var app = builder.Build();

        app.MapDoctorsEndpoints();

        app.Run();
    }
}