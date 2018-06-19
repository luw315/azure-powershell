

namespace Microsoft.Azure.Management.DataLake.Analytics.Extension.Utils
{
    using System;
    internal static class ConsoleReport
    {
        internal static void Report(string format, object[] args)
        {
            Console.WriteLine(format, args);
        }

        internal static void ReportClose(string name, string value1, string value2, string threshold)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("~ {0} is within threshold {1}: ({2}) vs. ({3})", name, threshold, value1, value2);
            Console.ResetColor();
        }

        internal static void ReportDifferent(string name, string value1, string value2)
        {
            Console.WriteLine("! {0} is different", name);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  > job1: {0} {1}", value1, name);
            Console.WriteLine("  < job2: {0} {1}", value2, name);
            Console.ResetColor();
        }

        internal static void ReportFar(string name, string value1, string value2, string ratio, string threshold)
        {
            if (threshold == null)
            {
                Console.WriteLine("! {0} ratio {1} exceeds threshold", name, ratio);
            }
            else
            {
                Console.WriteLine("! {0} ratio {1} exceeds threshold {2}", name, ratio, threshold);
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  > job1: {0} {1}", value1, name);
            Console.WriteLine("  < job2: {0} {1}", value2, name);
            Console.ResetColor();
        }

        internal static void ReportMissing1(string name)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("> {0} is in job1, but not in job2", name);
            Console.ResetColor();
        }

        internal static void ReportMissing2(string name)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("< {0} is in job2, but not in job1", name);
            Console.ResetColor();
        }

        internal static void ReportSame(string name, string value)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("= {0} is the same: {1}", name, value);
            Console.ResetColor();
        }

        internal static void ReportTimestamp(string format, object[] args)
        {
            Report("{0} {1}", new object[] { DateTime.Now, string.Format(format, args) });
        }
    }
}
