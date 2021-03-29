using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace versioncontrol_ONI
{
    public static class HelperStrings
    {
        public static int FindNextLine(List<string> line, int index, string search)
        {
            if (index + 1 >= line.Count)
                return -1;

            for (int j = 1; index + j < line.Count; j++)
            {
                if (line[index + j].Contains(search))
                    return j;
            }

            return -1;
        }

        /// <summary>Returns string inside quotation marks. Respects escape character.</summary>
        public static string GetQuotationString(this string line, int occurrence)
        {
            occurrence++;
            
            bool isEscape = false;
            int start = -1;
            int stop = -1;
            int count = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (isEscape)
                {
                    isEscape = false;
                }
                else if (line[i] == '\\')
                {
                    isEscape = true;
                    continue;
                }
                else if (line[i] == '"')
                {
                    if (occurrence == count / 2)
                    {
                        if (0 == count % 2)
                        {
                            start = i+1;
                        }
                        else
                        {
                            stop = i;
                            break;
                        }
                    }
                    // o = 0; c = 0 -> start
                    // o = 0; c = 1 -> stop
                    // o = 1; c = 2 -> start
                    // o = 1; c = 3 -> stop
                    // o = 2; c = 4 -> start
                    // o = 2; c = 5 -> stop
                    count++;
                }
            }

            if (start >= 0 && stop >= start)
                return line.Substring(start, stop - start); //"str"; start=1, stop=4
            return null;
        }
    }
}