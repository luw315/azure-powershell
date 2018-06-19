namespace Microsoft.Azure.Commands.DataLakeAnalytics.Models
{
    public class NebulaArguments
    {
        private string _options;

        public bool DisableBonusTokens;
        public string UserDebugStream;
        public int MaxUnavailability = -1;

        public NebulaArguments(string s)
        {
            this._options = s;
        }

        public override string ToString()
        {

            string NebulaArguments = this._options;

            if (this.DisableBonusTokens)
            {
                NebulaArguments = NebulaArguments ?? string.Empty;
                NebulaArguments = string.Format(" {0} {1} ", NebulaArguments, "-disableBonusTokens");
            }

            if (this.UserDebugStream != null)
            {
                NebulaArguments = NebulaArguments ?? string.Empty;
                NebulaArguments = string.Format(" {0} {1} {2}", NebulaArguments, "-UserDebugStream",
                    this.UserDebugStream);
            }

            if (this.MaxUnavailability >= 0)
            {
                NebulaArguments = NebulaArguments ?? string.Empty;
                NebulaArguments = string.Format(" {0} {1} {2}", NebulaArguments, "-maxUnavailability",
                    this.MaxUnavailability);
            }

            if (NebulaArguments != null)
            {
                NebulaArguments = NebulaArguments.Trim();
            }

            return NebulaArguments;
        }

    }
}
