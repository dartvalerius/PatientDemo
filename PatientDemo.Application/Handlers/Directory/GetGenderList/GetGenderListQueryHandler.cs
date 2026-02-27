using MediatR;
using PatientDemo.Application.Common.Extensions;
using PatientDemo.Domain.Enums;
using PatientDemo.Shared.DTO.Responses;

namespace PatientDemo.Application.Handlers.Directory.GetGenderList;

/// <summary>
/// Обработчик запроса на получение списка значений для пола пациента
/// </summary>
public class GetGenderListQueryHandler : IRequestHandler<GetGenderListQuery, IEnumerable<EnumVm>>
{
    public Task<IEnumerable<EnumVm>> Handle(GetGenderListQuery request, CancellationToken cancellationToken)
    {
        var list = Enum
            .GetValues(typeof(Gender))
            .Cast<Gender>()
            .Select(x => new EnumVm
            {
                Value = x.ToString(),
                Description = x.GetLocalizedDescription()!
            });

        return Task.FromResult(list);
    }
}