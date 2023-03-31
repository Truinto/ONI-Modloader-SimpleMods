using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.CodeDom.Compiler;
using static versioncontrol_ONI.HelperStrings;

namespace versioncontrol_ONI
{
    /*
     * This program fills out language strings in your source code.
     * - custom code style rules will probably cause issues
     * - you need 'namespace'
     * - you need the method 'public static void LoadStrings()', it will be overwritten
     * - properties and attributes must be in a single line
     * - put your options into named regions e.g. '#region Settings'
     * - you can prefix a region with $ to keep it category-less
	 * - number of decimal places is copied from default value
	 * - if variable ends with 'Percent' uses format 'P'
     */
    public class LanguageFillOut
    {
        public static Regex rex_variable = new(@"public\s+[\w\.<>\[\],\s]*\s+(\w+)\s+", RegexOptions.Compiled);
        public static Regex rex_numbers = new(@"\d+\.?(\d*)f;", RegexOptions.Compiled);

        public static Dictionary<string, Data> AllData = new(50);
        public static string Namespace;

        public class Data
        {
            public bool HasRegion => Region != null && Region != "";
            public string Region;
            public string PropertyName;
            public string Title;
            public string ToolTip;
            public string format;
        }

        public static void Run(string pathStateSource, bool overwrite = false)
        {
            if (pathStateSource == null || !File.Exists(pathStateSource))
                return;

            List<string> line = File.ReadAllLines(pathStateSource).ToList();
            SearchAndFillOutOptionAttribute(line, overwrite);
            FillOutLoadStrings(ref line);
            //FillOutStringsAdd(line);
            File.Move(pathStateSource, pathStateSource + ".bak", true);
            File.WriteAllLines(pathStateSource, line);
            Console.WriteLine("updated state source");
        }

        public static void SearchAndFillOutOptionAttribute(List<string> line, bool overwrite = false)
        {
            string region = "";

            for (int i = 0; i < line.Count; i++)
            {
                int index_region = line[i].IndexOf("#region ", StringComparison.Ordinal);
                if (index_region >= 0)
                {
                    region = line[i].Substring(index_region + 8).Trim();
                    if (region.StartsWith('$'))
                        region = "";
                }
                else if (line[i].StartsWith("namespace ", StringComparison.Ordinal))
                {
                    Namespace = line[i].Substring(10);
                }
                else if (line[i].Contains("Helpers.StringsAdd(", StringComparison.Ordinal)) // read already existing strings
                {
                    string q1 = line[i].GetQuotationString(1) ?? "";
                    int dot = q1.LastIndexOf('.') + 1;
                    int score = q1.LastIndexOf('_');
                    string q2 = line[i].GetQuotationString(2);

                    if (dot > 0 && score > dot && q2 != null)
                    {
                        string key = q1[dot..score];
                        if (!AllData.ContainsKey(key))
                            AllData[key] = new Data();

                        if (line[i].Contains("_Title") && AllData[key].Title == null)
                            AllData[key].Title = q2;
                        else if (line[i].Contains("_ToolTip") && AllData[key].ToolTip == null)
                            AllData[key].ToolTip = q2;
                    }
                }
                else if (line[i].Contains("public ", StringComparison.Ordinal) && !line[i].Contains(" static ", StringComparison.Ordinal) && !line[i].Contains(" class ", StringComparison.Ordinal))
                {
                    // try to match field that is public and non static
                    var variable_match = rex_variable.Match(line[i]);
                    if (variable_match.Success && variable_match.Groups.Count == 2 && variable_match.Groups[1].Value != "")
                    {
                        // if field matches, create new variable entry
                        string variable = variable_match.Groups[1].Value;
                        if (!AllData.ContainsKey(variable))
                            AllData[variable] = new Data();

                        // if field is a button, don't save property; could also check for JsonIgnore attribute
                        if (!line[i].Contains(" => ", StringComparison.Ordinal))
                            AllData[variable].PropertyName = variable;

                        // try to find format, is null when no match
                        string format = "null";
                        var float_match = rex_numbers.Match(line[i]);
                        if (float_match.Success && float_match.Groups.Count == 2)
                            format = $"\"F{float_match.Groups[1].Length}\"";
                        if (variable.EndsWith("Percent", StringComparison.Ordinal))
                            format = format.Replace('F', 'P');
                        AllData[variable].format = format;

                        // apply current region
                        AllData[variable].Region = region;

                        // get values from Option attribute, if any
                        // overwrites it with LOCSTRING key, if necessary
                        int j = FindPreviousLine(line, i, "[Option(");
                        if (j < 0 && j >= -2)
                        {
                            string q1 = line[i + j].GetQuotationString(1) ?? "";
                            string q2 = line[i + j].GetQuotationString(2) ?? "";
                            if (q1.Contains(".LOCSTRINGS.", StringComparison.Ordinal)) q1 = null;
                            if (q2.Contains(".LOCSTRINGS.", StringComparison.Ordinal)) q2 = null;
                            if (q1 != null) AllData[variable].Title = q1;
                            if (q2 != null) AllData[variable].ToolTip = q2;
                            if (q1 != null || q2 != null)
                                line[i + j] = $"        [Option(\"{Namespace}.LOCSTRINGS.{variable}_Title\", \"{Namespace}.LOCSTRINGS.{variable}_ToolTip\", \"{region}\", {format})]";
                        }
                    }
                }
            }
        }

        public static void FillOutLoadStrings(ref List<string> line)
        {
            int index1 = line.FindIndex(f => f.Contains("void LoadStrings()", StringComparison.Ordinal)) + 1;
            if (index1 <= 0)
                return;

            int index2 = line.FindIndex(index1, f => f.Trim() == "}");
            if (index2 < index1)
                return;

            List<string> result = new List<string>(line.Count);

            for (int i = 0; i <= index1; i++)
                result.Add(line[i]);

            string region = null;
            foreach (var data in AllData.OrderBy(o => o.Value.Region))
            {
                if (region != data.Value.Region)
                {
                    if (region != null)
                    {
                        result.RemoveAt(result.Count - 1);
                        result.Add("            #endregion");
                    }

                    region = data.Value.Region;
                    result.Add("            #region " + region);
                }

                if (data.Value.PropertyName != null)
                    result.Add($"            Helpers.StringsAddProperty(\"{Namespace}.PROPERTY.{data.Key}\", \"{data.Value.PropertyName}\");");
                if (data.Value.Title != null)
                {
                    result.Add($"            Helpers.StringsAdd(\"{Namespace}.LOCSTRINGS.{data.Key}_Title\", \"{data.Value.Title}\");");
                    result.Add($"            Helpers.StringsAdd(\"{Namespace}.LOCSTRINGS.{data.Key}_ToolTip\", \"{data.Value.ToolTip}\");");
                }
                result.Add("");
            }

            if (region != null)
            {
                result.RemoveAt(result.Count - 1);
                result.Add("            #endregion");
                //result.Add("");
            }

            //result.Add("#if LOCALE");
            //result.Add("            Helpers.StringsPrint();");
            //result.Add("#else");
            //result.Add("            Helpers.StringsLoad();");
            //result.Add("#endif");

            for (int i = index2; i < line.Count; i++)
                result.Add(line[i]);

            line = result;
        }


        public static void FillOutStringsAdd(List<string> line)
        {
            bool isStart = false;   // ignore everything before OnLoad
            bool isLastCategory = false;	// ignore missing $ categories until the end
            string trimmed;
            string region = null;
            Dictionary<string, List<string>> dic = GetAllOptions(line);

            for (int i = 0; i < line.Count; i++)
            {
                trimmed = line[i].Trim();

                if (trimmed.StartsWith("public static void LoadStrings()"))
                    isStart = true;

                else if (!isStart)
                    continue;

                else if (trimmed == "}")
                {
                    if (dic.Count == 0)
                        break;

                    // fill out missing REGIONS
                    isLastCategory = true;
                    int oldIndex = i;
                    foreach (var key in dic.Keys)
                    {
                        line.Insert(i++, "            #region " + key);
                        line.Insert(i++, "#error check new keys");
                        line.Insert(i++, "            #endregion");
                    }
                    i = oldIndex - 1;
                    continue;
                }

                else if (region != null && trimmed.StartsWith("Helpers.StringsAdd("))
                {
                    if (!dic[region].Remove(trimmed.GetQuotationString(1)))
                        line.Insert(i++, "#warning unused language key");
                }

                else if (trimmed.StartsWith("#region "))
                {
                    region = trimmed.Substring(8);
                    if (region == "$")
                        isLastCategory = true;
                    else if (region.StartsWith('$'))
                        region = "$";

                    if (!dic.ContainsKey(region))
                        region = null;
                }

                else if (region != null && trimmed.StartsWith("#endregion"))
                {
                    if (region != "$" || isLastCategory)
                    {
                        // fill out missing LOCSTRINGS
                        foreach (var entry in dic[region])
                        {
                            if (entry.Contains("_Title"))
                                line.Insert(i++, "#error define title string");
                            line.Insert(i++, $"            Helpers.StringsAdd(\"{entry}\", \"\");");
                            if (entry.Contains("_ToolTip"))
                                line.Insert(i++, "");
                        }
                        dic.Remove(region);
                    }
                    else if (dic[region].Count == 0)
                    {
                        dic.Remove(region);
                    }
                }
            }
        }
        public static Dictionary<string, List<string>> GetAllOptions(List<string> line)
        {
            var result = new Dictionary<string, List<string>>();

            for (int i = 0; i < line.Count; i++)
            {
                if (line[i].Contains("[Option("))
                {
                    string title = line[i].GetQuotationString(1);
                    string tooltip = line[i].GetQuotationString(2);
                    string region = line[i].GetQuotationString(3) ?? "$";
                    if (title != null && tooltip != null)
                    {
                        if (!result.ContainsKey(region))
                            result[region] = new List<string>();
                        result[region].Add(title);
                        result[region].Add(tooltip);
                    }
                }
            }

            return result;
        }
    }

    public class Parser
    {
        public void Main()
        {
            //System.CodeDom.Compiler.CodeParser.Parse();
        }
    }

}