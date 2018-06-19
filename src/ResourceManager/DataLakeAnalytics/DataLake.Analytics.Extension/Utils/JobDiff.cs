namespace Microsoft.Azure.Management.DataLake.Analytics.Extension.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using Microsoft.Azure.Management.DataLake.Analytics.Extension.Models;
    using Microsoft.Azure.Management.DataLake.Analytics.Models;

    internal enum PropCompareResult
    {
        None, // it's for grouping result into sections
        Same,
        Different,
        Close,
        Far,
        Missing1,
        Missing2
    }

    internal class PropertyDiff
    {
        internal PropCompareResult CompareResult;

        internal string Name;

        internal string Ratio;

        internal string Threshold;

        internal string Value1;

        internal string Value2;
    }

    internal class TextLine : IComparable
    {
        public int Hash;

        public string Line;

        public TextLine(string str)
        {
            Line = str.Replace("\t", "    ");
            Hash = str.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            return Hash.CompareTo(((TextLine)obj).Hash);
        }
    }

    internal interface IDiffList
    {
        int Count();

        IComparable GetByIndex(int index);
    }

    internal class TextDiffList : IDiffList
    {
        private readonly ArrayList _lines;

        private const int MaxLineLength = 1024;

        public TextDiffList(string text)
        {
            _lines = new ArrayList();
            using (var stringReader = new StringReader(text))
            {
                string str;
                while ((str = stringReader.ReadLine()) != null)
                {
                    if (str.Length > MaxLineLength)
                    {
                        throw new InvalidOperationException(
                            string.Format("File contains a line greater than {0} characters.", MaxLineLength));
                    }
                    _lines.Add(new TextLine(str));
                }
            }
        }

        public int Count()
        {
            return _lines.Count;
        }

        public IComparable GetByIndex(int index)
        {
            return (TextLine)_lines[index];
        }
    }

    internal partial class JobDiff
    {
        private readonly JobDiffInfo _leftJobInfo;

        private readonly JobDiffInfo _rightJobInfo;

        private List<PropertyDiff> _propDiffs;

        private ArrayList _scriptDiff;

        private TextDiffList _scriptDiffList1;

        private TextDiffList _scriptDiffList2;

        private readonly double _throughputThreshold;

        private readonly double _vertexThreshold;

        private readonly double _inputThreadHold;

        internal JobDiff(JobDiffInfo jobInfo1, JobDiffInfo jobInfo2)
        {
            _vertexThreshold = 30;
            _throughputThreshold = 60;
            _inputThreadHold = 10;
            _leftJobInfo = jobInfo1;
            _rightJobInfo = jobInfo2;
        }

        private void DiffProperty<T>(string propName, T prop1, T prop2)
        {
            if (string.IsNullOrWhiteSpace(propName) || prop1 == null || prop2 == null)
            {
                return;
            }
            var propertyDiff = new PropertyDiff
            {
                Name = propName,
                Value1 = prop1.ToString(),
                Value2 = prop2.ToString(),
                CompareResult =
                    prop1.Equals(prop2) ? PropCompareResult.Same : PropCompareResult.Different
            };
            _propDiffs.Add(propertyDiff);
        }

        private void DiffWithThreshold(string propName, double prop1, double prop2, bool printIfSame, PropFormat format,
            double threshold)
        {
            var propertyDiff = new PropertyDiff
            {
                Name = propName,
                Value1 = UnitFormat(format, prop1),
                Value2 = UnitFormat(format, prop2)
            };
            if (Math.Abs(prop1 - prop2) < Epsilon)
            {
                propertyDiff.CompareResult = PropCompareResult.Same;
                if (printIfSame)
                {
                    _propDiffs.Add(propertyDiff);
                }
                return;
            }
            propertyDiff.Threshold = string.Format("{0}%", threshold);
            if (format == PropFormat.Normal && prop1 < 10 && prop2 < 10)
            {
                propertyDiff.CompareResult = PropCompareResult.Close;
                if (printIfSame)
                {
                    _propDiffs.Add(propertyDiff);
                }
            }
            else if (Math.Abs(prop1 - prop2) * 100 / Math.Max(prop1, prop2) <= threshold)
            {
                propertyDiff.CompareResult = PropCompareResult.Close;
                if (printIfSame)
                {
                    _propDiffs.Add(propertyDiff);
                }
            }
            else
            {
                propertyDiff.Ratio = string.Format("{0:P}", Math.Abs(prop1 - prop2) / Math.Max(prop1, prop2));
                propertyDiff.CompareResult = PropCompareResult.Far;
                _propDiffs.Add(propertyDiff);
            }
        }

        protected double Epsilon
        {
            get { return 0.0001; }
        }

        internal void Run()
        {
            ConsoleReport.Report("", new object[0]); // add a new line
            ConsoleReport.ReportTimestamp("comparing jobs...", new object[0]);
            _propDiffs = new List<PropertyDiff>();

            //ConsoleReport.ReportTimestamp("comparing resources...", new object[0]);
            //DiffResources(_leftJobInfo.Resources, _rightJobInfo.Resources);
            ConsoleReport.ReportTimestamp("comparing stages...", new object[0]);
            _propDiffs.Add(new PropertyDiff()
            {
                CompareResult = PropCompareResult.None,
                Name = "Job Stages"
            });
            CompareJobStatisticsVertexStage(_leftJobInfo.JobStatistics.Stages, _rightJobInfo.JobStatistics.Stages);

            ConsoleReport.ReportTimestamp("comparing overall job stats...", new object[0]);
            _propDiffs.Add(new PropertyDiff()
            {
                CompareResult = PropCompareResult.None,
                Name = "Summary"
            });

            DiffProperty("Runtime Version", _leftJobInfo.RuntimeVersion, _rightJobInfo.RuntimeVersion);
            DiffWithThreshold("InputDataSize", _leftJobInfo.TotalInputDataSize, _rightJobInfo.TotalInputDataSize, true,
                PropFormat.Size, _inputThreadHold);
            TimeSpan? leftTimeSpan = null;
            TimeSpan? rightTimeSpan = null;
            if (_leftJobInfo.JobInfo == null || _rightJobInfo.JobInfo == null)
            {
                Console.WriteLine("Can't get job information");
                return;
            }
            var leftEndTime = _leftJobInfo.JobInfo.EndTime;
            var leftStartTime = _leftJobInfo.JobInfo.StartTime;
            if (leftEndTime.HasValue && leftStartTime.HasValue)
            {
                leftTimeSpan = leftEndTime.GetValueOrDefault() - leftStartTime.GetValueOrDefault();
            }
            var rightEndTime = _rightJobInfo.JobInfo.EndTime;
            var rightStartTime = _rightJobInfo.JobInfo.StartTime;
            if (rightEndTime.HasValue && rightStartTime.HasValue)
            {
                rightTimeSpan = rightEndTime.GetValueOrDefault() - rightStartTime.GetValueOrDefault();
            }
            DiffWithThreshold("Running Time", leftTimeSpan.Value.TotalSeconds, rightTimeSpan.Value.TotalSeconds, true,
                PropFormat.Time, _vertexThreshold);
            DiffProperty("Name", _leftJobInfo.JobInfo.Name, _rightJobInfo.JobInfo.Name);
            DiffProperty("User", _leftJobInfo.JobInfo.Submitter, _rightJobInfo.JobInfo.Submitter);
            DiffProperty("Status", _leftJobInfo.JobInfo.State, _rightJobInfo.JobInfo.State);
            DiffProperty("DegreeOfParallelism", _leftJobInfo.JobInfo.DegreeOfParallelism,
                _rightJobInfo.JobInfo.DegreeOfParallelism);
            DiffProperty("Priority", _leftJobInfo.JobInfo.Priority, _rightJobInfo.JobInfo.Priority);


            ConsoleReport.ReportTimestamp("comparing scripts...", new object[0]);
            if (!_leftJobInfo.JobInfo.Properties.Script.Equals(_rightJobInfo.JobInfo.Properties.Script))
            {
                _scriptDiffList1 = new TextDiffList(_leftJobInfo.JobInfo.Properties.Script);
                _scriptDiffList2 = new TextDiffList(_rightJobInfo.JobInfo.Properties.Script);
                var diffEngine = new DiffEngine();
                diffEngine.ProcessDiff(_scriptDiffList1, _scriptDiffList2, DiffEngineLevel.SlowPerfect);
                _scriptDiff = diffEngine.DiffReport();
            }
            else
            {
                _scriptDiff = null;
            }
        }

        private void CompareJobStatisticsVertexStage(IList<JobStatisticsVertexStage> vertexStats1, IList<JobStatisticsVertexStage> vertexStats2)
        {
            if (vertexStats1 == null || vertexStats2 == null)
            {
                return;
            }
            if (vertexStats1.Count != vertexStats2.Count)
            {
                var diff = new PropertyDiff
                {
                    Name = "# stages",
                    Value1 = vertexStats1.Count.ToString(CultureInfo.InvariantCulture),
                    Value2 = vertexStats2.Count.ToString(CultureInfo.InvariantCulture),
                    CompareResult = PropCompareResult.Different
                };
                _propDiffs.Add(diff);
            }

            foreach (var stats1 in vertexStats1)
            {
                var missing2 = true;
                foreach (var stats2 in vertexStats2)
                {
                    if (!stats1.StageName.Equals(stats2.StageName))
                    {
                        continue;
                    }

                    missing2 = false;
                    ConsoleReport.ReportTimestamp("  comparing {0}...", new object[] { stats1.StageName });
                    CompareJobStatisticsVertexStage(stats1, stats2);
                    break;
                }

                if (missing2)
                {
                    var diff = new PropertyDiff
                    {
                        Name = stats1.StageName,
                        CompareResult = PropCompareResult.Missing2
                    };
                    _propDiffs.Add(diff);
                }
            }

            foreach (var stats2 in vertexStats2)
            {
                var missing1 = true;
                foreach (var stats1 in vertexStats1)
                {
                    if (!stats1.StageName.Equals(stats2.StageName)) continue;
                    missing1 = false;
                    break;
                }

                if (missing1)
                {
                    var diff = new PropertyDiff
                    {
                        Name = stats2.StageName,
                        CompareResult = PropCompareResult.Missing1
                    };
                    _propDiffs.Add(diff);
                }
            }
        }

        private void CompareJobStatisticsVertexStage(JobStatisticsVertexStage s1, JobStatisticsVertexStage s2)
        {
            if (s1 == null || s2 == null)
            {
                return;
            }

            var flag = s1.StageName.StartsWith(DataLakeAnalyticsExtensionConstants.OverAllJobStageName);
            var vertexClassName = s1.StageName;
            if (flag)
            {
                vertexClassName = "Overall vertex";
            }
            var totalTimeCompleted1 = s1.TotalSucceededTime;
            var totalTimeCompleted2 = s2.TotalSucceededTime;
            if (totalTimeCompleted1 != null && totalTimeCompleted1.Value.TotalSeconds > 0 &&
                totalTimeCompleted2 != null && totalTimeCompleted2.Value.TotalSeconds > 0)
            {
                DiffWithThreshold(string.Format("{0} throughput", vertexClassName),
               (double)(s1.DataRead + s1.DataWritten) / totalTimeCompleted1.Value.TotalSeconds,
               (double)(s2.DataRead + s2.DataWritten) / totalTimeCompleted2.Value.TotalSeconds, flag,
               PropFormat.Throughput, _throughputThreshold);
            }

            if (s1.DataRead != null && s2.DataRead != null)
            {
                DiffWithThreshold(string.Format("{0} data read", vertexClassName), (double)s1.DataRead,
                    (double)s2.DataRead, flag, PropFormat.Size, _vertexThreshold);
            }
            if (s1.DataWritten != null && s2.DataWritten != null)
            {
                DiffWithThreshold(string.Format("{0} data written", vertexClassName), (double)s1.DataWritten,
                    (double)s2.DataWritten, flag,
                    PropFormat.Size, _vertexThreshold);
            }
            if (s1.FailedCount != null && s2.FailedCount != null)
            {
                DiffWithThreshold(string.Format("{0} failures", vertexClassName), (double)s1.FailedCount,
                    (double)s2.FailedCount, flag,
                    PropFormat.Normal, _vertexThreshold);
            }
            if (s1.ReadFailureCount != null && s2.ReadFailureCount != null)
            {
                DiffWithThreshold(string.Format("{0} read errors", vertexClassName), s1.ReadFailureCount.Value,
                    s2.ReadFailureCount.Value, flag, PropFormat.Normal, _vertexThreshold);
            }
            if (s1.RevocationCount != null && s2.RevocationCount != null)
            {
                DiffWithThreshold(string.Format("{0} revocations", vertexClassName), s1.RevocationCount.Value,
                    s2.RevocationCount.Value, flag, PropFormat.Normal, _vertexThreshold);
            }
        }

        internal void ShowDiff()
        {
            //ConsoleReport.Report("===============================================================================", new object[0]);
            ShowScriptDiff();
            foreach (PropertyDiff propDiff in _propDiffs)
            {
                PropCompareResult compareResult = propDiff.CompareResult;
                switch (compareResult)
                {
                    case PropCompareResult.None:
                        {
                            ConsoleReport.Report("\r\n=========={0}=========", new object[] { propDiff.Name });
                            break;
                        }
                    case PropCompareResult.Same:
                        {
                            ConsoleReport.ReportSame(propDiff.Name, propDiff.Value1);
                            break;
                        }
                    case PropCompareResult.Different:
                        {
                            ConsoleReport.ReportDifferent(propDiff.Name, propDiff.Value1, propDiff.Value2);
                            break;
                        }
                    case PropCompareResult.Close:
                        {
                            ConsoleReport.ReportClose(propDiff.Name, propDiff.Value1, propDiff.Value2, propDiff.Threshold);
                            break;
                        }
                    case PropCompareResult.Far:
                        {
                            ConsoleReport.ReportFar(propDiff.Name, propDiff.Value1, propDiff.Value2, propDiff.Ratio,
                                propDiff.Threshold);
                            break;
                        }
                    case PropCompareResult.Missing1:
                        {
                            ConsoleReport.ReportMissing1(propDiff.Name);
                            break;
                        }
                    case PropCompareResult.Missing2:
                        {
                            ConsoleReport.ReportMissing2(propDiff.Name);
                            break;
                        }
                }
            }
        }

        private void ShowScriptDiff()
        {
            ConsoleReport.Report("\r\n==========Script=========", new object[0]);
            if (_scriptDiff == null)
            {
                ConsoleReport.Report("= scripts are exactly the same", new object[0]);
                return;
            }

            var changeCount = 0;
            var totalCount = 0;
            foreach (DiffResultSpan span in _scriptDiff)
            {
                totalCount += span.Length;
                if (span.Status != DiffResultSpanStatus.NoChange)
                {
                    changeCount += span.Length;
                }
            }

            ConsoleReport.Report("* Scripts differ by {0:P} - {1} out of {2} lines are different",
                new object[] { (float)changeCount / totalCount, changeCount, totalCount });
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (DiffResultSpan diffResultSpan in _scriptDiff)
            {
                switch (diffResultSpan.Status)
                {
                    case DiffResultSpanStatus.Replace:
                        for (var i = 0; i < diffResultSpan.Length; i++)
                        {
                            ConsoleReport.Report(" > {0}",
                                new object[]
                                {((TextLine) _scriptDiffList1.GetByIndex(diffResultSpan.SourceIndex + i)).Line});
                            ConsoleReport.Report(" < {0}\r\n",
                                new object[]
                                {((TextLine) _scriptDiffList2.GetByIndex(diffResultSpan.DestIndex + i)).Line});
                        }
                        continue;

                    case DiffResultSpanStatus.DeleteSource:
                        for (var j = 0; j < diffResultSpan.Length; j++)
                        {
                            ConsoleReport.Report(" > {0}\r\n",
                                new object[]
                                {((TextLine) _scriptDiffList1.GetByIndex(diffResultSpan.SourceIndex + j)).Line});
                        }
                        continue;

                    case DiffResultSpanStatus.AddDestination:
                        for (var k = 0; k < diffResultSpan.Length; k++)
                        {
                            ConsoleReport.Report(" < {0}\r\n",
                                new object[]
                                {((TextLine) _scriptDiffList2.GetByIndex(diffResultSpan.DestIndex + k)).Line});
                        }
                        continue;

                    case DiffResultSpanStatus.NoChange:
                    default:
                        continue;
                }
            }
            Console.ResetColor();
        }

        private string SizeToString(double size)
        {
            if (size < 1024.0)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0} bytes", new object[] { size });
            }
            if (size < 1024.0 * 1024.0)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0:F2} KiBs", new object[] { size / 1024.0 });
            }
            if (size < 1024.0 * 1024.0 * 1024.0)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0:F2} MiBs", new object[] { (size / 1024.0) / 1024.0 });
            }
            if (size < 1024.0 * 1024.0 * 1024.0 * 1024.0)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0:F2} GiBs",
                    new object[] { ((size / 1024.0) / 1024.0) / 1024.0 });
            }

            return string.Format(CultureInfo.InvariantCulture, "{0:F2} TiBs",
                new object[] { (((size / 1024.0) / 1024.0) / 1024.0) / 1024.0 });
        }

        private static string TimeToString(double seconds)
        {
            if (seconds < 60.0)
            {
                return string.Format("{0} seconds", seconds);
            }
            if (seconds < 60.0 * 60.0)
            {
                return string.Format("{0} min {1:F0} sec", (int)seconds / 60, seconds % 60.0);
            }
            if (seconds < 60.0 * 60.0 * 24)
            {
                return string.Format("{0} hours {1:F0} min", (int)seconds / 3600, (seconds % 3600.0) / 60.0);
            }
            return string.Format("{0:F2} hours", seconds / 3600.0);
        }

        private enum PropFormat
        {
            Size,
            Time,
            Throughput,
            Normal
        }

        private string UnitFormat(PropFormat format, double val)
        {
            switch (format)
            {
                case PropFormat.Size:
                    return SizeToString(val);
                case PropFormat.Time:
                    return TimeToString(val);
                case PropFormat.Throughput:
                    if (val >= 1024 * 1024)
                    {
                        return string.Format("{0:F2} MiB/sec", val / 1024 / 1024);
                    }
                    return val >= 1024
                        ? string.Format("{0:F2} KiB/sec", val / 1024)
                        : string.Format("{0:F2} Bytes/sec", val);
                default:
                    return val.ToString(CultureInfo.InvariantCulture);
            }
        }
    }

    internal class DiffState
    {
        private int _length;

        private const int BadIndex = -1;

        public int EndIndex
        {
            get { return StartIndex + _length - 1; }
        }

        public int Length
        {
            get
            {
                if (_length <= 0)
                {
                    return _length != 0 ? 0 : 1;
                }

                return _length;
            }
        }

        public int StartIndex { get; private set; }

        public DiffStatus Status
        {
            get
            {
                if (_length > 0)
                {
                    return DiffStatus.Matched;
                }
                return _length == BadIndex ? DiffStatus.NoMatch : DiffStatus.Unknown;
            }
        }

        public DiffState()
        {
            SetToUnkown();
        }

        public bool HasValidLength(int newStart, int newEnd, int maxPossibleDestLength)
        {
            if (_length > 0 && (maxPossibleDestLength < _length || StartIndex < newStart || EndIndex > newEnd))
            {
                SetToUnkown();
            }
            return _length != -2;
        }

        public void SetMatch(int start, int length)
        {
            StartIndex = start;
            _length = length;
        }

        public void SetNoMatch()
        {
            StartIndex = BadIndex;
            _length = BadIndex;
        }

        protected void SetToUnkown()
        {
            StartIndex = BadIndex;
            _length = -2;
        }
    }

    internal class DiffStateList
    {
        private readonly DiffState[] _array;

        public DiffStateList(int destCount)
        {
            _array = new DiffState[destCount];
        }

        public DiffState GetByIndex(int index)
        {
            var diffState = _array[index];
            if (diffState == null)
            {
                diffState = new DiffState();
                _array[index] = diffState;
            }
            return diffState;
        }
    }

    internal enum DiffEngineLevel
    {
        FastImperfect,
        Medium,
        SlowPerfect
    }

    internal enum DiffResultSpanStatus
    {
        NoChange,
        Replace,
        DeleteSource,
        AddDestination
    }

    internal class DiffResultSpan : IComparable
    {
        private readonly int _destIndex;

        private readonly int _sourceIndex;

        private readonly DiffResultSpanStatus _status;

        private const int BadIndex = -1;

        public int DestIndex
        {
            get { return _destIndex; }
        }

        public int Length { get; private set; }

        public int SourceIndex
        {
            get { return _sourceIndex; }
        }

        public DiffResultSpanStatus Status
        {
            get { return _status; }
        }

        protected DiffResultSpan(DiffResultSpanStatus status, int destIndex, int sourceIndex, int length)
        {
            _status = status;
            _destIndex = destIndex;
            _sourceIndex = sourceIndex;
            Length = length;
        }

        public void AddLength(int i)
        {
            Length += i;
        }

        public int CompareTo(object obj)
        {
            return _destIndex.CompareTo(((DiffResultSpan)obj)._destIndex);
        }

        public static DiffResultSpan CreateAddDestination(int destIndex, int length)
        {
            return new DiffResultSpan(DiffResultSpanStatus.AddDestination, destIndex, BadIndex, length);
        }

        public static DiffResultSpan CreateDeleteSource(int sourceIndex, int length)
        {
            return new DiffResultSpan(DiffResultSpanStatus.DeleteSource, BadIndex, sourceIndex, length);
        }

        public static DiffResultSpan CreateNoChange(int destIndex, int sourceIndex, int length)
        {
            return new DiffResultSpan(DiffResultSpanStatus.NoChange, destIndex, sourceIndex, length);
        }

        public static DiffResultSpan CreateReplace(int destIndex, int sourceIndex, int length)
        {
            return new DiffResultSpan(DiffResultSpanStatus.Replace, destIndex, sourceIndex, length);
        }

        public override string ToString()
        {
            return string.Format("{0} (Dest: {1},Source: {2}) {3}",
                new object[] { _status, _destIndex, _sourceIndex, Length });
        }
    }

    internal enum DiffStatus
    {
        Unknown = -2,
        NoMatch = -1,
        Matched = 1
    }

    internal class DiffEngine
    {
        private IDiffList _dest;

        private DiffEngineLevel _level;

        private ArrayList _matchList;

        private IDiffList _source;

        private DiffStateList _stateList;

        public DiffEngine()
        {
            _source = null;
            _dest = null;
            _matchList = null;
            _stateList = null;
            _level = DiffEngineLevel.FastImperfect;
        }

        private static bool AddChanges(IList report, int curDest, int nextDest, int curSource, int nextSource)
        {
            var flag = false;
            var destSpan = nextDest - curDest;
            var sourceSpan = nextSource - curSource;
            if (destSpan > 0)
            {
                if (sourceSpan > 0)
                {
                    var length = Math.Min(destSpan, sourceSpan);
                    report.Add(DiffResultSpan.CreateReplace(curDest, curSource, length));
                    if (destSpan > sourceSpan)
                    {
                        curDest += length;
                        report.Add(DiffResultSpan.CreateAddDestination(curDest, destSpan - sourceSpan));
                    }
                    else if (sourceSpan > destSpan)
                    {
                        curSource += length;
                        report.Add(DiffResultSpan.CreateDeleteSource(curSource, sourceSpan - destSpan));
                    }
                }
                else
                {
                    report.Add(DiffResultSpan.CreateAddDestination(curDest, destSpan));
                }
                return true;
            }
            if (sourceSpan > 0)
            {
                report.Add(DiffResultSpan.CreateDeleteSource(curSource, sourceSpan));
                flag = true;
            }
            return flag;

        }

        public ArrayList DiffReport()
        {
            var report = new ArrayList();
            var destCount = _dest.Count();
            var sourceCount = _source.Count();
            if (destCount == 0)
            {
                if (sourceCount > 0)
                {
                    report.Add(DiffResultSpan.CreateDeleteSource(0, sourceCount));
                }
                return report;
            }
            if (sourceCount == 0)
            {
                report.Add(DiffResultSpan.CreateAddDestination(0, destCount));
                return report;
            }
            _matchList.Sort();
            var curDest = 0;
            var curSource = 0;
            DiffResultSpan span = null;
            foreach (DiffResultSpan matchDiffResult in _matchList)
            {
                if (!AddChanges(report, curDest, matchDiffResult.DestIndex, curSource, matchDiffResult.SourceIndex) &&
                    (span != null))
                {
                    span.AddLength(matchDiffResult.Length);
                }
                else
                {
                    report.Add(matchDiffResult);
                }
                curDest = matchDiffResult.DestIndex + matchDiffResult.Length;
                curSource = matchDiffResult.SourceIndex + matchDiffResult.Length;
                span = matchDiffResult;
            }

            AddChanges(report, curDest, destCount, curSource, sourceCount);
            return report;

        }

        private void GetLongestSourceMatch(DiffState curItem, int destIndex, int destEnd, int sourceStart, int sourceEnd)
        {
            var num = destEnd - destIndex + 1;
            var length = 0;
            var start = -1;
            for (var i = sourceStart; i <= sourceEnd; i++)
            {
                var maxLength = Math.Min(num, sourceEnd - i + 1);
                if (maxLength <= length)
                {
                    break;
                }
                var sourceMatchLength = GetSourceMatchLength(destIndex, i, maxLength);
                if (sourceMatchLength > length)
                {
                    start = i;
                    length = sourceMatchLength;
                }
                i += length;
            }
            if (start != -1)
            {
                curItem.SetMatch(start, length);
            }
            else
            {
                curItem.SetNoMatch();
            }
        }

        // remove whitespaces before diffing
        private TextLine NormalizeLine(TextLine line)
        {
            StringBuilder sb = new StringBuilder(line.Line.Length);
            for (int i = 0; i < line.Line.Length; i++)
            {
                if (!Char.IsWhiteSpace(line.Line[i]))
                {
                    sb.Append(line.Line[i]);
                }
            }
            return new TextLine(sb.ToString());
        }

        private bool AreLinesEqualIgnoreWhitespace(TextLine left, TextLine right)
        {
            if (left.CompareTo(right) == 0)
            {
                return true;
            }
            return NormalizeLine(left).CompareTo(NormalizeLine(right)) == 0;
        }

        private int GetSourceMatchLength(int destIndex, int sourceIndex, int maxLength)
        {
            var num = 0;
            while (num < maxLength &&
                   AreLinesEqualIgnoreWhitespace(
                       (TextLine)_dest.GetByIndex(destIndex + num),
                       (TextLine)_source.GetByIndex(sourceIndex + num)
                       ))
            {
                num++;
            }
            return num;
        }

        public double ProcessDiff(IDiffList source, IDiffList destination, DiffEngineLevel level)
        {
            _level = level;
            return ProcessDiff(source, destination);
        }

        public double ProcessDiff(IDiffList source, IDiffList destination)
        {
            var now = DateTime.Now;
            _source = source;
            _dest = destination;
            _matchList = new ArrayList();
            var destCount = _dest.Count();
            var sourceCount = _source.Count();
            if (destCount > 0 && sourceCount > 0)
            {
                _stateList = new DiffStateList(destCount);
                ProcessRange(0, destCount - 1, 0, sourceCount - 1);
            }
            var timeSpan = DateTime.Now - now;
            return timeSpan.TotalSeconds;
        }

        private void ProcessRange(int destStart, int destEnd, int sourceStart, int sourceEnd)
        {
            var destIndex = -1;
            var length = -1;
            DiffState diffState = null;
            for (var i = destStart; i <= destEnd; i++)
            {
                var maxPossibleDestLength = destEnd - i + 1;
                if (maxPossibleDestLength <= length)
                {
                    break;
                }
                var curItem = _stateList.GetByIndex(i);
                if (!curItem.HasValidLength(sourceStart, sourceEnd, maxPossibleDestLength))
                {
                    GetLongestSourceMatch(curItem, i, destEnd, sourceStart, sourceEnd);
                }

                if (curItem.Status != DiffStatus.Matched) continue;

                switch (_level)
                {
                    case DiffEngineLevel.FastImperfect:
                        if (curItem.Length > length)
                        {
                            destIndex = i;
                            length = curItem.Length;
                            diffState = curItem;
                        }
                        i += curItem.Length - 1;
                        break;
                    case DiffEngineLevel.Medium:
                        if (curItem.Length > length)
                        {
                            destIndex = i;
                            length = curItem.Length;
                            diffState = curItem;
                            i += curItem.Length - 1;
                        }
                        break;
                    default:
                        if (curItem.Length > length)
                        {
                            destIndex = i;
                            length = curItem.Length;
                            diffState = curItem;
                        }
                        break;
                }
            }
            if (destIndex >= 0)
            {
                var startIndex = diffState.StartIndex;
                _matchList.Add(DiffResultSpan.CreateNoChange(destIndex, startIndex, length));
                if (destStart < destIndex && sourceStart < startIndex)
                {
                    ProcessRange(destStart, destIndex - 1, sourceStart, startIndex - 1);
                }
                var dest = destIndex + length;
                var source = startIndex + length;
                if (destEnd > dest && sourceEnd > source)
                {
                    ProcessRange(dest, destEnd, source, sourceEnd);
                }
            }
        }
    }
}
