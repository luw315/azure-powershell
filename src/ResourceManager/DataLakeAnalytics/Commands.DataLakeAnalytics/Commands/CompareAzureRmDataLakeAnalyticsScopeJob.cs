using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Commands.DataLakeAnalytics.Models;
using Microsoft.Azure.Management.DataLake.Analytics.Models;

namespace Microsoft.Azure.Commands.DataLakeAnalytics.Commands
{
    [Cmdlet(VerbsData.Compare, "AzureRmDataLakeAnalyticsScopeJob", DefaultParameterSetName = BaseParameterSetName), OutputType(typeof(string))]
    [Alias("Compare-AdlScopeJob")]
    public class CompareAzureRmDataLakeAnalyticsScopeJob : DataLakeAnalyticsCmdletBase
    {
        internal const string BaseParameterSetName = "CompareAdlScopeJob";

        [Parameter(ParameterSetName = BaseParameterSetName, ValueFromPipelineByPropertyName = true, Position = 0,
            Mandatory = true,
            HelpMessage =
                "Name of the Data Lake Analytics account name under which want to retrieve the job information.")]
        [ValidateNotNullOrEmpty]
        [Alias("AccountName")]
        public string Account { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, ParameterSetName = BaseParameterSetName, Position = 1,
            Mandatory = true,
            HelpMessage = "ResourceGroup of Data Lake Analytics account under which the job will be submitted."
            )]
        [ValidateNotNullOrEmpty]
        public string ResourceGroup { get; set; }

        [Parameter(ParameterSetName = BaseParameterSetName, ValueFromPipelineByPropertyName = true, Position = 2,
            ValueFromPipeline = true, Mandatory = true,
            HelpMessage = "ID of the specific job to return job information for.")]
        [ValidateNotNullOrEmpty]
        public Guid JobId1 { get; set; }

        [Parameter(ParameterSetName = BaseParameterSetName, ValueFromPipelineByPropertyName = true, Position = 3,
            ValueFromPipeline = true, Mandatory = true,
            HelpMessage = "ID of the specific job to return job information for.")]
        [ValidateNotNullOrEmpty]
        public Guid JobId2{ get; set; }

        protected override void BeginProcessing()
        {
           // AppDomain.CurrentDomain.AssemblyResolve += BindingRedirect.CurrentDomain_BindingRedirect;
            base.BeginProcessing();
        }

        /// <summary>
        /// stop processing
        /// time-consuming operation should work with ShouldForceQuit
        /// </summary>
        protected override void StopProcessing()
        {
            base.StopProcessing();
        }

        public override void ExecuteCmdlet()
        {
            DataLakeAnalyticsClient.JobExClient.Extension.DiffJobsAsync(Account, ResourceGroup, JobId1, JobId2,
                DataLakeAnalyticsClient.FsClient, DataLakeAnalyticsClient.GetDataLakeAnalyticsAccountManagementClient())
                .GetAwaiter()
                .GetResult(); ;
        }
    }
}
