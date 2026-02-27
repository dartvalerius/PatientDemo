using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

using PatientDemo.Application;
using PatientDemo.Persistence;
using PatientDemo.WebApi.Configurations.Localization;
using PatientDemo.WebApi.Configurations.Swagger;
using PatientDemo.WebApi.Mappings;
using PatientDemo.WebApi.Middleware;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true; //отключение фильтра ModelState
});

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddAutoMapper(options =>
{
    options.AddProfile(typeof(PatientDTOMappingProfile));
});

#region Swagger

builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConfigureOptions<SwaggerGenOptions>, SwaggerGenConfiguration>();
builder.Services.AddSingleton<IConfigureOptions<SwaggerUIOptions>, SwaggerUIConfiguration>();

#endregion

#region Localization

builder.Services.AddLocalization();
builder.Services.AddSingleton<IConfigureOptions<LocalizationOptions>, LocalizationConfiguration>();
builder.Services.AddSingleton<IConfigureOptions<MvcDataAnnotationsLocalizationOptions>, DataAnnotationsLocalizationConfiguration>();
builder.Services.AddSingleton<IConfigureOptions<RequestLocalizationOptions>, RequestLocalizationConfiguration>();

#endregion

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    });

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseRequestLocalization();

app.UseStaticFiles();

app.UseCors(corsPolicy => corsPolicy
    .WithOrigins("https://localhost:7077", "http://localhost:5036")
    .AllowAnyMethod()
    .AllowAnyHeader());

// Middleware также включаем только для разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
