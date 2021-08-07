using System.Threading.Tasks;
using Abp.Application.Services;
using Odco.PointOfSales.Authorization.Accounts.Dto;

namespace Odco.PointOfSales.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
