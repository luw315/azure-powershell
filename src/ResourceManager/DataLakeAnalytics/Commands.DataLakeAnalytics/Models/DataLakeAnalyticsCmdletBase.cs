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

using Microsoft.Azure.Commands.Common.Authentication;
using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using Microsoft.Azure.Commands.DataLakeAnalytics.Properties;
using Microsoft.Azure.Commands.ResourceManager.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Hyak.Common;
using Microsoft.Azure.Management.DataLake.Analytics.Extension;
using Microsoft.Azure.Management.DataLake.Analytics.Models;
using Microsoft.Rest.Azure.Authentication;
using Microsoft.Rest;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.DataLakeAnalytics.Models
{
    /// <summary>
    ///     The base class for all Microsoft Azure DataLakeAnalytics Management cmdlets
    /// </summary>
    public abstract class DataLakeAnalyticsCmdletBase : AzureRMCmdlet
    {
        private DataLakeAnalyticsClient _bigAnalyticsClient;

        /// <summary>
        /// The filesystem request timeout in minutes, which is used for long running upload/download operations
        /// </summary>
        private const int filesystemRequestTimeoutInMinutes = 5;

        public DataLakeAnalyticsClient DataLakeAnalyticsClient
        {
            get
            {
                if (_bigAnalyticsClient == null)
                {
                    _bigAnalyticsClient = new DataLakeAnalyticsClient(DefaultProfile.DefaultContext);
                }
                return _bigAnalyticsClient;
            }

            set { _bigAnalyticsClient = value; }
        }

        protected override void BeginProcessing()
        {
            AppDomain.CurrentDomain.AssemblyResolve += BindingRedirect.CurrentDomain_BindingRedirect;
            base.BeginProcessing();
        }

        internal static TClient CreateAdlaClient<TClient>(IAzureContext context, string endpoint, bool parameterizedBaseUri = false) where TClient : Rest.ServiceClient<TClient>
        {
            if (context == null)
            {
                throw new ApplicationException(Properties.Resources.NoSubscriptionInContext);
            }

            var creds = AzureSession.Instance.AuthenticationFactory.GetServiceClientCredentials(context, endpoint);
            var clientFactory = AzureSession.Instance.ClientFactory;
            var newHandlers = clientFactory.GetCustomHandlers();
            TClient client;
            if (!parameterizedBaseUri)
            {
                client = (newHandlers == null || newHandlers.Length == 0)
                    // string.Empty ensures that we hit the constructors that set the assembly version properly
                    ? clientFactory.CreateCustomArmClient<TClient>(context.Environment.GetEndpointAsUri(endpoint), creds, string.Empty)
                    : clientFactory.CreateCustomArmClient<TClient>(context.Environment.GetEndpointAsUri(endpoint), creds, string.Empty, clientFactory.GetCustomHandlers());
            }
            else
            {
                client = (newHandlers == null || newHandlers.Length == 0)
                    ? clientFactory.CreateCustomArmClient<TClient>(creds, string.Empty, context.Environment.GetEndpoint(endpoint))
                    : clientFactory.CreateCustomArmClient<TClient>(creds, string.Empty, context.Environment.GetEndpoint(endpoint), clientFactory.GetCustomHandlers());
            }

            var subscriptionId = typeof(TClient).GetProperty("SubscriptionId");
            if (subscriptionId != null && context.Subscription != null)
            {
                subscriptionId.SetValue(client, context.Subscription.Id.ToString());
            }

            return client;
        }

        internal static TClient CreateAdlsClient<TClient>(IAzureContext context, string endpoint, bool parameterizedBaseUri = false) where TClient : Rest.ServiceClient<TClient>
        {
            if (context == null)
            {
                throw new ApplicationException(Properties.Resources.NoSubscriptionInContext);
            }

            var creds = AzureSession.Instance.AuthenticationFactory.GetServiceClientCredentials(context, endpoint);
            var clientFactory = AzureSession.Instance.ClientFactory;
            var newHandlers = clientFactory.GetCustomHandlers();
            TClient client;
            if (!parameterizedBaseUri)
            {
                client = (newHandlers == null || newHandlers.Length == 0)
                    // string.Empty ensures that we hit the constructors that set the assembly version properly
                    ? clientFactory.CreateCustomArmClient<TClient>(context.Environment.GetEndpointAsUri(endpoint), creds, string.Empty)
                    : clientFactory.CreateCustomArmClient<TClient>(context.Environment.GetEndpointAsUri(endpoint), creds, string.Empty, clientFactory.GetCustomHandlers());
            }
            else
            {
                client = (newHandlers == null || newHandlers.Length == 0)
                    // string.Empty ensures that we hit the constructors that set the assembly version properly
                    ? clientFactory.CreateCustomArmClient<TClient>(creds, string.Empty, context.Environment.GetEndpoint(endpoint), filesystemRequestTimeoutInMinutes)
                    : clientFactory.CreateCustomArmClient<TClient>(creds, string.Empty, context.Environment.GetEndpoint(endpoint), filesystemRequestTimeoutInMinutes, clientFactory.GetCustomHandlers());
            }

            var subscriptionId = typeof(TClient).GetProperty("SubscriptionId");
            if (subscriptionId != null && context.Subscription != null)
            {
                subscriptionId.SetValue(client, context.Subscription.Id.ToString());
            }

            return client;
        }

        internal string GetToken(IAzureContext context, string targetEndpoint)
        {
            ServiceClientCredentials creds = AzureSession.Instance.AuthenticationFactory.GetServiceClientCredentials(context, targetEndpoint);
            HttpRequestMessage message = new HttpRequestMessage();
            creds.ProcessHttpRequestAsync(message, CancellationToken.None).GetAwaiter().GetResult();
            return message.Headers.Authorization.Parameter;
        }

        internal static DataLakeAnalyticsJobManagementExtensionClient CreateAdlaExtensionClient(IAzureContext context, string endpoint, bool parameterizedBaseUri = false)
        {
            if (context == null)
            {
                throw new ApplicationException(Properties.Resources.NoSubscriptionInContext);
            }

            var creds = AzureSession.Instance.AuthenticationFactory.GetServiceClientCredentials(context, endpoint);

            var endpointRul = context.Environment.GetEndpoint(endpoint);
            if (endpointRul != null)
            {
                return new DataLakeAnalyticsJobManagementExtensionClient(creds,
                    adlaJobDnsSuffix: endpointRul);
            }
            else
            {
                return new DataLakeAnalyticsJobManagementExtensionClient(creds);
            }

        }

        #region scope
        protected Dictionary<string, string> CreateStringStringDictionaryCustProp(Hashtable hashtable)
        {
            return CreateStringStringDictionary(hashtable, true, true);
        }

        public string NormalizeScopePath(string[] scopePath)
        {
            string norm_scope_path = null;

            if (scopePath != null)
            {
                norm_scope_path = string.Join(";", scopePath);
                norm_scope_path = norm_scope_path.Replace(",", ";");
            }

            return norm_scope_path;
        }

        internal Dictionary<string, string> CreateStringStringDictionary(Hashtable hashtable, bool docustomprop, bool suppressquoting)
        {
            this.WriteVerbose("Building Dictionary");

            // If the hashtable is null don't both building a new dictionary
            if (hashtable == null)
            {
                return null;
            }

            var dic = new Dictionary<string, string>();

            foreach (var input_key in hashtable.Keys)
            {
                string output_key = input_key.ToString();
                object input_value_obj = hashtable[input_key];

                // null values are not allowed
                if (input_value_obj == null)
                {
                    string msg = string.Format("Value for key {0} is null", input_key);
                    throw new ArgumentException(msg);
                }

                // Now handle all non_null values

                // We'll need to understand the type to do the appropriate thing
                var input_value_type = input_value_obj.GetType();

                string output_value = null;

                if (input_value_type == typeof(string))
                {
                    // value is a string
                    output_value = input_value_obj.ToString();

                    if (!docustomprop)
                    {
                        output_value = this.Quote_value(output_key, output_value, suppressquoting);
                    }
                }
                else if (input_value_type == typeof(char) || input_value_type == typeof(int) || input_value_type == typeof(byte) || input_value_type == typeof(long) || input_value_type == typeof(ulong) || input_value_type == typeof(uint) || input_value_type == typeof(ushort) || input_value_type == typeof(ushort))
                {
                    // value is an integer of some kind
                    output_value = input_value_obj.ToString();
                }
                else if (input_value_type == typeof(bool))
                {
                    // value is an bool
                    bool input_value_bool = (bool)input_value_obj;
                    output_value = input_value_bool.ToString().ToLower();
                }
                else if (input_value_type == typeof(double))
                {
                    // value is double
                    double input_value_double = (double)input_value_obj;
                    output_value = input_value_double.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (input_value_type == typeof(float))
                {
                    // value is float
                    float input_value_float = (float)input_value_obj;
                    output_value = input_value_float.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    // value is an unsupported type
                    string msg = string.Format("Value for {0} has a type of {1} which is not supported", input_key,
                        input_value_obj);
                    throw new ArgumentException(msg);
                }

                dic[output_key] = output_value;
            }
            return dic;
        }

        private string Quote_value(string name, string value, bool suppressquoting)
        {
            // if quoting is surpressed then abandon all attempts at quoting
            if (suppressquoting)
            {
                return value;
            }

            // the value does not begin or end with a quote so add it
            string new_val = string.Format("\"{0}\"", value.Replace("\"", "\\\""));
            this.WriteVerbose(string.Format("Value for {0} quoted from \"{1}\" to \"{2}\"", name, value, new_val));
            return new_val;
        }

        protected void PrepareLocalScopeCompileEnvironment(ScopeEnvironmentParameters envParameters)
        {
            if (envParameters == null)
            {
                throw new ArgumentNullException(nameof(envParameters));
            }

            //ScopeClient.ScopeEnvironment.Instance.WorkingRoot = this.GetScopeWorkingDirectory(envParameters.ScopeWorkingDir);
            //ScopeClient.ScopeEnvironment.Instance.ScopePath = envParameters.NormalizeScopePath();
            //ScopeClient.ScopeEnvironment.Instance.OutputStreamPath = envParameters.OutputStreamPath;
            //ScopeClient.ScopeEnvironment.Instance.InputStreamPath = envParameters.InputStreamPath;

            if (envParameters.CppSdkPath != null)
            {
                System.Environment.SetEnvironmentVariable("SCOPE_CPP_SDK", envParameters.CppSdkPath);
            }

            //this.WriteVerbose(string.Format("Scope Working Dir: {0}",
            //    ScopeClient.ScopeEnvironment.Instance.WorkingRoot));
            //this.WriteVerbose(string.Format("Scope Path: {0}", ScopeClient.ScopeEnvironment.Instance.ScopePath));
            //this.WriteVerbose(string.Format("Scope InputStreamPath: {0}",
            //    ScopeClient.ScopeEnvironment.Instance.InputStreamPath));
            //this.WriteVerbose(string.Format("Scope OutputStreamPath: {0}",
            //    ScopeClient.ScopeEnvironment.Instance.OutputStreamPath));
            //this.WriteVerbose(string.Format("Scope TmpDirectory: {0}",
            //    ScopeClient.ScopeEnvironment.Instance.TmpDirectory));

            this.WriteVerbose(string.Format("Current Domain FriendlyName: {0}",
                System.AppDomain.CurrentDomain.FriendlyName));
            this.WriteVerbose(string.Format("Current Domain Directory: {0}",
                System.AppDomain.CurrentDomain.BaseDirectory));
        }

        protected string GetScopeWorkingDirectory(string path)
        {
            if (path == null)
            {
                string temp_path = Path.GetTempPath();
                return temp_path;
            }

            var p2 = SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);
            if (!Directory.Exists(p2.ToString()))
            {
                string msg = string.Format("Specified Scope Path \"{0}\" does not exist", path);
                throw new ArgumentException(msg);
            }
            return path;
        }

        public void ThrowErrorIfDirectoryDoesNotExist(string target_folder)
        {
            if (System.IO.Directory.Exists(target_folder))
            {
                return;
            }
            var exc =
                new InvalidOperationException(string.Format("{0} does not exist or is not a diretory", target_folder));
            throw exc;
        }

        public void CreateDirectoryIfNeeded(string target_folder)
        {
            if (!System.IO.Directory.Exists(target_folder))
            {
                System.IO.Directory.CreateDirectory(target_folder);
            }
        }

        protected string GetScopeCommandParams(Dictionary<string, string> dirt)
        {
            if (dirt == null || dirt.Count == 0)
            {
                return string.Empty;
            }
            var sb = new StringBuilder();
            foreach (var keyValuePair in dirt)
            {
                sb.Append(string.Format(" -params {0}={1}", keyValuePair.Key, keyValuePair.Value));
            }
            return sb.ToString();
        }

        #endregion
    }
}