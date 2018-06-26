namespace Microsoft.Azure.Management.DataLake.InternalAnalytics.Scope.Utils
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Management.DataLake.InternalAnalytics.Scope.Models;
    using Microsoft.Azure.Management.DataLake.Analytics.Models;
    using Microsoft.Azure.Management.DataLake.Store;
    using Analytics;

    internal static class DiffJobsHelper
    {
        internal static async Task DoAsync(string accountName, string resourceGroup,
            Guid leftJob, Guid rightJob,
            IDataLakeStoreFileSystemManagementClient dataLakeStoreFileSystemManagementClient,
            IDataLakeAnalyticsAccountManagementClient dataLakeAnalyticsAccountManagementClient,
            IDataLakeAnalyticsJobManagementClient dataLakeAnalyticsJobManagementClient,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            JobDiffInfo leftJobInfo, rightJobInfo;
            var adlaAccount =
                await
                    dataLakeAnalyticsAccountManagementClient.Account.GetAsync(resourceGroup, accountName,
                        cancellationToken).ConfigureAwait(false);
            var defaultAdlsAccountName = adlaAccount.DefaultDataLakeStoreAccount;
            try
            {
                leftJobInfo =
                    await
                        GetJobDiffInfoAsync(accountName, leftJob, "> job1", defaultAdlsAccountName,
                            dataLakeStoreFileSystemManagementClient, dataLakeAnalyticsJobManagementClient,
                            cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ConsoleReport.ReportTimestamp("Failed to get information about job {0}: error {1}", new object[] { leftJob, ex.Message });
                return;
            }

            try
            {
                rightJobInfo =
                    await
                        GetJobDiffInfoAsync(accountName, rightJob, "< job2", defaultAdlsAccountName,
                            dataLakeStoreFileSystemManagementClient, dataLakeAnalyticsJobManagementClient,
                            cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ConsoleReport.ReportTimestamp("Failed to get information about job {0}: error {1}", new object[] { rightJob, ex.Message });
                return;
            }
            var jobDiff = new JobDiff(leftJobInfo, rightJobInfo);
            jobDiff.Run();
            ConsoleReport.ReportTimestamp("comparison completed.", new object[0]);
            jobDiff.ShowDiff();
        }

        private static async Task<JobDiffInfo> GetJobDiffInfoAsync(string accountName, Guid jobId, string tag,
            string defaultAdlsAccountName,
            IDataLakeStoreFileSystemManagementClient dataLakeStoreFileSystemManagementClient,
            IDataLakeAnalyticsJobManagementClient dataLakeAnalyticsJobManagementClient, CancellationToken cancellationToken)
        {
            JobDiffInfo diffJobInfo = new JobDiffInfo();
            JobStatistics fakeJob = new JobStatistics();
            ConsoleReport.ReportTimestamp("Getting information about " + tag + " {0}", new object[] { jobId });
            try
            {
                var jobInfo = await dataLakeAnalyticsJobManagementClient.Job.GetAsync(accountName, jobId, cancellationToken).ConfigureAwait(false);
                diffJobInfo.JobInfo = jobInfo;
                diffJobInfo.RuntimeVersion = jobInfo.Properties.RuntimeVersion;
                diffJobInfo.TotalInputDataSize =
                    await
                        GetJobInputDataSizeAsync(jobInfo, dataLakeStoreFileSystemManagementClient,
                            defaultAdlsAccountName, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ConsoleReport.ReportTimestamp("Failed to get information about job {0}, error {1}.", new object[] { jobId, ex.Message });
                ConsoleReport.ReportTimestamp("Try get information from job repository about job {0}...", new object[] { jobId });
            }

            try
            {
                diffJobInfo.JobStatistics = await dataLakeAnalyticsJobManagementClient.Job.GetStatisticsAsync(accountName, jobId, cancellationToken);
            }
            catch
            {
                ConsoleReport.ReportTimestamp("WARNING: Failed to get JobStatistics about job {0}", new object[] { jobId });
                diffJobInfo.JobStatistics = fakeJob;
            }
            return diffJobInfo;
        }

        private static async Task<long> GetJobInputDataSizeAsync(JobInformation jobInfo,
            IDataLakeStoreFileSystemManagementClient dataLakeStoreFileSystemManagementClient,
            string defaultAdlsAccountName, CancellationToken cancellationToken)
        {
            try
            {
                var fullInputFile =
                    new Uri(new Uri(jobInfo.GetAlgebraFilePath()), DataLakeAnalyticsExtensionConstants.ScopeInternalInfo)
                        .PathAndQuery;
                using (
                    var stream =
                        await
                            dataLakeStoreFileSystemManagementClient.FileSystem.OpenAsync(defaultAdlsAccountName,
                                fullInputFile, cancellationToken: cancellationToken).ConfigureAwait(false))
                {
                    var xmlDoc = SafeXml.LoadFromStream(stream);
                    var node = xmlDoc.GetElementsByTagName("InputSize")[0];
                    return (long) double.Parse(node.InnerText);
                }
            }
            catch (Exception e)
            {
                ConsoleReport.ReportTimestamp("Failed to get InputDataSize.", new object[] { e.Message });
                // ignored
                return 0;
            }
        }
    }
}
