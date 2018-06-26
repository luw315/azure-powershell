namespace Microsoft.Azure.Management.DataLake.InternalAnalytics.Scope.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Azure.Management.DataLake.Analytics.Models;

    public class CreateScopeJobExtensionParameters
    {
        public CreateScopeJobExtensionParameters()
        {
            
        }
        /// <summary>
        /// List of files that need to be uploaded with the job
        /// </summary>
        public IList<ScopeJobResource> EmbeddedFiles { get; set; }
    }
}
