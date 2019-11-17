using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace BootDialog
{
    [HarmonyPatch(typeof(KMod.Manager), "Report")]
    public class PostBootDialog
    {
        public static List<string> ErrorList = new List<string>();

        public static string ToDialog(string log)
        {
            ErrorList.Add(log);
            return log;
        }

        internal static void Prefix()
        {
            if (ErrorList.Count != 0)
            {
                ErrorList.Insert(0, "Customize Geysers encountered an issue with your configuration:");
                string output = string.Join("\n  ", ErrorList.ToArray());
                KMod.Manager.Dialog(null, "Customize Geysers ERROR", output);
                Debug.LogWarning(output);
                ErrorList.Clear();
            }
        }
    }
}