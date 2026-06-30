using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using SomniPedia.Repository;
using SomniPedia.Repository.Repositories;
using SomniPedia.Core.Interfaces;
using SomniPedia.Core.Configuration;
using SomniPedia.Services;
using SomniPedia.Api.Middlewares;
using MongoDB.Driver;
using System.Reflection;

namespace SomniPedia.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Central Exception Handling
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            // Add services to the container.
            builder.Services.AddControllers();
            
            // Add OpenAPI/Swagger Specification
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if(File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
                
                var coreXmlFile = "SomniPedia.Core.xml";
                var coreXmlPath = Path.Combine(AppContext.BaseDirectory, coreXmlFile);
                if(File.Exists(coreXmlPath)) c.IncludeXmlComments(coreXmlPath);
            });

            // Strongly Typed Options Pattern
            builder.Services.Configure<MongoDbSettings>(
                builder.Configuration.GetSection(MongoDbSettings.SectionName));

            // MongoDB Setup using IOptions
            builder.Services.AddDbContext<SomniPediaDbContext>((serviceProvider, options) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                var mongoClient = new MongoClient(settings.ConnectionString);
                options.UseMongoDB(mongoClient, settings.DatabaseName);
            });

            // DI Registrations
            builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
            builder.Services.AddScoped<IArticleService, ArticleService>();

            var app = builder.Build();

            // Use Central Exception Handling
            app.UseExceptionHandler();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
