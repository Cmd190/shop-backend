using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.Configuration;
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
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors());

builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi( options =>
    {
        options.TokenValidationParameters.RoleClaimType = "roles";
        options.TokenValidationParameters.ValidAudience = builder.Configuration["AzureAd:Audience"];

        // for username logging
        options.TokenValidationParameters.NameClaimType = "name";
    }, options =>  builder.Configuration.Bind("AzureAd", options));


builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("ReadScope", policy =>
        policy.RequireScope(builder.Configuration["AzureAd:ReadScope"]
                            ?? throw new InvalidConfigurationException("AzureAd:ReadScope not configured")));

    opt.AddPolicy("WriteScope", policy =>
        policy.RequireScope(builder.Configuration["AzureAd:WriteScope"]
                            ?? throw new InvalidConfigurationException("AzureAd:WriteScope not configured")));

    opt.AddPolicy("HasReadRole", policy =>
        policy.RequireRole(builder.Configuration["AzureAd:ReadRole"]
                           ?? throw new InvalidConfigurationException("AzureAd:ReadRole not configured")));

    opt.AddPolicy("HasWriteRole", policy =>
        policy.RequireRole(builder.Configuration["AzureAd:WriteRole"]
                           ?? throw new InvalidConfigurationException("AzureAd:WriteRole not configured")));
});


var app = builder.Build();
app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // TODO remove
    // IdentityModelEventSource.ShowPII = true;
    // IdentityModelEventSource.LogCompleteSecurityArtifact = true;
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
    app.UseSwaggerUi(options => { options.DocumentPath = "/openapi/v1.json"; });
}


if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
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