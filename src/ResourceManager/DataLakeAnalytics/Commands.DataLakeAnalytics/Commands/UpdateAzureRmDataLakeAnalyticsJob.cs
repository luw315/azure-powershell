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
using Microsoft.Azure.Management.DataLake.Analytics.Models;
using System;
using System.Collections;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.DataLakeAnalytics
{
    [Cmdlet(VerbsData.Update, "AzureRmDataLakeAnalyticsJob", SupportsShouldProcess = true), OutputType(typeof(JobInformation))]
    [Alias("Update-AdlJob")]
    public class UpdateAzureDataLakeAnalyticsJob : DataLakeAnalyticsCmdletBase
    {
        [Parameter(ValueFromPipelineByPropertyName = true, Position = 0, Mandatory = true,
            HelpMessage = "Name of the Data Lake Analytics account name under which want to update the job.")]
        [ValidateNotNullOrEmpty]
        [Alias("AccountName")]
        public string Account { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Position = 1, ValueFromPipeline = true, Mandatory = true,
            HelpMessage = "ID of the specific job to update.")]
        [ValidateNotNullOrEmpty]
        public Guid JobId { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = false,
            HelpMessage = "The degree of parallelism to use for this job. Typically, a higher degree of parallelism dedicated to a script results in faster script execution time. At least DegreeOfParallelism, Priority, or Tags must be specified.")]
        [ValidateNotNull]
        [ValidateRange(1, int.MaxValue)]
        public int? DegreeOfParallelism { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = false,
            HelpMessage = "The priority for this job with a range from 1 to 1000, where 1000 is the lowest priority and 1 is the highest. At least DegreeOfParallelism, Priority, or Tags must be specified.")]
        [ValidateNotNull]
        [ValidateRange(1, int.MaxValue)]
        public int? Priority { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = false,
            HelpMessage = "A string,string dictionary of tags associated with this job. At least DegreeOfParallelism, Priority, or Tags must be specified.")]
        [ValidateNotNull]
        public Hashtable Tag { get; set; }

        public override void ExecuteCmdlet()
        {
            if (!DegreeOfParallelism.HasValue && !Priority.HasValue && Tag == null)
            {
                throw new ArgumentException(Properties.Resources.MissingUpdateJobField);
            }

            ConfirmAction(
                string.Format(
                    Properties.Resources.UpdateDataLakeAnalyticsJob,
                    JobId,
                    DegreeOfParallelism.HasValue ? "\r\nDegreeOfParallelism: " + DegreeOfParallelism.Value : string.Empty,
                    Priority.HasValue ? "\r\nPriority: " + Priority.Value : string.Empty,
                    Tag != null ? "\r\nTags: " + Tag.ToString() : string.Empty),
                JobId.ToString(), 
                () =>
                {
                    WriteObject(
                        this.DataLakeAnalyticsClient.UpdateJob(
                            Account,
                            JobId,
                            DegreeOfParallelism,
                            Priority,
                            Tag));
                });
        }
    }
}