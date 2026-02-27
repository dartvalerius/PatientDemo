using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace PatientDemo.WebApi.Configurations.Localization;

public class RequestLocalizationConfiguration : IConfigureOptions<RequestLocalizationOptions>
{
    private const string DefaultCulture = "ru";

    public void Configure(RequestLocalizationOptions options)
    {
        var cultureListNames = new List<string> { DefaultCulture, "en" };
        var supportedCultures = cultureListNames.Select(cultureName => new CultureInfo(cultureName)).ToList();

        options.DefaultRequestCulture = new RequestCulture(DefaultCulture);
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;
    }
}