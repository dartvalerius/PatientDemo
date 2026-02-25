using System.ComponentModel;
using System.Resources;

namespace PatientDemo.Domain.Attributes;

/// <summary>
/// Атрибут для локализации
/// </summary>
/// <param name="resourceKey">Ключ ресурса локализации</param>
/// <param name="resourceType">Тип ресурса локализации</param>
public class LocalizedDescriptionAttribute(string resourceKey, Type resourceType) : DescriptionAttribute
{
    private readonly ResourceManager _resourceManager = new(resourceType);

    public override string Description
    {
        get
        {
            var description = _resourceManager.GetString(resourceKey);
            return string.IsNullOrEmpty(description) ? $"[[{resourceKey}]]" : description;
        }
    }
}