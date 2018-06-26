namespace Microsoft.Azure.Management.DataLake.InternalAnalytics.Scope.Models
{
    using System.Collections.Specialized;
    using Microsoft.Azure.Management.DataLake.Analytics.Models;

    internal class JobDiffInfo
    {
        public JobInformation JobInfo { get; set; }

        public JobStatistics JobStatistics { get; set; }

        public string RuntimeVersion { get; set; }

        public long TotalInputDataSize { get; set; }
    }
}
