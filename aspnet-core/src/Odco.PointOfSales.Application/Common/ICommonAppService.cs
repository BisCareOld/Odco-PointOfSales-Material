using Abp.Application.Services;
using Odco.PointOfSales.Application.GeneralDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Odco.PointOfSales.Application.Common
{
    public interface ICommonAppService : IApplicationService
    {
        Task<List<CommonKeyValuePairDto>> GetAllBanksAsync();
    }
}
