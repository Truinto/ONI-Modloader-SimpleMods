using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace versioncontrol_ONI
{
    public class MDConvert
    {
        public static Regex RegexURL = new Regex(@"\[\*\*(.*?)\*\*\]\((.*?)\)", RegexOptions.Compiled);

        public static void Convert(string path)
        {
            var lines = File.ReadAllLines(path).ToList();
            Convert(lines);
            File.WriteAllLines(path, lines);
        }

        public static void Convert(List<string> lines)
        {
            bool flagList = false;
            for (int i = 1; i < lines.Count; i++)
            {
                if (lines[i].StartsWith("* "))
                {
                    lines[i] = "[*]" + lines[i].Substring(2);

                    if (!flagList)
                    {
                        flagList = true;
                        lines.Insert(i, "[list]");
                        i++;
                    }
                }
                else if (flagList)
                {
                    flagList = false;
                    lines.Insert(i, "[/list]");
                    i++;
                }

                if (lines[i].StartsWith("-----------"))
                {
                    lines[i - 1] = "[h1]" + lines[i - 1] + "[/h1]";
                    lines.RemoveAt(i);
                    i--;
                    continue;
                }

                lines[i] = RegexURL.Replace(lines[i], @"[url=\2]\1[/url]"); //"[**Link**](https://)"
            }
        }
    }
}