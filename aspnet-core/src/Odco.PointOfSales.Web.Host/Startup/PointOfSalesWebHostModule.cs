using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Odco.PointOfSales.Configuration;

namespace Odco.PointOfSales.Web.Host.Startup
{
    [DependsOn(
       typeof(PointOfSalesWebCoreModule))]
    public class PointOfSalesWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public PointOfSalesWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(PointOfSalesWebHostModule).GetAssembly());
        }
    }
}
