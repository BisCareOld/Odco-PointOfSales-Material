using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using Odco.PointOfSales.Configuration.Dto;

namespace Odco.PointOfSales.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : PointOfSalesAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
