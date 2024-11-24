using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Game.Proto
{
    internal class OpcodeInfo
    {
        public string name;
        public int opcode;
    }

    public class Proto2CsHandler : EditorWindow
    {
        static string s_messagePath;
        static string s_protoPath;
        static readonly string ProtoPathKey = $"{nameof(Proto2CsHandler)}-ProtoPath-Key";
        readonly GUIContent _tips = new("操作完成，请等待编译...");

        [MenuItem("Tools/Proto2Cs")]
        public static void ShowWindow()
        {
            GetWindow(typeof(Proto2CsHandler));
        }

        public void OnEnable()
        {
            s_messagePath = $"{Application.dataPath}/Scripts/Agen/Proto";
            if (!Directory.Exists(s_messagePath))
            {
                Directory.CreateDirectory(s_messagePath);
            }

            s_protoPath = EditorPrefs.GetString(ProtoPathKey);
            minSize = new Vector2(360, 220);
        }

        private void OnGUI()
        {
            GUILayout.Space(15);

            GUI.enabled = false;
            var folder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(FileUtil.GetProjectRelativePath(s_messagePath));
            EditorGUILayout.ObjectField("消息存储路径：", folder, typeof(DefaultAsset), false);
            GUI.enabled = true;

            s_protoPath = EditorGUILayout.TextField("Proto文件存储路径：", s_protoPath);

            if (GUI.changed)
            {
                EditorPrefs.SetString(ProtoPathKey, s_protoPath);
            }

            var rt = GUILayoutUtility.GetLastRect();
            rt.width = 200;
            rt.height = 48;
            rt.x = (position.width - rt.width) / 2;
            rt.y = position.height - rt.height - 10;
            if (GUI.Button(rt, "生成 .cs 实体类"))
            {
                InnerProto2Cs.Proto2Cs(s_protoPath, s_messagePath);
                ShowNotification(_tips);
                AssetDatabase.Refresh();
            }
        }
    }

    public static class InnerProto2Cs
    {
        private static readonly char[] SplitChars = { ' ', '\t', '=' };
        private static readonly List<OpcodeInfo> MsgOpcode = new();

        public static void Proto2Cs(string path, string messagePath)
        {
            MsgOpcode.Clear();
            var files = Directory.GetFiles(path, "*.proto", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                LoadOpcode(file);
            }

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                Proto2Cs("Game.Proto", fileName, file, messagePath, "ProtoOpcode");
            }

            GenerateOpcode("Game.Proto", "ProtoOpcode", messagePath);
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
                    string msgName = newline.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries)[1];
                    msgName = ToPascalCase(msgName);
                    if (parsedMsgIdFlag)
                    {
                        MsgOpcode.Add(new OpcodeInfo() { name = msgName, opcode = parsedMsgId });
                        parsedMsgIdFlag = false;
                    }
                }
            }
        }

        static void Proto2Cs(string ns, string name, string protoPath, string outputPath, string opcodeClassName)
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
                    string msgName = newline.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries)[1];
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
                            foreach (OpcodeInfo info in MsgOpcode)
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
            foreach (OpcodeInfo info in MsgOpcode)
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
                string[] ss = newline.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
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
                string[] ss = newline.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
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
                string[] ss = newline.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
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
                "uint32" => "uint",
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
                string[] ss = newline.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
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