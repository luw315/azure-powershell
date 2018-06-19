// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Commands.DataLakeAnalytics.Models;
using Microsoft.Azure.Commands.DataLakeAnalytics.Properties;
using System;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.DataLakeAnalytics
{
    [Cmdlet(VerbsLifecycle.Suspend, "AzureRmDataLakeAnalyticsJob", SupportsShouldProcess = true), OutputType(typeof(bool))]
    [Alias("Suspend-AdlJob")]
    public class SuspendAzureDataLakeAnalyticsJob : DataLakeAnalyticsCmdletBase
    {
        [Parameter(ValueFromPipelineByPropertyName = true, Position = 0, Mandatory = true,
            HelpMessage = "Name of the Data Lake Analytics account name under which want to yield the job.")]
        [ValidateNotNullOrEmpty]
        [Alias("AccountName")]
        public string Account { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Position = 1, ValueFromPipeline = true, Mandatory = true,
            HelpMessage = "ID of the specific job to yield.")]
        [ValidateNotNullOrEmpty]
        public Guid JobId { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Position = 2, Mandatory = false,
            HelpMessage = "Indicates that the job should be forcibly yielded.")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter Force { get; set; }

        [Parameter(Position = 3, Mandatory = false)]
        public SwitchParameter PassThru { get; set; }

        public override void ExecuteCmdlet()
        {
            ConfirmAction(
                Force.IsPresent,
                string.Format(Properties.Resources.YieldingDataLakeAnalyticsJob, JobId),
                string.Format(Properties.Resources.YieldDataLakeAnalyticsJob, JobId),
                JobId.ToString(),
                () =>
                {
                    DataLakeAnalyticsClient.YieldJob(Account, JobId);
                    if (PassThru)
                    {
                        WriteObject(true);
                    }
                });
        }
    }
}