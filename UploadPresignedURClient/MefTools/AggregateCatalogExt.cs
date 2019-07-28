using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bolareshet.UploadPresignedURClient.MefTools;

namespace System.ComponentModel.Composition.Hosting
{
    public static class AggregateCatalogExt
    {
        public static void AddMoudlesFromAppConfig(this AggregateCatalog aggregator, string configPath)
        {
            if (string.IsNullOrEmpty(configPath))
                throw new ArgumentNullException("configPath");

            IDictionary<string, string> assemblyList = ConvertConfigurationSection.ConvertSection<string, string>(configPath);

            if (assemblyList != null)
            {
                foreach (var codeBase in assemblyList.Values)
                {
                    aggregator.Catalogs.Add(new AssemblyCatalog(codeBase));
                }
            }
        }
    }
}
