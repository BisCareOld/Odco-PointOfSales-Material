using Abp.AutoMapper;
using Odco.PointOfSales.Authentication.External;

namespace Odco.PointOfSales.Models.TokenAuth
{
    [AutoMapFrom(typeof(ExternalLoginProviderInfo))]
    public class ExternalLoginProviderInfoModel
    {
        public string Name { get; set; }

        public string ClientId { get; set; }
    }
}
