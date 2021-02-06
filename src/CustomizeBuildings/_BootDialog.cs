using CustomizeBuildings;
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
        public static string ModName = "Customize X";

        public static List<string> ErrorList = new List<string>();

        public static string ToDialog(string log)
        {
            ErrorList.Add(log);
            return log;
        }

        public static void Prefix()
        {
            if (CustomizeBuildingsState.StateManager.State.AdvancedSettings != null)
            {
                foreach (string leftover in CustomizeBuildingsState.StateManager.State.AdvancedSettings.Keys)
                {
                    ErrorList.Add("Did not find building with PrefabID = " + leftover);
                    CustomizeBuildingsState.StateManager.State.AdvancedSettings.Remove(leftover);
                } 
            }

            if (ErrorList.Count != 0)
            {
                ErrorList.Insert(0, ModName + " encountered an issue with your configuration:");
                string output = string.Join("\n  ", ErrorList.ToArray());
                KMod.Manager.Dialog(null, ModName + " WARNING", output);
                Debug.LogWarning(output);
                ErrorList.Clear();
            }
        }
    }
}