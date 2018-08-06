namespace Microsoft.Azure.Management.DataLake.InternalAnalytics.Scope
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Management.DataLake.InternalAnalytics.Scope.Models;
    using Microsoft.Azure.Management.DataLake.InternalAnalytics.Scope.Utils;
    using Microsoft.Azure.Management.DataLake.Analytics.Models;
    using Microsoft.Azure.Management.DataLake.Store;
    using Microsoft.Rest;
    using Microsoft.Rest.Azure;
    using Analytics;

    internal class ExtensionOperations : IExtensionOperations
    {
        /// <summary>
        /// Initializes a new instance of the ExtensionOperations class.
        /// </summary>
        /// <param name='client'>
        /// Reference to the service client.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        internal ExtensionOperations(DataLakeInternalAnalyticsScopeJobManagementClient client)
        {
            if (client == null)
            {
                throw new System.ArgumentNullException("client");
            }
            Client = client;
        }

        /// <summary>
        /// Gets a reference to the DataLakeAnalyticsJobManagementExtensionClient
        /// </summary>
        private DataLakeInternalAnalyticsScopeJobManagementClient Client { get; set; }


        /// <summary>
        /// Submits a job to the specified Data Lake Analytics account.
        /// </summary>
        /// <param name='accountName'>
        /// The Azure Data Lake Analytics account to execute job operations on.
        /// </param>
        /// <param name='jobIdentity'>
        /// Job identifier. Uniquely identifies the job across all jobs submitted to
        /// the service.
        /// </param>
        /// <param name='parameters'>
        /// The parameters to submit a job.
        /// </param>
        /// <param name='localResources'>
        /// List of files that need to be uploaded with the job.
        /// </param>
        /// <param name='customHeaders'>
        /// Headers that will be added to request.
        /// </param>
        /// <param name="adlsAccount">
        /// Upload the local resources to the Azure Data Lake Store account.
        /// </param>
        /// <param name='cancellationToken' cerf="CancellationToken">
        /// The cancellation token.
        /// </param>
        /// <exception cref="CloudException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        /// <return>
        /// A response object containing the response body and response headers.
        /// </return>
        public async Task<AzureOperationResponse<JobInformation>> CreateWithHttpMessagesAsync(string accountName,
            Guid jobIdentity, CreateScopeJobParameters parameters,
            List<ScopeJobResource> localResources = null, 
            Dictionary<string, List<string>> customHeaders = null,
            DataLakeStoreAccountInfo adlsAccount = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (accountName == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "accountName");
            }
            if (jobIdentity == null || jobIdentity == Guid.Empty)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "jobIdentity");
            }
            if (Client.AdlaJobDnsSuffix == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.AdlaJobDnsSuffix");
            }
            if (parameters == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "parameters");
            }
            parameters.Validate();
            if (Client.ApiVersion == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
            }

            if (localResources != null && localResources.Any())
            {
                var createScopeJobProperties = parameters.Properties as CreateScopeJobProperties;
                if (createScopeJobProperties != null)
                {
                    var adlResources = await this.UploadResourceAsync(adlsAccount, jobIdentity, localResources.ToArray(), Client.Credentials, cancellationToken).ConfigureAwait(false);

                    if (createScopeJobProperties.Resources == null)
                    {
                        createScopeJobProperties.Resources = new List<ScopeJobResource>();
                    }
                    if (adlResources != null && adlResources.Any())
                    {
                        foreach (var resource in adlResources)
                        {
                            createScopeJobProperties.Resources.Add(resource);
                        }
                    }
                }
            }

            return await Client.BaseClient.Job.CreateWithHttpMessagesAsync(accountName, jobIdentity, parameters, customHeaders, cancellationToken).ConfigureAwait(false);
        }

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
        public async Task DiffJobsAsync(string accountName, string resourceGroup,
            Guid leftJob, Guid rightJob, 
            IDataLakeStoreFileSystemManagementClient dataLakeStoreFileSystemManagementClient,
            IDataLakeAnalyticsAccountManagementClient dataLakeAnalyticsAccountManagementClient,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(accountName))
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "accountName");
            }
            if (string.IsNullOrWhiteSpace(resourceGroup))
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "resourceGroup");
            }
            if (leftJob == null || leftJob == Guid.Empty)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "leftJob");
            }
            if (rightJob == null || rightJob == Guid.Empty)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "rightJob");
            }
            if (dataLakeStoreFileSystemManagementClient == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "dataLakeStoreFileSystemManagementClient");
            }
            if (dataLakeAnalyticsAccountManagementClient == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "dataLakeAnalyticsAccountManagementClient");
            }
            if (Client.AdlaJobDnsSuffix == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.AdlaJobDnsSuffix");
            }
            if (Client.ApiVersion == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
            }

            await
                DiffJobsHelper.DoAsync(accountName, resourceGroup, leftJob, rightJob,
                    dataLakeStoreFileSystemManagementClient, dataLakeAnalyticsAccountManagementClient, this.Client.BaseClient,
                    cancellationToken).ConfigureAwait(false);
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
        public async Task<DataLakeStoreAccountInfo> GetDataRootAsync(string accountName, string resourceGroup, string subscriptionId, ServiceClientCredentials credentials,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(accountName))
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "accountName");
            }

            if (string.IsNullOrWhiteSpace(resourceGroup))
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "resourceGroup");
            }

            DataLakeAnalyticsAccountManagementClient accountClient = new DataLakeAnalyticsAccountManagementClient(credentials);
            accountClient.SubscriptionId = subscriptionId;

            DataLakeAnalyticsAccount account = await accountClient.Account.GetAsync(resourceGroup, accountName, cancellationToken).ConfigureAwait(false);
            return account.DataLakeStoreAccounts.First(s => s.Name == account.DefaultDataLakeStoreAccount);
        }
    }
}
