using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using Microsoft.Azure.Commands.DataLakeAnalytics.Models;
using Microsoft.Azure.Management.DataLake.Analytics.Extension;
using Microsoft.Azure.Management.DataLake.Analytics.Extension.Models;
using Microsoft.Azure.Management.DataLake.Analytics.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.DataLakeAnalytics.Commands.Scope
{
    public class ScopeHelpers
    {
        private static bool ExtractJobResources(string scriptPath, string tokenFile, SubmitAzureDataLakeAnalyticsJob command, string resourceGroup, out string newScriptPath, out string nebulaCommandLineArgs, out List<string> clusterResources, out List<string> localResources)
        {
            StringBuilder extractCommands = new StringBuilder();
            extractCommands.Append("extractjobresources -on adl ");
            extractCommands.Append("-i \"" + scriptPath + "\" ");
            extractCommands.Append("-p " + command.Priority + " ");
            extractCommands.Append("-tokens " + command.AnalyticsUnits + " ");
            if (command.Runtime != null)
            {
                extractCommands.Append("-sv " + command.Runtime + " ");
            }

            NebulaArguments nebulaArguments = new NebulaArguments(command.NebulaArgument);
            nebulaArguments.DisableBonusTokens = command.DisableBonusToken;
            nebulaArguments.MaxUnavailability = command.MaxUnavailability;
            extractCommands.Append(nebulaArguments.ToString() + " ");

            Dictionary<string, string> parameters = command.CreateStringStringDictionary(command.Parameter, false, false);
            if (parameters != null)
            {
                foreach (KeyValuePair<string, string> p in parameters)
                {
                    extractCommands.Append(string.Format("-params {0}=\\\"{1}\\\" ", p.Key, p.Value));
                }
            }

            Dictionary<string, string> properties = command.CreateStringStringDictionary(command.CustomProperty, false, false);
            if (properties != null)
            {
                foreach (KeyValuePair<string, string> p in properties)
                {
                    extractCommands.Append(string.Format("-property {0}={1} ", p.Key, p.Value));
                }
            }

            if (command.NotifyEmail != null)
            {
                extractCommands.Append("-notify " + string.Join(", ", command.NotifyEmail) + " ");
            }

            if (!string.IsNullOrEmpty(command.Account))
            {
                extractCommands.Append("-vc " + GetDataRoot(command, resourceGroup) + " ");
            }

            extractCommands.Append("-SecureInfoFile " + tokenFile + " ");

            if (command.ScopePath != null && command.ScopePath.Count() > 0)
            {
                extractCommands.Append("-SCOPE_PATH " + string.Join(";", command.ScopePath) + " ");
            }

            if (command.ScopeWorkingDir != null)
            {
                extractCommands.Append("-workingRoot " + command.ScopeWorkingDir + " ");
            }

            command.WriteVerbose(string.Format("Extract job resources: scope.exe {0}", extractCommands.ToString()));

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = command.GetScopeExePath();
            startInfo.Arguments = extractCommands.ToString();
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;

            process.StartInfo = startInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            command.WriteVerbose(output);

            Regex reg = new Regex(@"(?s)Cluster resources:\s*(.*)\s*Local resources:\s*(.*)\s*Nebula command line args:\s*([^\r\n]*)\s*Script path:\s*([^\r\n]*)\s*", RegexOptions.Compiled | RegexOptions.Multiline);
            Match result = reg.Match(output);
            if (result.Success)
            {
                clusterResources = result.Groups[1].Value.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                localResources = result.Groups[2].Value.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                nebulaCommandLineArgs = result.Groups[3].Value;
                newScriptPath = result.Groups[4].Value;
                return true;
            }
            else
            {
                newScriptPath = null;
                nebulaCommandLineArgs = null;
                clusterResources = null;
                localResources = null;
                return false;
            }
        }

        public static string GetDataRoot(SubmitAzureDataLakeAnalyticsJob command, string resourceGroup)
        {
            DataLakeAnalyticsAccount account = command.DataLakeAnalyticsClient.GetAccount(resourceGroup, command.Account);
            DataLakeStoreAccountInfo sa = account.DataLakeStoreAccounts.First(s => s.Name == account.DefaultDataLakeStoreAccount);
            return string.Format("adl://{0}.{1}/", sa.Name, sa.Suffix);
        }

        public static JobInformation DoSubmit(List<string> clusterResources, List<string> localResources, string scripPath, SubmitAzureDataLakeAnalyticsJob command, string resourceGroup)
        {
            command.WriteVerbose("Submitting Job");
            CreateScopeJobExtensionParameters createScopeJobExtensionParameters = new CreateScopeJobExtensionParameters()
            {
                EmbeddedFiles = localResources != null ? localResources.Select(f => new ScopeJobResource(Path.GetFileNameWithoutExtension(f), f)).ToList() : new List<ScopeJobResource>()
            };

            CreateScopeJobProperties properties = new CreateScopeJobProperties(File.ReadAllText(scripPath),
                command.Runtime, resources: clusterResources != null ? clusterResources.Select(r => new ScopeJobResource(Path.GetFileNameWithoutExtension(r), r)).ToList() : new List<ScopeJobResource>());

            Dictionary<string, string> customProperties = command.CreateStringStringDictionary(command.CustomProperty, false, false);

            CreateScopeJobParameters jobParameters = new CreateScopeJobParameters(Microsoft.Azure.Management.DataLake.Analytics.Models.JobType.Scope,
                properties, command.Name, command.AnalyticsUnits > 0 ? command.AnalyticsUnits : (int?)null, command.Priority, tags: customProperties);

            return command.DataLakeAnalyticsClient.JobExClient.Extension.Create(command.Account, Guid.NewGuid(), jobParameters, resourceGroup, createScopeJobExtensionParameters, command.DataLakeAnalyticsClient.GetDataLakeAnalyticsAccountManagementClient());
        }

        public static JobInformation Submit(SubmitAzureDataLakeAnalyticsJob command, string scriptPath)
        {
            string final_script_filename = scriptPath;

            // Handle Code-Behind
            string code_behind_filename = string.Format("{0}{1}", command.ScriptPath, ".cs");
            bool requires_code_behind = File.Exists(code_behind_filename);
            var str = string.Format("Code-behind: {0}.", requires_code_behind);
            command.WriteVerbose(str);

            string gen_script_suffix = "_w_code_behind";

            if (requires_code_behind)
            {
                final_script_filename = string.Format("{0}{1}", command.ScriptPath, gen_script_suffix);

                if (File.Exists(final_script_filename))
                {
                    command.WriteVerbose(string.Format("Deleting previous merged code behind file: {0}",
                        final_script_filename));
                    File.Delete(final_script_filename);
                }

                var sb = new System.Text.StringBuilder();

                // Add the original script
                string original_script_code = File.ReadAllText(command.ScriptPath);
                sb.AppendLine(original_script_code);

                // Add the code behind text
                sb.AppendLine("");
                sb.AppendLine("#CS");
                string cs_code = File.ReadAllText(code_behind_filename);
                sb.AppendLine(cs_code);
                sb.AppendLine("#ENDCS");

                File.WriteAllText(final_script_filename, sb.ToString());

                command.WriteVerbose("Created merged script with codebehind");
            }

            if (command.Name == null)
            {
                var now = DateTime.Now;
                var sb = new System.Text.StringBuilder(Path.GetFileNameWithoutExtension(command.ScriptPath));
                sb.AppendFormat("_{0}_{1}_{2}_{3}_{4}",
                    now.Month, now.Day, now.Year,
                    now.Hour, now.Minute);
                command.Name = sb.ToString();
            }

            string token = command.GetToken(command.DefaultProfile.DefaultContext, AzureEnvironment.Endpoint.AzureDataLakeAnalyticsCatalogAndJobEndpointSuffix);
            string tokenFile = Path.GetTempFileName();
            File.WriteAllText(tokenFile, token);

            try
            {
                string newScriptPath;
                string nebulaCommandLineArgs;
                List<string> clusterResources;
                List<string> localResources;
                string resourceGroup = command.ResourceGroup ?? command.DataLakeAnalyticsClient.GetResourceGroupByAccountName(command.Account);

                if (ExtractJobResources(final_script_filename, tokenFile, command, resourceGroup, out newScriptPath, out nebulaCommandLineArgs, out clusterResources, out localResources))
                {
                    JobInformation jobinfo = DoSubmit(clusterResources, localResources, newScriptPath, command, resourceGroup);
                    command.WriteVerbose(jobinfo == null ? "Submit returned NULL JobInfoObject" : "Finished Submitting Job");
                    return jobinfo;
                }

                return null;
            }
            finally
            {
                command.WriteVerbose("Now in Finally Clause after calling Submit()");
                if (requires_code_behind)
                {
                    if (final_script_filename != command.ScriptPath && final_script_filename.Contains(gen_script_suffix))
                    {
                        command.WriteVerbose(string.Format("Deleting {0}", final_script_filename));
                        try
                        {
                            File.Delete(final_script_filename);
                        }
                        catch (Exception e)
                        {
                            command.WriteError(new ErrorRecord(e, "Delete token file failed.", ErrorCategory.WriteError, final_script_filename));
                        }
                    }
                }

                if (File.Exists(tokenFile))
                {
                    try
                    {
                        File.Delete(tokenFile);
                    }
                    catch (Exception e)
                    {
                        command.WriteError(new ErrorRecord(e, "Delete token file failed.", ErrorCategory.WriteError, tokenFile));
                    }
                }
            }
        }
    }
}
