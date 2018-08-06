namespace Microsoft.Azure.Management.DataLake.InternalAnalytics.Scope
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Management.DataLake.Analytics;
    using Microsoft.Azure.Management.DataLake.Analytics.Models;
    using Microsoft.Azure.Management.DataLake.InternalAnalytics.Scope.Models;
    using Microsoft.Azure.Management.DataLake.Store;
    using Microsoft.Azure.Management.DataLake.Store.Models;
    using Microsoft.Rest;
    using System.Text.RegularExpressions;

    public static class ExtensionOperationsExtensions
    {
        /// <summary>
        /// Upload Resource Async
        /// </summary>
        /// <param name="adlsAccount">Data Lake Store account</param>
        /// <param name="jobIdentity">jobIdentity</param>
        /// <param name="resources">Local resources</param>
        /// <param name="cancellationToken" cref="CancellationToken"></param>
        /// <returns></returns>
        public static async Task<IList<ScopeJobResource>> UploadResourceAsync(this IExtensionOperations operations, DataLakeStoreAccountInfo adlsAccount,
            Guid jobIdentity, IList<ScopeJobResource> resources, ServiceClientCredentials credentials, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (jobIdentity == null || jobIdentity == Guid.Empty)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "jobIdentity");
            }
            if (resources == null || resources.Count <= 0)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "resources");
            }

            var ret = new List<ScopeJobResource>();
            if (resources.Any())
            {
                var resourceFolder = string.Format(@"system/jobresources/{0}/", jobIdentity);
                var resourceFolderFullPath = string.Format(@"adl://{0}.{1}/system/jobresources/{2}/", adlsAccount.Name, adlsAccount.Suffix, jobIdentity);
                // Set expiration time
                var expirationTime = new TimeSpan(21, 0, 0, 0);
                DataLakeStoreFileSystemManagementClient dataLakeStoreFileSystemManagementClient = new DataLakeStoreFileSystemManagementClient(credentials, adlsFileSystemDnsSuffix: adlsAccount.Suffix);
                foreach (var resource in resources)
                {
                    var resourceFileName = Path.GetFileName(resource.Path);
                    var adlPath = resourceFolder + resourceFileName;
                    var adlFullPath = resourceFolderFullPath + resourceFileName;
                    dataLakeStoreFileSystemManagementClient.FileSystem.UploadFile(adlsAccount.Name,
                        resource.Path,
                        adlPath,
                        cancellationToken: cancellationToken);
                    await dataLakeStoreFileSystemManagementClient.FileSystem.SetFileExpiryAsync(adlsAccount.Name,
                        adlPath,
                        ExpiryOptionType.RelativeToNow,
                        (long)expirationTime.TotalMilliseconds,
                        cancellationToken: cancellationToken).ConfigureAwait(false);
                    ret.Add(new ScopeJobResource(resource.Name, adlFullPath));
                }
            }
            return ret;
        }

        /// <summary>
        /// Upload Resource
        /// </summary>
        /// <param name="adlsAccount">Data Lake Store account</param>
        /// <param name="jobIdentity">jobIdentity</param>
        /// <param name="resources">List of files that need to be uploaded with the job</param>
        public static IList<ScopeJobResource> UploadResource(this IExtensionOperations operations, DataLakeStoreAccountInfo adlsAccount,
            Guid jobIdentity, IList<ScopeJobResource> resources, ServiceClientCredentials credentials)
        {
            return operations.UploadResourceAsync(adlsAccount, jobIdentity, resources, credentials).GetAwaiter().GetResult();
        }


        /// <summary>
        /// Create job
        /// </summary>
        /// <param name="accountName">DataLake Analytics account name</param>
        /// <param name="jobIdentity">jobIdentity</param>
        /// <param name="parameters" cref="CreateScopeJobParameters">CreateScopeJobParameters</param>
        /// <param name="adlsAccount">Data Lake Store account</param>
        /// <param name="localResources"  cref="localResources">localResources</param>
        /// <returns></returns>
        public static JobInformation Create(this IExtensionOperations operations, string accountName,
            Guid jobIdentity, CreateScopeJobParameters parameters,
            DataLakeStoreAccountInfo adlsAccount = null,
            List<ScopeJobResource> localResources = null)
        {
            return
                operations.CreateAsync(accountName, jobIdentity, parameters, adlsAccount, localResources).GetAwaiter().GetResult();
        }

        private static readonly Regex adlsPath = new Regex(@"[^:\/]+:\/\/([^.]+).([^\/]+)\/", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Create job
        /// </summary>
        /// <param name="accountName">DataLake Analytics account name</param>
        /// <param name="jobIdentity">jobIdentity</param>
        /// <param name="parameters" cref="CreateScopeJobParameters">CreateScopeJobParameters</param>
        /// <param name="dataRoot">Data Lake Store account. For example adl://sandbox.azuredatalakestore.net/</param>
        /// <param name="localResources"  cref="localResources">localResources</param>
        /// <returns></returns>
        public static JobInformation Create(this IExtensionOperations operations, string accountName,
            Guid jobIdentity, CreateScopeJobParameters parameters,
            string dataRoot = null,
            List<ScopeJobResource> localResources = null)
        {
            DataLakeStoreAccountInfo adlsAccount = null;
            if (dataRoot != null)
            {
                adlsAccount = new DataLakeStoreAccountInfo();
                Match m = adlsPath.Match(dataRoot);
                if (m.Success)
                {
                    adlsAccount.Name = m.Groups[1].Value;
                    adlsAccount.Suffix = m.Groups[2].Value;
                }
                else
                {
                    throw new ArgumentException("dataRoot is not available.");
                }
            }

            return operations.CreateAsync(accountName, jobIdentity, parameters, adlsAccount, localResources).GetAwaiter().GetResult();
        }

        /// <summary>
        /// CreateAsync job
        /// </summary>
        /// <param name="accountName">DataLake Analytics account name</param>
        /// <param name="jobIdentity">jobIdentity</param>
        /// <param name="parameters" cref="CreateScopeJobParameters">CreateScopeJobParameters</param>
        /// <param name="adlsAccount">Data Lake Store account</param>
        /// <param name="localResources"  cref="localResources">localResources</param>
        /// <param name="cancellationToken" cref="CancellationToken"></param>
        /// <returns></returns>
        public static async Task<JobInformation> CreateAsync(this IExtensionOperations operations, string accountName,
            Guid jobIdentity, CreateScopeJobParameters parameters,
            DataLakeStoreAccountInfo adlsAccount = null,
            List<ScopeJobResource> localResources = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (
                var _result =
                    await
                        operations.CreateWithHttpMessagesAsync(accountName, jobIdentity, parameters,
                            localResources, /*customHeaders*/
                            null, adlsAccount, cancellationToken).ConfigureAwait(false))
            {
                return _result.Body;
            }
        }

        /// <summary>
        /// Diff Jobs
        /// </summary>
        /// <param name="accountName">DataLake Analytics account name</param>
        /// <param name="resourceGroup">DataLake Analytics account resourceGroup</param>
        /// <param name="leftJob">leftJob Id</param>
        /// <param name="rightJob">rightJob Id</param>
        /// <param name="dataLakeStoreFileSystemManagementClient" cerf="IDataLakeStoreFileSystemManagementClient">dataLakeStoreFileSystemManagementClient</param>
        /// <param name="dataLakeAnalyticsAccountManagementClient" cerf="dataLakeAnalyticsAccountManagementClient">dataLakeAnalyticsAccountManagementClient</param>
        public static void DiffJobs(this IExtensionOperations operations, string accountName, string resourceGroup,
            Guid leftJob, Guid rightJob,
            IDataLakeStoreFileSystemManagementClient dataLakeStoreFileSystemManagementClient,
            IDataLakeAnalyticsAccountManagementClient dataLakeAnalyticsAccountManagementClient)
        {
            operations.DiffJobsAsync(accountName, resourceGroup, leftJob, rightJob,
                    dataLakeStoreFileSystemManagementClient, dataLakeAnalyticsAccountManagementClient, CancellationToken.None)
                    .GetAwaiter()
                    .GetResult();
        }

        /// <summary>
        /// GetDataRoot
        /// </summary>
        /// <param name="accountName">DataLake Analytics account name</param>
        /// <param name="resourceGroup">DataLake Analytics account resourceGroup</param>
        /// <param name="subscriptionId">subscriptionId</param>
        /// <param name="credentials">credentials</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Data root</returns>
        public static DataLakeStoreAccountInfo GetDataRoot(this IExtensionOperations operations, string accountName, string resourceGroup, string subscriptionId, ServiceClientCredentials credentials,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return operations.GetDataRootAsync(accountName, resourceGroup, subscriptionId, credentials, cancellationToken).GetAwaiter().GetResult();
        }
    }
}
