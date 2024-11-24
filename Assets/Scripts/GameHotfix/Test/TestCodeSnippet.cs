/// <summary>
/// 2024-06-11 Game Framework Code By Hurley
/// </summary>

using NovaEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Game
{
    /// <summary>
    /// 测试代码片段
    /// </summary>
    public class TestCodeSnippet : ITestCase
    {
        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public void Update()
        {
            if (UnityEngine.Input.anyKey)
            {
                if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.A))
                {
                    Test6();
                }
                else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.B))
                {
                }
            }
        }

        public void Test1()
        {
            string input = "Example!String-with_symbols?heLlo";
            // string output = Regex.Replace(input, @"([^\w])(\w)", m => $"{m.Groups[1]}{m.Groups[2].Value.ToUpper()}");
            string output = Regex.Replace(input, @"([^\p{L}\p{N}])(\p{L})", m => $"{m.Groups[1]}{char.ToUpper(m.Groups[2].Value[0])}");
            Debugger.Warn(output);
            Debugger.Warn(input.ToLargeHumpFormat());
        }

        public void Test2()
        {
        }

        public void Test3()
        {
            Debugger.Log(NovaEngine.Utility.Resource.StreamingAssetsPath);
            Debugger.Log(NovaEngine.Utility.Resource.ApplicationDataPath);
            Debugger.Log(NovaEngine.Utility.Resource.PersistentDataPath);
            Debugger.Log(NovaEngine.Utility.Resource.TemporaryCachePath);
            Debugger.Log(NovaEngine.Utility.Resource.ApplicationContentPath);
        }

        public void EmptyCall(int v) { }

        public void Test4()
        {
            Debugger.Info("_________________________________________________");
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            ArrayList list = new ArrayList();

            int count = 10000000;
            for (int n = 0; n < count; ++n)
            {
                list.Add(n + 1);
            }

            stopwatch.Start();
            for (int n = 0; n < list.Count; ++n)
            {
                int v = (int) list[n];
                EmptyCall(v);
            }
            stopwatch.Stop();
            Debugger.Warn($"Run soldier for call for {count} times, using elapsed time was {stopwatch.ElapsedMilliseconds}.");

            stopwatch.Restart();
            foreach (int n in list)
            {
                EmptyCall(n);
            }
            stopwatch.Stop();
            Debugger.Warn($"Run soldier foreach call for {count} times, using elapsed time was {stopwatch.ElapsedMilliseconds}.");

            stopwatch.Restart();
            IEnumerator e = list.GetEnumerator();
            while (e.MoveNext())
            {
                EmptyCall((int) e.Current);
            }
            stopwatch.Stop();
            Debugger.Warn($"Run soldier while call for {count} times, using elapsed time was {stopwatch.ElapsedMilliseconds}.");
        }

        public void Test5()
        {
            IDictionary<int, int> d1 = new Dictionary<int, int>();
            IDictionary<string, int> d2 = new Dictionary<string, int>();

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            string methodName = "Test5";

            int count = 1000000;
            for (int n = 0; n < count; ++n)
            {
                d1.Add(n, n + 1);
                d2.Add(n.ToString(), n + 1);
            }

            stopwatch.Start();
            for (int n = 0; n < d1.Count; ++n)
            {
                int hashCode = methodName.GetHashCode();
                int key = n;
                if (d1.ContainsKey(key))
                {
                    int ret = d1[key] + hashCode;
                }
            }
            stopwatch.Stop();
            Debugger.Warn($"Run soldier for call for {count} times, using elapsed time was {stopwatch.ElapsedMilliseconds}.");

            int b = methodName.GetHashCode();
            stopwatch.Restart();
            for (int n = 0; n < d2.Count; ++n)
            {
                string key = n.ToString();
                if (d2.ContainsKey(key))
                {
                    int ret = d2[key] + b;
                }
            }
            stopwatch.Stop();
            Debugger.Warn($"Run soldier foreach call for {count} times, using elapsed time was {stopwatch.ElapsedMilliseconds}.");
        }

        public enum LogFormatParameterType
        {
            Unknown,
            Digit,
            String,
            Object,
            ClassType,
            Max,
        }

        public struct LogFormatConvertionInfo
        {
            public LogFormatParameterType parameterType;
            public string formatSymbol;
        }

        LogFormatConvertionInfo[] infos = new LogFormatConvertionInfo[(int) LogFormatParameterType.Max] {
                new LogFormatConvertionInfo { parameterType = LogFormatParameterType.Unknown, formatSymbol = string.Empty },
                new LogFormatConvertionInfo { parameterType = LogFormatParameterType.Digit, formatSymbol = "d" },
                new LogFormatConvertionInfo { parameterType = LogFormatParameterType.String, formatSymbol = "s" },
                new LogFormatConvertionInfo { parameterType = LogFormatParameterType.Object, formatSymbol = "o" },
                new LogFormatConvertionInfo { parameterType = LogFormatParameterType.ClassType, formatSymbol = "f" },
            };

        private LogFormatParameterType FindLogFormatConvertionInfoBySymbolName(string symbolName)
        {
            for (int n = 0; n < infos.Length; ++n)
            {
                if (symbolName.Equals(infos[n].formatSymbol))
                {
                    return infos[n].parameterType;
                }
            }

            return LogFormatParameterType.Unknown;
        }

        public void Test6()
        {
            

            string str = "Could not found {} object '{{%f}}' with target '{c}' name '{%s}' and type '{%d}', added object instance count '{4213}' failed!{100}";
            string pattern = @"\{([^\{\}]+)\}";
            //string pattern = @"\{(.*?)\}";
            //string pattern2 = @"\{(\d+)\}";
            string pattern2 = @"(\d+)";
            System.Text.RegularExpressions.MatchCollection matches = System.Text.RegularExpressions.Regex.Matches(str, pattern);
            Debugger.Warn("{0}", str);
            int startpos = 0, endpos = 0;
            StringBuilder sb = new StringBuilder();
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                sb.Append(str.Substring(startpos, match.Index - startpos));
                startpos = match.Index + match.Value.Length;
                if (match.Value.Length <= 2)
                {
                    Debugger.Warn("error !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    continue;
                }
                string text = match.Value.Substring(1, match.Value.Length - 2);
                Debugger.Warn("{0} - {1}", match.Index, text);

                bool isDigit = System.Text.RegularExpressions.Regex.IsMatch(text, pattern2);
                if (isDigit)
                {
                    sb.Append("{0}");
                }
                else
                {
                    if (text.Length > 1 && text[0] == '%')
                    {
                        LogFormatParameterType parameterType = FindLogFormatConvertionInfoBySymbolName(text.Substring(1));
                        if (LogFormatParameterType.Unknown == parameterType)
                        {
                            Debugger.Warn("error on text {0}", text);
                        }
                        else
                        {
                            switch (parameterType)
                            {
                                case LogFormatParameterType.Digit:
                                    sb.Append("111");
                                    break;
                                case LogFormatParameterType.String:
                                    sb.Append("hello");
                                    break;
                                case LogFormatParameterType.Object:
                                    sb.Append("obj");
                                    break;
                                case LogFormatParameterType.ClassType:
                                    sb.Append("classtype");
                                    break;
                            }
                        }
                    }
                    else
                    {
                        Debugger.Warn("error on text {0}", text);
                    }
                }
            }
            if (str.Length > startpos)
            {
                sb.Append(str.Substring(startpos));
            }

            Debugger.Warn("{0}", sb.ToString());

            bool b1 = int.TryParse("19z", out int a1);
            bool b2 = int.TryParse("194", out int a2);
            bool b3 = int.TryParse("19.13", out int a3);
            Debugger.Warn($"a1={a1},b1={b1},a2={a2},b2={b2},a3={a3},b3={b3}");

            IList<int> list1 = new List<int>();
            list1.Add(5);
            list1.Add(10);
            list1.Add(15);
            IList<string> list2 = new List<string>();
            list2.Add("hello");
            list2.Add("world");
            list2.Add("yukie");

            Debugger.Warn("list1 = {%s}, list2 = {%s}.", NovaEngine.Utility.Text.ToString<int>(list1), NovaEngine.Utility.Text.ToString<string>(list2));

            IDictionary<int, string> dict1 = new Dictionary<int, string>();
            dict1.Add(1, "bilibili");
            dict1.Add(2, "bobo");
            Debugger.Warn("dict1 = {%s}.", NovaEngine.Utility.Text.ToString<int, string>(dict1));

            str = "Could not found object '{{{%d}}}' with target '{%s}' name '{%t}' and type '{%o}', collection is '{%m}' and '{%m}' - '{%m}' added object instance count '{7}' failed!{8}";
            string str2 = NovaEngine.Utility.Text.Format(str, 100, "hello", "good", this, list1, list2, dict1, 99, 101, "hello", 191);
            Debugger.Warn(str2);
        }
    }
}
