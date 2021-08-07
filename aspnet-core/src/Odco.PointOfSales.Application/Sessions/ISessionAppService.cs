using System.Threading.Tasks;
using Abp.Application.Services;
using Odco.PointOfSales.Sessions.Dto;

namespace Odco.PointOfSales.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
