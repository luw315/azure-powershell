namespace Microsoft.Azure.Management.DataLake.Analytics.Extension
{
    using System.Net.Http;
    using System.Collections.Generic;
    using Microsoft.Azure.Management.DataLake.Analytics.Extension.Utils;
    using Microsoft.Azure.Management.DataLake.Analytics.Models;
    using Microsoft.Rest;
    using Microsoft.Rest.Azure;
    using Microsoft.Rest.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// Creates a Data Lake Store filesystem management extension client.
    /// </summary>
    public sealed class DataLakeAnalyticsJobManagementExtensionClient : ServiceClient<DataLakeAnalyticsJobManagementExtensionClient>, IDataLakeAnalyticsJobManagementExtensionClient, IAzureClient
    {
        /// <summary>
        /// The base client
        /// </summary>
        public IDataLakeAnalyticsJobManagementClient BaseClient { get; private set; }

        /// <summary>
        /// The base URI of the service.
        /// </summary>
        internal string BaseUri { get; set; }

        /// <summary>
        /// Gets or sets json serialization settings.
        /// </summary>
        public JsonSerializerSettings SerializationSettings { get; private set; }

        /// <summary>
        /// Gets or sets json deserialization settings.
        /// </summary>
        public JsonSerializerSettings DeserializationSettings { get; private set; }

        /// <summary>
        /// Credentials needed for the client to connect to Azure.
        /// </summary>
        public ServiceClientCredentials Credentials { get; private set; }

        /// <summary>
        /// Client Api Version.
        /// </summary>
        public string ApiVersion { get; private set; }

        /// <summary>
        /// Gets the DNS suffix used as the base for all Azure Data Lake Analytics Job
        /// service requests.
        /// </summary>
        public string AdlaJobDnsSuffix { get; set; }

        /// <summary>
        /// Gets or sets the preferred language for the response.
        /// </summary>
        public string AcceptLanguage { get; set; }

        /// <summary>
        /// Gets or sets the retry timeout in seconds for Long Running Operations.
        /// Default value is 30.
        /// </summary>
        public int? LongRunningOperationRetryTimeout { get; set; }

        /// <summary>
        /// When set to true a unique x-ms-client-request-id value is generated and
        /// included in each request. Default is true.
        /// </summary>
        public bool? GenerateClientRequestId { get; set; }

        /// <summary>
        /// Gets the IExtensionOperations.
        /// </summary>
        public IExtensionOperations Extension { get; private set; }

        /// <summary>
        /// Initializes a new instance of the DataLakeAnalyticsJobManagementExtensionClient class.
        /// </summary>
        /// <param name='credentials'>
        /// Required. Gets Azure subscription credentials.
        /// </param>
        /// <param name='userAgentAssemblyVersion'>
        /// Optional. The version string that should be sent in the user-agent header for all requests. The default is the current version of the SDK.
        /// </param>
        /// <param name='adlaJobDnsSuffix'>
        /// Optional. The dns suffix to use for all requests for this client instance. The default is 'azuredatalakeanalytics.net'.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        public DataLakeAnalyticsJobManagementExtensionClient(ServiceClientCredentials credentials,
            string userAgentAssemblyVersion = "",
            string adlaJobDnsSuffix = DataLakeAnalyticsExtensionConstants.DefaultAdlaDnsSuffix,
            params DelegatingHandler[] handlers)
            : this(credentials, handlers)
        {
            this.AdlaJobDnsSuffix = adlaJobDnsSuffix;
            DataLakeAnalyticsCustomizationHelper.UpdateUserAgentAssemblyVersion(this, userAgentAssemblyVersion);
            BaseClient = new DataLakeAnalyticsJobManagementClient(credentials, userAgentAssemblyVersion, adlaJobDnsSuffix, handlers);
        }

        /// <summary>
        /// Initializes a new instance of the DataLakeAnalyticsJobManagementExtensionClient class.
        /// </summary>
        /// <param name='credentials'>
        /// Required. Gets Azure subscription credentials.
        /// </param>
        /// <param name='rootHandler'>
        /// Optional. The http client handler used to handle http transport.
        /// </param>
        /// <param name='userAgentAssemblyVersion'>
        /// Optional. The version string that should be sent in the user-agent header for all requests. The default is the current version of the SDK.
        /// </param>
        /// <param name='adlaJobDnsSuffix'>
        /// Optional. The dns suffix to use for all requests for this client instance. The default is 'azuredatalakeanalytics.net'.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        public DataLakeAnalyticsJobManagementExtensionClient(ServiceClientCredentials credentials,
            HttpClientHandler rootHandler, string userAgentAssemblyVersion = "",
            string adlaJobDnsSuffix = DataLakeAnalyticsExtensionConstants.DefaultAdlaDnsSuffix,
            params DelegatingHandler[] handlers)
            : this(credentials, rootHandler, handlers)
        {
            this.AdlaJobDnsSuffix = adlaJobDnsSuffix;
            DataLakeAnalyticsCustomizationHelper.UpdateUserAgentAssemblyVersion(this, userAgentAssemblyVersion);
            BaseClient = new DataLakeAnalyticsJobManagementClient(credentials, rootHandler, userAgentAssemblyVersion, adlaJobDnsSuffix, handlers);
        }

        /// <summary>
        /// Initializes a new instance of the DataLakeAnalyticsJobManagementClient class.
        /// </summary>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        private DataLakeAnalyticsJobManagementExtensionClient(params DelegatingHandler[] handlers) : base(handlers)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the DataLakeAnalyticsJobManagementClient class.
        /// </summary>
        /// <param name='rootHandler'>
        /// Optional. The http client handler used to handle http transport.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        private DataLakeAnalyticsJobManagementExtensionClient(HttpClientHandler rootHandler, params DelegatingHandler[] handlers) : base(rootHandler, handlers)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the DataLakeAnalyticsJobManagementClient class.
        /// </summary>
        /// <param name='credentials'>
        /// Required. Credentials needed for the client to connect to Azure.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        internal DataLakeAnalyticsJobManagementExtensionClient(ServiceClientCredentials credentials, params DelegatingHandler[] handlers) : this(handlers)
        {
            if (credentials == null)
            {
                throw new System.ArgumentNullException("credentials");
            }
            Credentials = credentials;
            if (Credentials != null)
            {
                Credentials.InitializeServiceClient(this);
            }
        }

        /// <summary>
        /// Initializes a new instance of the DataLakeAnalyticsJobManagementClient class.
        /// </summary>
        /// <param name='credentials'>
        /// Required. Credentials needed for the client to connect to Azure.
        /// </param>
        /// <param name='rootHandler'>
        /// Optional. The http client handler used to handle http transport.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        internal DataLakeAnalyticsJobManagementExtensionClient(ServiceClientCredentials credentials, HttpClientHandler rootHandler, params DelegatingHandler[] handlers) : this(rootHandler, handlers)
        {
            if (credentials == null)
            {
                throw new System.ArgumentNullException("credentials");
            }
            Credentials = credentials;
            if (Credentials != null)
            {
                Credentials.InitializeServiceClient(this);
            }
        }

        private void Initialize()
        {
            Extension = new ExtensionOperations(this);
            BaseUri = "https://{accountName}.{adlaJobDnsSuffix}";
            ApiVersion = "2017-09-01-preview";
            AdlaJobDnsSuffix = "azuredatalakeanalytics.net";
            AcceptLanguage = "en-US";
            LongRunningOperationRetryTimeout = 30;
            GenerateClientRequestId = true;
            SerializationSettings = new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
                ContractResolver = new ReadOnlyJsonContractResolver(),
                Converters = new List<JsonConverter>
                    {
                        new Iso8601TimeSpanConverter()
                    }
            };
            DeserializationSettings = new JsonSerializerSettings
            {
                DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
                ContractResolver = new ReadOnlyJsonContractResolver(),
                Converters = new List<JsonConverter>
                    {
                        new Iso8601TimeSpanConverter()
                    }
            };
            SerializationSettings.Converters.Add(new PolymorphicSerializeJsonConverter<JobProperties>("type"));
            DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<JobProperties>("type"));
            SerializationSettings.Converters.Add(new PolymorphicSerializeJsonConverter<CreateJobProperties>("type"));
            DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<CreateJobProperties>("type"));
            DeserializationSettings.Converters.Add(new CloudErrorJsonConverter());
        }
    }
}
