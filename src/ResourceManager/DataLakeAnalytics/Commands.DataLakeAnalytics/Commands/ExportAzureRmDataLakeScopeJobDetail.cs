using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Azure.Commands.DataLakeAnalytics.Models;
using Microsoft.Azure.Management.DataLake.Analytics;
using Microsoft.Azure.Management.DataLake.Analytics.Models;

namespace Microsoft.Azure.Commands.DataLakeAnalytics.Commands
{
    [Cmdlet(VerbsData.Export, "AzureRmDataLakeScopeJobDetail", SupportsShouldProcess = true, DefaultParameterSetName = BaseParameterSetName), OutputType(typeof(string))]
    [Alias("Export-AdlScopeJob")]
    public class ExportAzureRmDataLakeScopeJobDetail : DataLakeAnalyticsCmdletBase
    {
        internal const string BaseParameterSetName = "AllInResourceGroupAndAccount";

        [Parameter(ParameterSetName = BaseParameterSetName, ValueFromPipelineByPropertyName = true, Position = 0,
            Mandatory = true,
            HelpMessage =
                "Name of the Data Lake Analytics account name under which want to retrieve the job information.")]
        [ValidateNotNullOrEmpty]
        [Alias("AccountName")]
        public string Account { get; set; }

        [Parameter(ParameterSetName = BaseParameterSetName, ValueFromPipelineByPropertyName = true, Position = 1,
            ValueFromPipeline = true, Mandatory = true,
            HelpMessage = "ID of the specific job to return job information for.")]
        [ValidateNotNullOrEmpty]
        public Guid JobId { get; set; }

        [Parameter(ParameterSetName = BaseParameterSetName, ValueFromPipelineByPropertyName = true, Position = 2,
            ValueFromPipeline = true, Mandatory = true,
            HelpMessage = "TargetFolder.")]
        [ValidateNotNullOrEmpty]
        public string TargetFolder { get; set; }

        protected override void BeginProcessing()
        {
            //AppDomain.CurrentDomain.AssemblyResolve += BindingRedirect.CurrentDomain_BindingRedirect;
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
            ThrowErrorIfDirectoryDoesNotExist(TargetFolder);

            var jobinfo = DataLakeAnalyticsClient.JobExClient.BaseClient.Job.Get(Account, JobId);

            // Create the folder that will contain the downloaded job files
            string root_target_folder = Path.Combine(TargetFolder, JobId.ToString("N"));

            CreateDirectoryIfNeeded(root_target_folder);

            var scopeJobProperties = jobinfo.Properties as ScopeJobProperties;
            if (scopeJobProperties != null)
            {
                foreach (var resource in scopeJobProperties.Resources)
                {
                    try
                    {
                        WriteVerbose(string.Format("Downloading {0}", resource.Name));
                        var uri = new Uri(resource.Path);
                        var adlsAccount = uri.Host.Split(new char[] {'.'}, StringSplitOptions.RemoveEmptyEntries)[0];
                        if (adlsAccount == null) throw new ArgumentNullException(nameof(adlsAccount));
                        var path = uri.PathAndQuery;
                        string destFilename = Path.Combine(root_target_folder, Path.GetFileName(path));
                        DataLakeAnalyticsClient.FsClient.FileSystem.DownloadFile(adlsAccount, path, destFilename);
                    }
                    catch (Exception e)
                    {
                        WriteVerbose(e.ToString());
                    }
                }
            }

            var uSqlJobProperties = jobinfo.Properties as USqlJobProperties;
            if (uSqlJobProperties != null)
            {

                foreach (var resource in uSqlJobProperties.Resources)
                {
                    try
                    {
                        WriteVerbose(string.Format("Downloading {0}", resource.Name));
                        var uri = new Uri(resource.ResourcePath);
                        var adlsAccount = uri.Host.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0];
                        if (adlsAccount == null) throw new ArgumentNullException(nameof(adlsAccount));
                        var path = uri.PathAndQuery;
                        string destFilename = Path.Combine(root_target_folder, Path.GetFileName(path));
                        DataLakeAnalyticsClient.FsClient.FileSystem.DownloadFile(adlsAccount, path, destFilename);
                    }
                    catch (Exception e)
                    {
                        WriteVerbose(e.ToString());
                    }
                }
            }

            // Write out the script
            var scriptPath = Path.Combine(root_target_folder, jobinfo.Name + ".script");
            WriteVerbose(string.Format("Downloading Script {0}", scriptPath));
            File.WriteAllText(scriptPath, jobinfo.Properties.Script);
            WriteVerbose("Compelet.");
        }
    }
}
