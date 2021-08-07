using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Odco.PointOfSales.EntityFrameworkCore;
using Odco.PointOfSales.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Odco.PointOfSales.Web.Tests
{
    [DependsOn(
        typeof(PointOfSalesWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class PointOfSalesWebTestModule : AbpModule
    {
        public PointOfSalesWebTestModule(PointOfSalesEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(PointOfSalesWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(PointOfSalesWebMvcModule).Assembly);
        }
    }
}