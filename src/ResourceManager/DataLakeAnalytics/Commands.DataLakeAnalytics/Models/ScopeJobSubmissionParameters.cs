


using System.Collections.Generic;
using Microsoft.Azure.Management.DataLake.Analytics;
using Microsoft.Azure.Management.DataLake.Analytics.Scope;

namespace Microsoft.Azure.Commands.DataLakeAnalytics.Models
{
    public class ScopeJobSubmissionParameters
    {
        public string ScriptPath;
        public string FriendlyName;
        public int Priority = 1000;
        public string SdkVersion;
        public Dictionary<string,string> Parameters;
        public string[] Notify;
        public NebulaArguments NebulaArguments;
      //  public VirtualClusterRef Vc;
        public Dictionary<string, string> CustomProperties;
    //    public TokenAllocation Tokens;
        public int DegreeOfParallelism;
        public ScopeEnvironmentParameters EnvironmentParameters;

        /// <summary>
        /// AdlaName
        /// </summary>
        public string AdlaName { set; get; }

        /// <summary>
        /// ResourceGroup
        /// </summary>
        public string ResourceGroup { get; set; }

        /// <summary>
        /// DataLakeAnalyticsJobManagementExtensionClient
        /// </summary>
        public DataLakeAnalyticsJobManagementExtensionClient JobExClient { get; set; }

        /// <summary>
        /// DataLakeAnalyticsAccountManagementClient
        /// </summary>
        public DataLakeAnalyticsAccountManagementClient AdlaClient { get; set; }

        public ScopeJobSubmissionParameters()
        {
            this.EnvironmentParameters = new ScopeEnvironmentParameters();
            this.NebulaArguments = new NebulaArguments(null);
        }
    }
}