using Microsoft.EntityFrameworkCore;
using PatientDemo.Domain.Entities;

namespace PatientDemo.Application.Interfaces;

/// <summary>
/// Интерфейс контекста базы данных
/// </summary>
public interface IPatientDemoDbContext
{
    /// <summary>
    /// Пациенты
    /// </summary>
    DbSet<Patient> Patients { get; set; }

    /// <summary>
    /// Имена пациентов
    /// </summary>
    DbSet<HumanName> HumanNames { get; set; }

    /// <summary>
    /// Сохранение изменений в БД
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Количество отслеживаемых объектов</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}