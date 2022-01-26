using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace CarePackageMod
{
    // not tested... sorry
    public class CarePackageAPI
    {
        /// Loads the config State
        public static bool Reload()
        {
            if (CarePackageState.StateManager == null || CarePackageState.StateManager.State == null)
                return false;

            return OverridePackages(CarePackageState.StateManager.State.CarePackages);
        }

        /// creates CarePackageInfo array and overrides vanilla values
        /// should work at any point and persist loads
        /// returns true when load successful
        public static bool OverridePackages(IEnumerable<CarePackageContainer> containerList)
        {
            if (containerList == null) return false;

            if (CarePackageMod.carePackages == null)
                CarePackageMod.carePackages = new List<CarePackageInfo>();
            else
                CarePackageMod.carePackages.Clear();

            foreach (var pack in containerList)
                CarePackageMod.carePackages.Add(pack.ToInfo());

            OverridePackages(CarePackageMod.carePackages, false);   // doesn't need to save, since we loaded everything into the save already
            return true;
        }

        /// directly overrides values
        /// if save == true will apply values whenever needed, otherwise might get overriden on load of a save-file
        public static void OverridePackages(List<CarePackageInfo> packageList, bool save = true)
        {
            if (save)
                CarePackageMod.carePackages = packageList;    // saves the list so the config doesn't override it back

            if (Immigration.Instance == null)   // instance must be loaded; if Instance == null and save == true then it will load it later
                return;

            if (CarePackageState.StateManager.State.loadPackages)   // only apply if allowed by settings
                AccessTools.Field(typeof(Immigration), "carePackages").SetValue(Immigration.Instance, packageList.ToArray());
            else
                CarePackageMod.dirty = true;
        }

        /// returns the currently set CarePackageInfo array, is null if not initialized AND not set
        public static CarePackageInfo[] GetPackages()
        {
            if (Immigration.Instance == null)
                return CarePackageMod.carePackages.ToArray();
            else 
                return (CarePackageInfo[])AccessTools.Field(typeof(Immigration), "carePackages").GetValue(Immigration.Instance);
        }
    }

}