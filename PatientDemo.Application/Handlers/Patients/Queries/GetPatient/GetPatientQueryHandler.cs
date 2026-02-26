using AutoMapper;
using MediatR;

using Microsoft.EntityFrameworkCore;

using PatientDemo.Application.Exceptions;
using PatientDemo.Application.Handlers.Patients.Resources;
using PatientDemo.Application.Interfaces;
using PatientDemo.Shared.DTO.Responses;

namespace PatientDemo.Application.Handlers.Patients.Queries.GetPatient;

/// <summary>
/// Обработчик запроса на получение пациента
/// </summary>
public class GetPatientQueryHandler(IPatientDemoDbContext dbContext, IMapper mapper) : IRequestHandler<GetPatientQuery, PatientVm>
{
    public async Task<PatientVm> Handle(GetPatientQuery request, CancellationToken cancellationToken)
    {
        var patientEntity =
            await dbContext.Patients.Include(x => x.Name)
                .SingleOrDefaultAsync(x => x.Id.Equals(request.Id), cancellationToken) ??
            throw new NotFoundException(PatientResources.NotFound);

        return mapper.Map<PatientVm>(patientEntity);
    }
}