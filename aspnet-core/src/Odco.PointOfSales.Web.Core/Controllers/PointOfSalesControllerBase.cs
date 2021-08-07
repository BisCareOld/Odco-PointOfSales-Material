using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace Odco.PointOfSales.Controllers
{
    public abstract class PointOfSalesControllerBase: AbpController
    {
        protected PointOfSalesControllerBase()
        {
            LocalizationSourceName = PointOfSalesConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
