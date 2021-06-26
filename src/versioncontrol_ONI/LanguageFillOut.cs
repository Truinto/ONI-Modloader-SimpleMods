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
     * - you need the method 'public static void OnLoad()'
     * - add empty '[Option()]' attribute (ignore the error) to each option
     * - 'overwrite = true' will overwrite all option attributes
     * - put your options into named regions e.g. '#region Settings'
     * - you can prefix a region with $ to keep it category-less
	 * - if you have the category "$", put it at the very end
	 * - number of decimal places is copied from default value
	 * - if variable ends with 'Percent' uses format 'P'
     */
    public class LanguageFillOut
    {
        public static Regex rex_variable = new Regex(@"public\s*[\w\.]*\s*(\w*)", RegexOptions.Compiled);
        public static Regex rex_numbers = new Regex(@"\d+\.?(\d*)f;", RegexOptions.Compiled);

        public static void Run(string pathStateSource, bool overwrite = false)
        {
            if (pathStateSource == null || !File.Exists(pathStateSource))
                return;

            List<string> line = File.ReadAllLines(pathStateSource).ToList();
            FillOutOptionAttribute(line, overwrite);
            FillOutStringsAdd(line);
            File.WriteAllLines(pathStateSource, line);
            Console.WriteLine("updated state source");
        }

        public static void FillOutOptionAttribute(List<string> line, bool overwrite = false)
        {
            string region = "";
            string @namespace = "";
            for (int i = 0; i < line.Count; i++)
            {
                int index_region = line[i].IndexOf("#region ");
                if (index_region >= 0)
                {
                    region = line[i].Substring(index_region + 8).Trim();
                    if (region.StartsWith('$'))
                        region = "";
                }
                else if (line[i].StartsWith("namespace "))
                {
                    @namespace = line[i].Substring(10);
                }
                else if (line[i].Contains(overwrite ? "[Option(" : "[Option()]"))
                {
                    int j = FindNextLine(line, i, "public ");
                    if (j > 0 && !line[i + j].Contains(" static "))
                    {
                        string format = "null";

                        var float_match = rex_numbers.Match(line[i + j]);
                        if (float_match.Success && float_match.Groups.Count == 2)
                        {
                            format = $"\"F{float_match.Groups[1].Length}\"";
                        }

                        var variable_match = rex_variable.Match(line[i + j]);
                        if (variable_match.Success && variable_match.Groups.Count == 2)
                        {
							if (variable_match.Groups[1].Value.EndsWith("Percent"))
								format = format.Replace('F', 'P');
							
                            line[i] = $"        [Option(\"{@namespace}.LOCSTRINGS.{variable_match.Groups[1]}_Title\", \"{@namespace}.LOCSTRINGS.{variable_match.Groups[1]}_ToolTip\", \"{region}\", {format})]";
                        }
                    }
                }
            }
        }

        public static void FillOutStringsAdd(List<string> line)
        {
            bool isStart = false;	// ignore everything before OnLoad
			bool isLastCategory = false;	// ignore missing $ categories until the end
            string trimmed;
            string region = null;
            Dictionary<string, List<string>> dic = GetAllOptions(line);

            for (int i = 0; i < line.Count; i++)
            {
                trimmed = line[i].Trim();

                if (trimmed.StartsWith("public static void OnLoad()"))
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

    public class LanguageContainer
    {
        public string variable;
        public string region;
        public string titleID;
        public string titleText;
        public string tooltipID;
        public string tooltipText;

        public LanguageContainer() { }

        public LanguageContainer(string variable)
        {
            this.variable = variable;
        }

        private int hash;
        public override bool Equals(object obj)
        {
            return this.GetHashCode() == (obj as LanguageContainer)?.GetHashCode();
        }

        public override int GetHashCode()
        {
            if (hash == 0)
                hash = Hash.SDBMLower(this.variable);
            return hash;
        }

        public override string ToString()
        {
            return variable;
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