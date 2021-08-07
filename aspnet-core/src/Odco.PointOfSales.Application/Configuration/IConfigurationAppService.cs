using System.Threading.Tasks;
using Odco.PointOfSales.Configuration.Dto;

namespace Odco.PointOfSales.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
