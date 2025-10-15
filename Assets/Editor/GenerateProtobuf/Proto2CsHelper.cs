using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GenerateProtobuf
{
    internal class OpcodeInfo
    {
        public string name;
        public int opcode;
    }

    /// <summary>
    /// 协议转代码
    /// </summary>
    public static class Proto2CsHelper
    {
        /// <summary>
        /// 保存的Key值
        /// </summary>
        internal const string SaveKey = "EditorProto2CsHelper_ProtobufPath";

        /// <summary>
        /// 默认目录
        /// </summary>
        internal static readonly string defaultPath = Application.dataPath + "/../../common/proto/";

        /// <summary>
        /// 导出代码目录
        /// </summary>
        static readonly string s_outputCodePath = $"{Application.dataPath}/Scripts/Agen/Proto";

        /// <summary>
        /// 消息Opcode信息列表
        /// </summary>
        static readonly List<OpcodeInfo> s_msgOpcode = new();

        /// <summary>
        /// 分隔符
        /// </summary>
        static readonly char[] s_splitChars = { ' ', '\t', '=' };

        /// <summary>
        /// proto文件目录
        /// </summary>
        public static string ProtobufPath
        {
            get
            {
                string protobufPath = EditorPrefs.GetString(SaveKey, defaultPath);
                if (!Directory.Exists(protobufPath))
                {
                    ProtoPathSettingWindow.Open();
                    Debug.LogError("请先正确配置协议目录");
                    return null;
                }

                string[] files = Directory.GetFiles(protobufPath, "*.proto", SearchOption.AllDirectories);
                if (files.Length == 0)
                {
                    ProtoPathSettingWindow.Open();
                    Debug.LogError("配置的目录不存在proto文件");
                    return null;
                }

                return protobufPath;
            }
        }

        /// <summary>
        /// 开始生成
        /// </summary>
        public static void Start()
        {
            string protoPath = ProtobufPath;
            if (string.IsNullOrEmpty(protoPath))
            {
                return;
            }

            s_msgOpcode.Clear();

            string[] files = Directory.GetFiles(protoPath, "*.proto", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                LoadOpcode(file);
            }

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                GenerateCs("Game.Proto", fileName, file, s_outputCodePath, "ProtoOpcode");
            }

            GenerateOpcode("Game.Proto", "ProtoOpcode", s_outputCodePath);

            AssetDatabase.Refresh();

            Debug.Log("生成完毕");
        }

        static void LoadOpcode(string protoPath)
        {
            var protoContent = File.ReadAllText(protoPath);

            int parsedMsgId = 0;
            bool parsedMsgIdFlag = false;

            foreach (string line in protoContent.Split('\n'))
            {
                string newline = line.Trim();

                if (newline == "")
                {
                    continue;
                }

                if (newline.StartsWith("//"))
                {
                    if (newline.Contains("@id="))
                    {
                        parsedMsgId = int.Parse(line.Split('=')[1]);
                        parsedMsgIdFlag = true;
                    }

                    continue;
                }

                if (newline.StartsWith("message"))
                {
                    string msgName = newline.Split(s_splitChars, StringSplitOptions.RemoveEmptyEntries)[1];
                    msgName = ToPascalCase(msgName);
                    if (parsedMsgIdFlag)
                    {
                        s_msgOpcode.Add(new OpcodeInfo { name = msgName, opcode = parsedMsgId });
                        parsedMsgIdFlag = false;
                    }
                }
            }
        }

        static void GenerateCs(string ns, string name, string protoPath, string outputPath, string opcodeClassName)
        {
            //msgOpcode.Clear();
            string csPath = Path.Combine(outputPath, $"{ToPascalCase(name)}.cs");
            var protoContent = File.ReadAllText(protoPath);
            StringBuilder sb = new StringBuilder();
            sb.Append("using ProtoBuf;\n");
            sb.Append("using ProtoBuf.Extension;\n");
            sb.Append("using System.Collections.Generic;\n\n");
            sb.Append($"namespace {ns}\n");
            sb.Append("{\n");

            bool isMsgStart = false; // 已到达消息行
            bool hasMember = false;  // 消息中是否由成员
            bool parsedMsgIdFlag = false;
            bool isCommentStart = false;                          // 已到达注释行
            bool isLastComment = false;                           // 是否为结尾注释
            StringBuilder normalCommentSb = new StringBuilder();  // 普通注释
            StringBuilder summaryCommentSb = new StringBuilder(); // summary注释

            foreach (string line in protoContent.Split('\n'))
            {
                string newline = line.Trim();

                // 结束注释
                if (isCommentStart && !newline.StartsWith("//"))
                {
                    isCommentStart = false;
                    summaryCommentSb.Append("\t/// </summary>\n");

                    // 结束在message行视为summary注释, 否则视为普通注释
                    sb.Append(newline.StartsWith("message") ? summaryCommentSb.ToString() : normalCommentSb.ToString());
                    normalCommentSb.Clear();
                    summaryCommentSb.Clear();
                }

                if (newline.Trim() == "")
                {
                    continue;
                }

                if (newline.StartsWith("//"))
                {
                    if (newline.Contains("@id="))
                    {
                        parsedMsgIdFlag = true;

                        // 结束注释, 结束在id行也视为summary注释
                        if (isCommentStart)
                        {
                            isCommentStart = false;
                            summaryCommentSb.Append("\t/// </summary>\n");
                            sb.Append(summaryCommentSb.ToString());
                            normalCommentSb.Clear();
                            summaryCommentSb.Clear();
                        }
                    }
                    else
                    {
                        if (!isCommentStart)
                        {
                            isCommentStart = true;
                            isLastComment = true;
                            summaryCommentSb.Append("\t/// <summary>\n");
                        }

                        normalCommentSb.Append($"\t// {newline[2..].TrimStart()}\n");
                        summaryCommentSb.Append($"\t/// {newline[2..].TrimStart()}\n");
                    }

                    continue;
                }

                if (newline.StartsWith("message"))
                {
                    string parentClass = "";
                    isMsgStart = true;
                    hasMember = false;
                    isLastComment = false;
                    string msgName = newline.Split(s_splitChars, StringSplitOptions.RemoveEmptyEntries)[1];
                    msgName = ToPascalCase(msgName);

                    sb.Append("\t[ProtoContract]\n");

                    if (parsedMsgIdFlag)
                    {
                        parsedMsgIdFlag = false;
                        sb.Append($"\t[Message({opcodeClassName}.{msgName})]\n");
                        parentClass = "IMessage";

                        // 注册返回类型
                        if (msgName.EndsWith("Req"))
                        {
                            foreach (OpcodeInfo info in s_msgOpcode)
                            {
                                if (info.name == msgName[..^3] + "Resp")
                                {
                                    sb.Append($"\t[MessageResponseType({opcodeClassName}.{info.name})]\n");
                                    break;
                                }
                            }
                        }
                    }

                    sb.Append($"\tpublic partial class {msgName} : Object");
                    if (parentClass == "IMessage")
                    {
                        sb.Append($", {parentClass}\n");
                    }
                    else if (parentClass != "")
                    {
                        sb.Append($", {parentClass}\n");
                    }
                    else
                    {
                        sb.Append("\n");
                    }

                    continue;
                }

                if (isMsgStart)
                {
                    if (newline == "{")
                    {
                        sb.Append("\t{\n");
                        continue;
                    }

                    if (newline == "}")
                    {
                        isMsgStart = false;
                        if (hasMember)
                            sb.Remove(sb.Length - 1, 1); // 去除最后的回车
                        sb.Append("\t}\n\n");
                        hasMember = false;
                        continue;
                    }

                    if (newline.Trim().StartsWith("//"))
                    {
                        sb.Append("\t\t/// <summary>\n");
                        sb.Append($"\t\t/// {newline.TrimStart('/', ' ')}\n");
                        sb.Append("\t\t/// </summary>\n");
                        continue;
                    }

                    if (newline.Trim() != "" && newline != "}")
                    {
                        hasMember = true;

                        string memberStr;
                        if (newline.Contains("//"))
                        {
                            string[] lineSplit = newline.Split("//");
                            memberStr = lineSplit[0].Trim();
                            sb.Append("\t\t/// <summary>\n");
                            sb.Append($"\t\t/// {lineSplit[1].Trim()}\n");
                            sb.Append("\t\t/// </summary>\n");
                        }
                        else
                        {
                            memberStr = newline;
                        }

                        if (memberStr.StartsWith("repeated"))
                        {
                            Repeated(sb, memberStr);
                        }
                        else if (memberStr.StartsWith("required"))
                        {
                            Required(sb, memberStr);
                        }
                        else if (memberStr.StartsWith("optional"))
                        {
                            Optional(sb, memberStr);
                        }
                        else
                        {
                            Members(sb, memberStr);
                        }
                    }
                }
            }

            sb.Remove(sb.Length - 1, 1); // 去除最后的回车
            // 有结尾注释时需要加上, 若是空字符则换行
            if (isLastComment)
            {
                sb.Append(normalCommentSb.Length > 0 ? $"{normalCommentSb.ToString().TrimEnd()}\n" : "\n");
            }
            sb.Append("}");
            using FileStream txt = new FileStream(csPath, FileMode.Create, FileAccess.ReadWrite);
            using StreamWriter sw = new StreamWriter(txt);
            sw.Write(sb.ToString().Replace("\n", "\r\n").Replace("\t", "    "));
        }

        static void GenerateOpcode(string ns, string outputFileName, string outputPath)
        {
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"namespace {ns}\n");
            sb.Append("{\n");
            sb.Append($"\tpublic static class {outputFileName}\n");
            sb.Append("\t{\n");
            foreach (OpcodeInfo info in s_msgOpcode)
            {
                sb.Append($"\t\tpublic const ushort {info.name} = {info.opcode};\n");
            }

            sb.Append("\t}\n");
            sb.Append("}");

            string csPath = Path.Combine(outputPath, $"{outputFileName}.cs");
            using FileStream txt = new FileStream(csPath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(txt);
            sw.Write(sb.ToString().Replace("\n", "\r\n").Replace("\t", "    "));
        }

        static void Repeated(StringBuilder sb, string newline)
        {
            try
            {
                int index = newline.IndexOf(';');
                newline = newline.Remove(index);
                string[] ss = newline.Split(s_splitChars, StringSplitOptions.RemoveEmptyEntries);
                string type = ss[1];
                type = ConvertType(type);
                string name = ToPascalCase(ss[2]);
                int n = int.Parse(ss[3]);

                sb.Append($"\t\t[ProtoMember({n})]\n");
                sb.Append($"\t\tpublic List<{type}> {name} {{ get; set; }} = new();\n\n");
            }
            catch (Exception e)
            {
                Debug.LogError($"{newline}\n {e}");
            }
        }

        static void Optional(StringBuilder sb, string newline)
        {
            try
            {
                int index = newline.IndexOf(';');
                newline = newline.Remove(index);
                string[] ss = newline.Split(s_splitChars, StringSplitOptions.RemoveEmptyEntries);
                string type = ss[1];
                type = ConvertType(type);
                string name = ToPascalCase(ss[2]);
                int n = int.Parse(ss[3]);

                sb.Append($"\t\t[ProtoMember({n})]\n");
                sb.Append($"\t\tpublic {type} {name} {{ get; set; }}\n\n");
            }
            catch (Exception e)
            {
                Debug.LogError($"{newline}\n {e}");
            }
        }

        static void Required(StringBuilder sb, string newline)
        {
            try
            {
                int index = newline.IndexOf(';');
                newline = newline.Remove(index);
                string[] ss = newline.Split(s_splitChars, StringSplitOptions.RemoveEmptyEntries);
                string type = ss[1];
                type = ConvertType(type);
                string name = ToPascalCase(ss[2]);
                int n = int.Parse(ss[3]);

                sb.Append($"\t\t[ProtoMember({n})]\n");
                sb.Append($"\t\tpublic {type} {name} {{ get; set; }}\n\n");
            }
            catch (Exception e)
            {
                Debug.LogError($"{newline}\n {e}");
            }
        }

        /// <summary>
        /// 成员类型转换
        /// </summary>
        static string ConvertType(string type)
        {
            return type switch
            {
                "int16" => "short",
                "int32" => "int",
                "bytes" => "byte[]",
                "uint32" => "int",
                "long" => "long",
                "int64" => "long",
                "uint64" => "ulong",
                "uint16" => "ushort",
                "bool" => "bool",
                "string" => "string",
                "float" => "float",
                _ => ToPascalCase(type)
            };
        }

        static void Members(StringBuilder sb, string newline)
        {
            try
            {
                int index = newline.IndexOf(";", StringComparison.Ordinal);
                newline = newline.Remove(index);
                string[] ss = newline.Split(s_splitChars, StringSplitOptions.RemoveEmptyEntries);
                string type = ss[0];
                string name = ToPascalCase(ss[1]);
                int n = int.Parse(ss[2]);
                string typeCs = ConvertType(type);

                sb.Append($"\t\t[ProtoMember({n})]\n");
                sb.Append($"\t\tpublic {typeCs} {name} {{ get; set; }}\n\n");
            }
            catch (Exception e)
            {
                Debug.LogError($"{newline}\n {e}");
            }
        }

        /// <summary>
        /// 首字母大写
        /// </summary>
        static string UpperCaseFirstChar(string s)
        {
            return char.ToUpper(s[0]) + s[1..];
        }

        /// <summary>
        /// 由小写下划线转换成大驼峰
        /// </summary>
        static string ToPascalCase(string name)
        {
            return string.Join("", name.Split('_').Where(s => !string.IsNullOrWhiteSpace(s)).Select(UpperCaseFirstChar));
        }
    }
}