using System.Reflection;

using Microsoft.EntityFrameworkCore;

using PatientDemo.Application.Interfaces;
using PatientDemo.Domain.Entities;

namespace PatientDemo.Persistence.Data;

public class PatientDemoDbContext(DbContextOptions<PatientDemoDbContext> options) : DbContext(options), IPatientDemoDbContext
{
    public DbSet<Patient> Patients { get; set; }
    public DbSet<HumanName> HumanNames { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}