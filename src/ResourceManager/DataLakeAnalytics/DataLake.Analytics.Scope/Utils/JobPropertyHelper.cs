

namespace Microsoft.Azure.Management.DataLake.Analytics.Scope.Utils
{
    using System;
    using Microsoft.Azure.Management.DataLake.Analytics.Models;

    internal static class JobInformationExtensions
    {
        internal static string GetAlgebraFilePath(this JobInformation jobInformation)
        {
            var path = string.Empty;
            switch (jobInformation.Type)
            {
                case JobType.USql:
                    var uSqlJobProperties = jobInformation.Properties as USqlJobProperties;
                    if (uSqlJobProperties != null)
                    {
                        path = uSqlJobProperties.AlgebraFilePath;
                    }
                    break;
                case JobType.Scope:
                    var scopeJobProperties = jobInformation.Properties as ScopeJobProperties;
                    if (scopeJobProperties != null)
                    {
                        //TODO: SDK will add this property 
                        path = scopeJobProperties.UserAlgebraPath;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException("AlgebraFilePath");
            }
            return path;
        }
    }
}
