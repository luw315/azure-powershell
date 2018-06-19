namespace Microsoft.Azure.Commands.DataLakeAnalytics.Models
{
    public class TokenAllocation
    {
        public int? Percent;
        public int? Absolute;

        public static TokenAllocation ParseTokens(string tokens)
        {
            var to = new TokenAllocation();
            if (tokens == null)
            {
                return new TokenAllocation();
            }

            var norm_token_str = tokens.Trim();

            if (norm_token_str.Length < 1)
            {
                return to;
            }

            bool is_percentage = norm_token_str.EndsWith("%");
            if (is_percentage)
            {
                norm_token_str = norm_token_str.Replace("%", "");
            }
            bool is_int;
            int n;
            is_int = int.TryParse(norm_token_str, out n);
            if (is_int)
            {
                if (is_percentage)
                {
                    to.Percent = n;
                    to.Absolute = null;
                }
                else
                {
                    to.Percent = null;
                    to.Absolute = n;
                }

                return to;
            }
            string msg = string.Format("Could not parse \"{0}\" as an integer", norm_token_str);
            throw new System.ArgumentException(msg);
        }
    }
}