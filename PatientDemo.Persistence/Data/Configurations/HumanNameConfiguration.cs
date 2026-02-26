using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientDemo.Domain.Entities;

namespace PatientDemo.Persistence.Data.Configurations;

/// <summary>
/// Конфигурация данных имени пациента
/// </summary>
public class HumanNameConfiguration : IEntityTypeConfiguration<HumanName>
{
    public void Configure(EntityTypeBuilder<HumanName> builder)
    {
        builder
            .ToTable("HUMAN_NAMES");

        builder
            .HasKey(x => x.Id);

        builder
            .HasIndex(x => x.Id)
            .IsUnique();

        builder
            .Property(x => x.Id)
            .HasColumnName("ID")
            .HasColumnType("uuid")
            .HasColumnOrder(0)
            .IsRequired();

        builder
            .Property(x => x.Use)
            .HasColumnName("USE")
            .HasColumnType("varchar(20)")
            .HasColumnOrder(1)
            .HasDefaultValue("official");

        builder
            .Property(x => x.Family)
            .HasColumnName("FAMILY")
            .HasColumnType("varchar(100)")
            .HasColumnOrder(2)
            .IsRequired();

        builder
            .Property(x => x.Given)
            .HasColumnName("GIVEN")
            .HasColumnType("varchar(30)[]")
            .HasColumnOrder(3)
            .HasDefaultValue(Array.Empty<string>());
    }
}