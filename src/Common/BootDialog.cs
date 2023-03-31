using Common;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Config
{
    [HarmonyPatch(typeof(KMod.Manager), "Report")]
    public class PostBootDialog
    {
        public static List<string> ErrorList = new();

        public static string ToDialog(string log)
        {
            ErrorList.Add(log);
            return log;
        }

        public static void Prefix()
        {
            if (ErrorList.Count != 0)
            {
                ErrorList.Insert(0, Helpers.ModName + " encountered an issue with your configuration:");
                string output = string.Join("\n  ", ErrorList.ToArray());
                KMod.Manager.Dialog(null, Helpers.ModName + " WARNING", output);
                Debug.LogWarning(output);
                ErrorList.Clear();
            }
        }
    }
}