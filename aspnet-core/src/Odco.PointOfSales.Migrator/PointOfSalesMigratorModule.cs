using Microsoft.Extensions.Configuration;
using Castle.MicroKernel.Registration;
using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Odco.PointOfSales.Configuration;
using Odco.PointOfSales.EntityFrameworkCore;
using Odco.PointOfSales.Migrator.DependencyInjection;

namespace Odco.PointOfSales.Migrator
{
    [DependsOn(typeof(PointOfSalesEntityFrameworkModule))]
    public class PointOfSalesMigratorModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public PointOfSalesMigratorModule(PointOfSalesEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

            _appConfiguration = AppConfigurations.Get(
                typeof(PointOfSalesMigratorModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                PointOfSalesConsts.ConnectionStringName
            );

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(
                typeof(IEventBus), 
                () => IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(PointOfSalesMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}
