using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Options;
using TimeTracker.WebApi;

namespace PatientDemo.WebApi.Configurations.Localization;

public class DataAnnotationsLocalizationConfiguration : IConfigureOptions<MvcDataAnnotationsLocalizationOptions>
{
    public void Configure(MvcDataAnnotationsLocalizationOptions options)
    {
        options.DataAnnotationLocalizerProvider = (_, factory) =>
            factory.Create(typeof(ErrorMessage));
    }
}