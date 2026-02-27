using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PatientDemo.WebApi.Configurations.Swagger;

public class SwaggerGenConfiguration : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1",
            new OpenApiInfo
            {
                Version = "Version 1",
                Title = "PatientDemo v1",
                Description = "Результат тестового задания",
                Contact = new OpenApiContact
                {
                    Email = "v.filippenkov@defin.by",
                    Name = "Филиппенков Валерий",
                    Url = new Uri("https://t.me/dartvalerius")
                }
            });

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), includeControllerXmlComments: true);
    }
}