using HarmonyLib;

namespace CustomizeCritter
{
    /// Scales grow in any gas, if target is set to "Void"
    [HarmonyPatch(typeof(ScaleGrowthMonitor), "IsInCorrectAtmosphere")]
    public class Patch_Scales
    {
        public static bool Prefix(ScaleGrowthMonitor.Instance smi, ref bool __result)
        {
            if (smi.def.targetAtmosphere == SimHashes.Void) //if user sets atmosphere to void, then it will always grow
            {
                __result = true;
                return false;
            }
            return true;
        }
    }

}