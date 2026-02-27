using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace PatientDemo.WebApi.Configurations.Swagger;

public class SwaggerUIConfiguration : IConfigureOptions<SwaggerUIOptions>
{
    public void Configure(SwaggerUIOptions options)
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Version 1");
        options.RoutePrefix = string.Empty;
        options.DisplayRequestDuration();
    }
}