using Common;
using HarmonyLib;
using KMod;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeBuildings
{
    public class FumiKMod : KMod.UserMod2
    {
        public static Harmony instance;

        public override void OnLoad(Harmony harmony)
        {
            instance = harmony;
            CustomizeBuildingsState.LoadStrings();

            Miscellaneous.OnLoad();
            Speed_Patch.OnLoad();

            new POptions().RegisterOptions(this, typeof(CustomizeBuildingsState));

            //var state = POptions.ReadSettings<CustomizeBuildingsState>();
            //if (CustomizeBuildingsState.CheckUpdate(state))
            //    POptions.WriteSettings(state);

            base.OnLoad(harmony);
        }

        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<Mod> mods)
        {
            // if (mods.Any(a => a.staticID == "PeterHan.AIImprovements" && a.IsEnabledForActiveDlc()))
            // {
            //     harmony.Unpatch(typeof(FallMonitor.Instance).GetMethod(nameof(FallMonitor.Instance.UpdateFalling)), HarmonyPatchType.Prefix, harmony.Id);
            //     Helpers.Print("Unpatched DoorEntomb_Patch because PeterHan.AIImprovements is enabled.");
            // }
        }
    }
}
