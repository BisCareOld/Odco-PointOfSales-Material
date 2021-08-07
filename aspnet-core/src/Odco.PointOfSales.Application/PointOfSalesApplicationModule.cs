using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Odco.PointOfSales.Authorization;

namespace Odco.PointOfSales
{
    [DependsOn(
        typeof(PointOfSalesCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class PointOfSalesApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<PointOfSalesAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(PointOfSalesApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
