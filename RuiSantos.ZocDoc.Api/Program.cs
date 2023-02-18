using Microsoft.OpenApi.Models;
using System.Reflection;
using RuiSantos.ZocDoc.Data.Mongodb;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging(config => config.AddConsole());
builder.Services.AddDataContext(builder.Configuration.GetSection("Database").Get<MongoSettings>());
builder.Services.AddControllers();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ZocDoc from Rui Santos",
        Description = "A simple ZocDoc implementation by Rui Santos"
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();