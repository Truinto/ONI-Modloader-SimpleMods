using Harmony;
using ONI_Common;
using System.Collections.Generic;

namespace CarePackageMod
{
    [HarmonyPatch(typeof(Immigration), "ConfigureCarePackages")]
    internal class Immigration_ConfigureCarePackages
    {
        private static bool Prefix(ref CarePackageInfo[] ___carePackages)
        {
            if (! CarePackageState.StateManager.State.enabled) return true;

            List<CarePackageInfo> carePackages = new List<CarePackageInfo>();

            Debug.Log("Setting up care packages:");
            foreach (KeyValuePair<string, float> info in CarePackageState.StateManager.State.CarePackages)
            {
                Debug.Log(" id: " + info.Key + " quantity: " + info.Value);
                carePackages.Add(new CarePackageInfo(info.Key, info.Value, null));
            }

            ___carePackages = carePackages.ToArray();

            return false;
        }
    }
}