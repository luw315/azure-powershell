


namespace Microsoft.Azure.Management.DataLake.Analytics.Scope
{
    using Microsoft.Rest;
    using Newtonsoft.Json;

    public interface IDataLakeAnalyticsJobManagementExtensionClient
    {
        /// <summary>
        /// The base URI of the service.
        /// </summary>

        /// <summary>
        /// Gets or sets json serialization settings.
        /// </summary>
        JsonSerializerSettings SerializationSettings { get; }

        /// <summary>
        /// Gets or sets json deserialization settings.
        /// </summary>
        JsonSerializerSettings DeserializationSettings { get; }

        /// <summary>
        /// Credentials needed for the client to connect to Azure.
        /// </summary>
        ServiceClientCredentials Credentials { get; }

        /// <summary>
        /// Client Api Version.
        /// </summary>
        string ApiVersion { get; }

        /// <summary>
        /// Gets the DNS suffix used as the base for all Azure Data Lake
        /// Analytics Job service requests.
        /// </summary>
        string AdlaJobDnsSuffix { get; set; }

        /// <summary>
        /// Gets or sets the preferred language for the response.
        /// </summary>
        string AcceptLanguage { get; set; }

        /// <summary>
        /// Gets or sets the retry timeout in seconds for Long Running
        /// Operations. Default value is 30.
        /// </summary>
        int? LongRunningOperationRetryTimeout { get; set; }

        /// <summary>
        /// When set to true a unique x-ms-client-request-id value is generated
        /// and included in each request. Default is true.
        /// </summary>
        bool? GenerateClientRequestId { get; set; }

        /// <summary>
        /// Gets the IExtensionOperations.
        /// </summary>
        IExtensionOperations Extension { get; }
    }
}
