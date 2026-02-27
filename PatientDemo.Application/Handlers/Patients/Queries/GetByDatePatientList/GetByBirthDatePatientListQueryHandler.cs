using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PatientDemo.Application.Interfaces;
using PatientDemo.Shared.DTO.Responses;

namespace PatientDemo.Application.Handlers.Patients.Queries.GetByDatePatientList;

/// <summary>
/// Обработчик запрос на получение пациентов по дате рождения
/// </summary>
public class GetByBirthDatePatientListQueryHandler(IPatientDemoDbContext dbContext, IMapper mapper) : IRequestHandler<GetByBirthDatePatientListQuery, IEnumerable<PatientVm>>
{
    public async Task<IEnumerable<PatientVm>> Handle(GetByBirthDatePatientListQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Patients.Include(x => x.Name).AsQueryable();

        if (request.DateFrom.HasValue)
            query = query.Where(x => x.BirthDate >= request.DateFrom);

        if (request.DateTo.HasValue)
            query = query.Where(x => x.BirthDate <= request.DateTo);

        return await query
            .ProjectTo<PatientVm>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken: cancellationToken);
    }
}