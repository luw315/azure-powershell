namespace Microsoft.Azure.Management.DataLake.InternalAnalytics.Scope
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Management.DataLake.Analytics.Models;
    using Microsoft.Azure.Management.DataLake.InternalAnalytics.Scope.Models;
    using Microsoft.Azure.Management.DataLake.Store;
    using Microsoft.Rest.Azure;
    using Analytics;

    public interface IExtensionOperations
    {
        /// <summary>
        /// CreateWithHttpMessagesAsync
        /// </summary>
        /// <param name="accountName">DataLake Analytics account name</param>
        /// <param name="jobIdentity">jobIdentity</param>
        /// <param name="parameters" cref="CreateScopeJobParameters">CreateScopeJobParameters</param>
        /// <param name="resourceGroup">DataLake Analytics account resourceGroup</param>
        /// <param name="createScopeJobExtensionParameters"  cref="CreateScopeJobExtensionParameters">CreateScopeJobExtensionParameters</param>
        /// <param name="dataLakeAnalyticsAccountManagementClient" cref="DataLakeAnalyticsAccountManagementClient"></param>
        /// <param name="cancellationToken" cref="CancellationToken"></param>
        /// <returns></returns>
        Task<AzureOperationResponse<JobInformation>> CreateWithHttpMessagesAsync(string accountName,
            Guid jobIdentity, CreateScopeJobParameters parameters,
            CreateScopeJobExtensionParameters createScopeJobExtensionParameters = null,
            Dictionary<string, List<string>> customHeaders = null,
            string resourceGroup = null,
            IDataLakeAnalyticsAccountManagementClient dataLakeAnalyticsAccountManagementClient = null,
            CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// DiffJobsAsync
        /// </summary>
        /// <param name="accountName">DataLake Analytics account name</param>
        /// <param name="resourceGroup">DataLake Analytics account resourceGroup</param>
        /// <param name="leftJob">leftJob Id</param>
        /// <param name="rightJob">rightJob Id</param>
        /// <param name="dataLakeStoreFileSystemManagementClient" cerf="IDataLakeStoreFileSystemManagementClient">dataLakeStoreFileSystemManagementClient</param>
        /// <param name="dataLakeAnalyticsAccountManagementClient" cerf="dataLakeAnalyticsAccountManagementClient">dataLakeAnalyticsAccountManagementClient</param>
        /// <param name="cancellationToken" cerf="CancellationToken"></param>
        /// <returns></returns>
        Task DiffJobsAsync(string accountName, string resourceGroup,
            Guid leftJob, Guid rightJob,
            IDataLakeStoreFileSystemManagementClient dataLakeStoreFileSystemManagementClient,
            IDataLakeAnalyticsAccountManagementClient dataLakeAnalyticsAccountManagementClient,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// GetDataRoot
        /// </summary>
        /// <param name="accountName">DataLake Analytics account name</param>
        /// <param name="resourceGroup">DataLake Analytics account resourceGroup</param>
        /// <param name="dataLakeAnalyticsAccountManagementClient">dataLakeAnalyticsAccountManagementClient</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Data root. For example: adl://sandboxadl.azuredatalakestore.net/</returns>
        Task<string> GetDataRootAsync(string accountName, string resourceGroup,
            IDataLakeAnalyticsAccountManagementClient dataLakeAnalyticsAccountManagementClient,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
