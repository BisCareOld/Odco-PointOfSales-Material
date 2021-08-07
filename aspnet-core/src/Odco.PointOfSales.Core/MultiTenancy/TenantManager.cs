using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using Odco.PointOfSales.Authorization.Users;
using Odco.PointOfSales.Editions;

namespace Odco.PointOfSales.MultiTenancy
{
    public class TenantManager : AbpTenantManager<Tenant, User>
    {
        public TenantManager(
            IRepository<Tenant> tenantRepository, 
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository, 
            EditionManager editionManager,
            IAbpZeroFeatureValueStore featureValueStore) 
            : base(
                tenantRepository, 
                tenantFeatureRepository, 
                editionManager,
                featureValueStore)
        {
        }
    }
}
