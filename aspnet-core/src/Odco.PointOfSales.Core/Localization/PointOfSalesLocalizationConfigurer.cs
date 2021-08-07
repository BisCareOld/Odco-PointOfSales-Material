using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace Odco.PointOfSales.Localization
{
    public static class PointOfSalesLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(PointOfSalesConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(PointOfSalesLocalizationConfigurer).GetAssembly(),
                        "Odco.PointOfSales.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
