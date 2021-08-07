using Abp.MultiTenancy;
using Odco.PointOfSales.Authorization.Users;

namespace Odco.PointOfSales.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {            
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}
