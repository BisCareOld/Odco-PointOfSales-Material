using Abp.Application.Services;
using Odco.PointOfSales.MultiTenancy.Dto;

namespace Odco.PointOfSales.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

