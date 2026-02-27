using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace PatientDemo.WebApi.Configurations.Localization;

public class LocalizationConfiguration : IConfigureOptions<LocalizationOptions>
{
    private const string ResourcesPath = "Resources"; // ресурсы локализации и прочего

    public void Configure(LocalizationOptions options)
    {
        options.ResourcesPath = ResourcesPath;
    }
}