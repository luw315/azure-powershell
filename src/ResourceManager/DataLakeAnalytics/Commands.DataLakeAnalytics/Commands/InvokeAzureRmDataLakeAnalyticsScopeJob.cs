using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using Microsoft.Azure.Commands.DataLakeAnalytics.Models;
using Microsoft.Azure.Commands.DataLakeAnalytics.Properties;
using Microsoft.Azure.Management.DataLake.Analytics.Models;
using Microsoft.Rest.Azure;

namespace Microsoft.Azure.Commands.DataLakeAnalytics.Commands
{
    [Cmdlet(VerbsLifecycle.Invoke, "AzureRmDataLakeAnalyticsScopeJob", SupportsShouldProcess = true), OutputType(typeof(JobInformation))]
    [Alias("Invoke-AdlScopeJob")]
    public class InvokeAzureRmDataLakeAnalyticsScopeJob : DataLakeAnalyticsCmdletBase
    {
        private static string _option = "Run";

        //[Parameter(ValueFromPipelineByPropertyName = true, ParameterSetName = ScopeJobWithScriptPath, Position = 0,
        //     Mandatory = false, HelpMessage = "The default Data Lake storage account for this script")]
        // [ValidateNotNullOrEmpty]
        //public string StorageAccount { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, ParameterSetName = ScopeJobWithScriptPath, Position = 1,
            Mandatory = true, HelpMessage = "Path to the script file to run.")]
        [ValidateNotNullOrEmpty]
        public string ScriptPath { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, ParameterSetName = ScopeJobWithScriptPath,
            Mandatory = false, HelpMessage = "If the path given in the script is an absolute path, it is used and no further processing is needed. If the path is relative, the system will do probing as described above using the search path for input streams. The input stream search path is configurable, but the default value is: $(SCRIPT_DIR).")]
        [ValidateNotNullOrEmpty]
        public string[] InputPath { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, ParameterSetName = ScopeJobWithScriptPath,
                    Mandatory = false, HelpMessage = "If the path given in the script is an absolute path, it is used and no further processing is needed. If the path is relative, the system will combine the relate path with the system Output Stream Path. The output stream path is a configurable value with a default of $(SCRIPT_DIR).")]
        [ValidateNotNullOrEmpty]
        public string OutputPath { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, ParameterSetName = ScopeJobWithScriptPath,
           Mandatory = false,
           HelpMessage =
               "The ScopePath for this job"
           )]
        public string[] ScopePath { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, ParameterSetName = ScopeJobWithScriptPath,
            Mandatory = false,
            HelpMessage =
                "Specify parameters that you want to pass to SCOPE script. "
            )]
        public Hashtable Parameter { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, ParameterSetName = ScopeJobWithScriptPath,
            Mandatory = false,
            HelpMessage =
                "Specify root directory for script cache folders and temporary folders for script execution."
            )]
        public string ScopeWorkingDir { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, ParameterSetName = ScopeJobWithScriptPath,
            Mandatory = false,
            HelpMessage =
                "The scope Option"
            )]
        [ValidateSet("Run", "Compile", "buildclusterplan")]
        public string Option
        {
            get { return _option; }
            set { _option = value; }
        }

        public SwitchParameter SuppressParameterQuoting { get; set; }

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
            var powerShellDestinationPath = SessionState.Path.GetUnresolvedProviderPathFromPSPath(ScriptPath);
            string tokenFileName = null;
            if (!File.Exists(powerShellDestinationPath))
            {
                WriteVerbose(powerShellDestinationPath);
                throw new CloudException(string.Format(Properties.Resources.ScriptFilePathDoesNotExist,
                    powerShellDestinationPath));
            }

            try
            {
                var process = new Process();
                process.StartInfo.FileName = GetScopeExePath();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.Arguments = string.Format(@"{0} -i {1} {2} -on adl", Option, powerShellDestinationPath,
                    GetScopeCommandParams(CreateStringStringDictionary(Parameter, false,
                        false)));
                if (!string.IsNullOrWhiteSpace(ScopeWorkingDir))
                {
                    process.StartInfo.Arguments = process.StartInfo.Arguments + " -workingRoot " + GetScopeWorkingDirectory(ScopeWorkingDir);
                }

                if (ScopePath != null)
                {
                    process.StartInfo.Arguments = process.StartInfo.Arguments + " -SCOPE_PATH " + NormalizeScopePath(ScopePath);
                }
                else
                {
                    process.StartInfo.Arguments = process.StartInfo.Arguments + " -SCOPE_PATH $(SCOPE_DIR)";
                }

                if (InputPath != null)
                {
                    process.StartInfo.Arguments = process.StartInfo.Arguments + " -INPUT_PATH " + string.Join(";", InputPath);
                }
                else
                {
                    process.StartInfo.Arguments = process.StartInfo.Arguments + " -INPUT_PATH $(SCRIPT_DIR);$(SCOPE_DIR)";
                }

                if (!string.IsNullOrEmpty(OutputPath))
                {
                    process.StartInfo.Arguments = process.StartInfo.Arguments + " -OUTPUT_PATH " + OutputPath;
                }

                //if (!string.IsNullOrWhiteSpace(StorageAccount))
                //{
                //    var token = GetToken(DefaultProfile.DefaultContext,
                //        AzureEnvironment.Endpoint.AzureDataLakeStoreFileSystemEndpointSuffix);
                //    tokenFileName = string.Format("{0}.txt", Guid.NewGuid());
                //    File.WriteAllText(tokenFileName, token);
                //    tokenFileName = Path.GetFullPath(tokenFileName);
                //    process.StartInfo.Arguments = process.StartInfo.Arguments +
                //                                  string.Format(" -vc adl://{0}.{1}/ -SecureInfoFile {2} ",
                //                                      StorageAccount,
                //                                      DefaultProfile.DefaultContext.Environment.GetEndpoint(AzureEnvironment.Endpoint
                //                                          .AzureDataLakeStoreFileSystemEndpointSuffix),
                //                                      tokenFileName);
                //}
                WriteVerbose($"{process.StartInfo.FileName} {process.StartInfo.Arguments}");
                process.Start();
                while (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine();

                    Console.WriteLine(line);
                }
                process.WaitForExit();
                process.Close();
            }
            finally
            {
                if (tokenFileName != null)
                {
                    File.Delete(tokenFileName);
                }
            }
        }

        static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            Console.WriteLine(outLine.Data);
        }
    }
}
