using Common;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Config
{
    [HarmonyPatch(typeof(KMod.Manager), nameof(KMod.Manager.Report))]
    public class PostBootDialog
    {
        public static List<string> ErrorList = new();
        public static bool DoOnLoadLast = false;

        public static string ToDialog(string log)
        {
            ErrorList.Add(log);
            return log;
        }

        public static void Prefix()
        {
            if (DoOnLoadLast)
            {
                DoOnLoadLast = false;
                foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    var mi_onLast = type.GetMethod("OnLoadLast", BindingFlags.Public | BindingFlags.Static);
                    try
                    {
                        mi_onLast?.Invoke(null, null);
                    } catch (Exception e)
                    {
                        Helpers.PrintDialog($"Exception during PostBootDialog: {e.Message}");
                        Helpers.Print(e.ToString());
                    }
                }
            }

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
