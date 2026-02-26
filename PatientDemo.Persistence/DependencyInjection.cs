using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using PatientDemo.Application.Interfaces;
using PatientDemo.Persistence.Data;

namespace PatientDemo.Persistence;

/// <summary>
/// Класс с методами расширения внедрения зависимостей
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PatientDemoDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DbConnection")));

        services.AddScoped<IPatientDemoDbContext, PatientDemoDbContext>();

        return services;
    }
}