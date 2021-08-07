using System.Collections.Generic;

namespace Odco.PointOfSales.Authentication.External
{
    public interface IExternalAuthConfiguration
    {
        List<ExternalLoginProviderInfo> Providers { get; }
    }
}
