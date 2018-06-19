namespace Microsoft.Azure.Management.DataLake.Analytics.Extension
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Management.DataLake.Analytics;
    using Microsoft.Azure.Management.DataLake.Analytics.Models;
    using Microsoft.Azure.Management.DataLake.Analytics.Extension.Models;
    using Microsoft.Azure.Management.DataLake.Store;
    using Microsoft.Azure.Management.DataLake.Store.Models;
    using Microsoft.Rest;

    public static class ExtensionOperationsExtensions
    {
        /// <summary>
        /// Upload Resource Async
        /// </summary>
        /// <param name="accountName">DataLake Analytics account name</param>
        /// <param name="resourceGroup">DataLake Analytics account resourceGroup</param>
        /// <param name="jobIdentity">jobIdentity</param>
        /// <param name="resources">Local resources</param>
        /// <param name="dataLakeAnalyticsAccountManagementClient" cref="DataLakeAnalyticsAccountManagementClient"></param>
        /// <param name="cancellationToken" cref="CancellationToken"></param>
        /// <returns></returns>
        public static async Task<IList<ScopeJobResource>> UploadResourceAsync(this IExtensionOperations operations, string accountName,
            string resourceGroup,
            Guid jobIdentity, IList<ScopeJobResource> resources,
            ServiceClientCredentials credentials,
            IDataLakeAnalyticsAccountManagementClient dataLakeAnalyticsAccountManagementClient,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (accountName == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "accountName");
            }
            if (resourceGroup == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "resourceGroup");
            }
            if (jobIdentity == null || jobIdentity == Guid.Empty)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "jobIdentity");
            }
            if (resources == null || resources.Count <= 0)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "resources");
            }
            if (dataLakeAnalyticsAccountManagementClient == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "dataLakeAnalyticsAccountManagementClient");
            }

            var adlaAccount =
                await
                    dataLakeAnalyticsAccountManagementClient.Account.GetAsync(resourceGroup, accountName,
                        cancellationToken).ConfigureAwait(false);
            var ret = new List<ScopeJobResource>();
            var defaultAdlsAccountName = adlaAccount.DefaultDataLakeStoreAccount;

            if (resources.Any())
            {
                DataLakeStoreAccountInfo defaultAdlsAccount = adlaAccount.DataLakeStoreAccounts.First(adlsAccount => adlsAccount.Name == defaultAdlsAccountName);
                var resourceFolder = string.Format(@"system/jobresources/{0}/", jobIdentity);
                var resourceFolderFullPath = string.Format(@"adl://{0}.{1}/system/jobresources/{2}/", defaultAdlsAccountName, defaultAdlsAccount.Suffix, jobIdentity);
                // Set expiration time
                var expirationTime = new TimeSpan(21, 0, 0, 0);
                DataLakeStoreFileSystemManagementClient dataLakeStoreFileSystemManagementClient = new DataLakeStoreFileSystemManagementClient(credentials, adlsFileSystemDnsSuffix: defaultAdlsAccount.Suffix);
                foreach (var resource in resources)
                {
                    var resourceFileName = Path.GetFileName(resource.Path);
                    var adlPath = resourceFolder + resourceFileName;
                    var adlFullPath = resourceFolderFullPath + resourceFileName;
                    dataLakeStoreFileSystemManagementClient.FileSystem.UploadFile(defaultAdlsAccountName,
                        resource.Path,
                        adlPath,
                        cancellationToken: cancellationToken);
                    await dataLakeStoreFileSystemManagementClient.FileSystem.SetFileExpiryAsync(defaultAdlsAccountName,
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
        /// <param name="accountName">DataLake Analytics account name</param>
        /// <param name="resourceGroup">DataLake Analytics account resourceGroup</param>
        /// <param name="jobIdentity">jobIdentity</param>
        /// <param name="resources">List of files that need to be uploaded with the job</param>
        /// <param name="dataLakeAnalyticsAccountManagementClient" cref="DataLakeAnalyticsAccountManagementClient"></param>
        public static IList<ScopeJobResource> UploadResource(this IExtensionOperations operations, string accountName,
            string resourceGroup,
            Guid jobIdentity, IList<ScopeJobResource> resources,
            ServiceClientCredentials credentials,
            IDataLakeAnalyticsAccountManagementClient dataLakeAnalyticsAccountManagementClient)
        {
            return operations.UploadResourceAsync(accountName, resourceGroup, jobIdentity, resources,
                credentials, dataLakeAnalyticsAccountManagementClient)
                .GetAwaiter()
                .GetResult();
        }


        /// <summary>
        /// Create job
        /// </summary>
        /// <param name="accountName">DataLake Analytics account name</param>
        /// <param name="jobIdentity">jobIdentity</param>
        /// <param name="parameters" cref="CreateScopeJobParameters">CreateScopeJobParameters</param>
        /// <param name="resourceGroup">DataLake Analytics account resourceGroup</param>
        /// <param name="createScopeJobExtensionParameters"  cref="CreateScopeJobExtensionParameters">CreateScopeJobExtensionParameters</param>
        /// <param name="dataLakeAnalyticsAccountManagementClient" cref="DataLakeAnalyticsAccountManagementClient"></param>
        /// <returns></returns>
        public static JobInformation Create(this IExtensionOperations operations, string accountName,
            Guid jobIdentity, CreateScopeJobParameters parameters,
            string resourceGroup = null,
            CreateScopeJobExtensionParameters createScopeJobExtensionParameters = null,
            IDataLakeAnalyticsAccountManagementClient dataLakeAnalyticsAccountManagementClient = null)
        {
            return
                operations.CreateAsync(accountName, jobIdentity, parameters, resourceGroup,
                    createScopeJobExtensionParameters, dataLakeAnalyticsAccountManagementClient)
                    .GetAwaiter()
                    .GetResult();
        }

        /// <summary>
        /// CreateAsync job
        /// </summary>
        /// <param name="accountName">DataLake Analytics account name</param>
        /// <param name="jobIdentity">jobIdentity</param>
        /// <param name="parameters" cref="CreateScopeJobParameters">CreateScopeJobParameters</param>
        /// <param name="resourceGroup">DataLake Analytics account resourceGroup</param>
        /// <param name="createScopeJobExtensionParameters"  cref="CreateScopeJobExtensionParameters">CreateScopeJobExtensionParameters</param>
        /// <param name="dataLakeAnalyticsAccountManagementClient" cref="DataLakeAnalyticsAccountManagementClient"></param>
        /// <param name="cancellationToken" cref="CancellationToken"></param>
        /// <returns></returns>
        public static async Task<JobInformation> CreateAsync(this IExtensionOperations operations, string accountName,
            Guid jobIdentity, CreateScopeJobParameters parameters,
            string resourceGroup = null,
            CreateScopeJobExtensionParameters createScopeJobExtensionParameters = null,
            IDataLakeAnalyticsAccountManagementClient dataLakeAnalyticsAccountManagementClient = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (
                var _result =
                    await
                        operations.CreateWithHttpMessagesAsync(accountName, jobIdentity, parameters,
                            createScopeJobExtensionParameters, /*customHeaders*/
                            null, resourceGroup,
                            dataLakeAnalyticsAccountManagementClient, cancellationToken).ConfigureAwait(false))
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
    }
}
