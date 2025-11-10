using System.Net.Mime;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Webshop;
using Webshop.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var serverVersion = new MySqlServerVersion(new Version(8, 0, 43));
var connection =
    $"{builder.Configuration.GetConnectionString("DefaultConnection")}Pwd={Environment.GetEnvironmentVariable("db_pwd")}";
builder.Services.AddDbContext<ProductContext>(dbContextOptions => dbContextOptions
    .UseMySql(connection, serverVersion)
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors());

builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi( options =>
    {
        builder.Configuration.Bind("AzureAd");
        options.TokenValidationParameters.RoleClaimType = "groups";

        // for username logging
        options.TokenValidationParameters.NameClaimType = "name";
    }, options =>  builder.Configuration.Bind("AzureAd", options));


builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("ReadGroup", policy =>
        policy.RequireClaim("groups", "8c6d10cb-472e-4714-9f44-27ee188aaedd"));

    opt.AddPolicy("WriteGroup", policy =>
        policy.RequireClaim("groups", "991a4e8b-5aa5-470e-920d-0e36701a5f5d"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
    app.UseSwaggerUi(options => { options.DocumentPath = "/openapi/v1.json"; });
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            // using static System.Net.Mime.MediaTypeNames;
            context.Response.ContentType = MediaTypeNames.Text.Plain;

            await context.Response.WriteAsync("An exception was thrown.");

            var exceptionHandlerPathFeature =
                context.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
            {
                await context.Response.WriteAsync(" The file was not found.");
            }

            if (exceptionHandlerPathFeature?.Path == "/")
            {
                await context.Response.WriteAsync(" Page: Home.");
            }
        });
    });
}

app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();