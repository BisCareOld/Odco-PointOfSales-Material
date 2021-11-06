using System.IO;
using System.Reflection;

namespace Odco.PointOfSales.StoredProcedures
{
    public class StoredProcedureHelper
    {
        public string GetSP(string fileName)
        {
            string namespacesWithFileName = "Odco.PointOfSales.StoredProcedures." + fileName;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(namespacesWithFileName))
            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
