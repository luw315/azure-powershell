

namespace Microsoft.Azure.Management.DataLake.Analytics.Scope.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Rest.Azure;

    internal static class DataLakeAnalyticsCustomizationHelper
    {
        /// <summary>
        /// This constant is used as the default package version to place in the user agent.
        /// It should mirror the package version in the project.json file.
        /// </summary>
        internal const string PackageVersion = "0.0.1";

        internal const string DefaultAdlaDnsSuffix = "azuredatalakeanalytics.net";

        /// <summary>
        /// Get the assembly version of a service client.
        /// </summary>
        /// <returns>The assembly version of the client.</returns>        
        internal static void UpdateUserAgentAssemblyVersion(IAzureClient clientToUpdate, string assemblyVersionToUse)
        {
            var type = clientToUpdate.GetType();

            var newVersion = string.IsNullOrEmpty(assemblyVersionToUse) ?
                PackageVersion : assemblyVersionToUse;

            foreach (
                var info in
                    clientToUpdate.HttpClient.DefaultRequestHeaders.UserAgent.Where(
                        info => info.Product.Name.Equals(type.FullName, StringComparison.OrdinalIgnoreCase)))
            {
                clientToUpdate.HttpClient.DefaultRequestHeaders.UserAgent.Remove(info);
                clientToUpdate.HttpClient.DefaultRequestHeaders.UserAgent.Add(
                    new System.Net.Http.Headers.ProductInfoHeaderValue(type.FullName, newVersion));
                break;
            }

        }
    }
}
