namespace Microsoft.Azure.Commands.DataLakeAnalytics.Models
{
    public class ScopeEnvironmentParameters
    {
        public string ScopeWorkingDir;
        public string [] ScopePath;
        public string CppSdkPath;
        public string OutputStreamPath;
        public string InputStreamPath;

        public string NormalizeScopePath()
        {
            string norm_scope_path = null;

            if (this.ScopePath != null)
            {
                norm_scope_path = string.Join(";", this.ScopePath);
                norm_scope_path = norm_scope_path.Replace(",", ";");
            }

            return norm_scope_path;
        }
    }
}