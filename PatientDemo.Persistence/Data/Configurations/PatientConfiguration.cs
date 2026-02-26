using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientDemo.Domain.Entities;
using PatientDemo.Domain.Enums;

namespace PatientDemo.Persistence.Data.Configurations;

/// <summary>
/// Конфигурация данных пациента
/// </summary>
public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder
            .ToTable("PATIENTS");

        builder
            .HasOne(p => p.Name)
            .WithOne(hn => hn.Patient)
            .HasForeignKey<HumanName>(hn => hn.PatientId);

        builder
            .HasKey(x => x.Id);

        builder
            .HasIndex(x => x.Id)
            .IsUnique();

        builder
            .HasIndex(x => x.BirthDate);

        builder
            .Property(x => x.Id)
            .HasColumnName("ID")
            .HasColumnType("uuid")
            .HasColumnOrder(0)
            .IsRequired();

        builder
            .Property(x => x.Gender)
            .HasColumnName("GENDER")
            .HasColumnType("varchar(10)")
            .HasColumnOrder(1)
            .HasConversion<string>()
            .HasDefaultValue(Gender.Unknown);

        builder
            .Property(x => x.BirthDate)
            .HasColumnName("BIRTH_DATE")
            .HasColumnType("date")
            .HasColumnOrder(2)
            .HasConversion(
                d => d.ToUniversalTime(),
                d => DateTime.SpecifyKind(d, DateTimeKind.Utc))
            .IsRequired();

        builder
            .Property(x => x.Active)
            .HasColumnName("ACTIVE")
            .HasColumnType("boolean")
            .HasColumnOrder(3)
            .HasDefaultValue(true);
    }
}