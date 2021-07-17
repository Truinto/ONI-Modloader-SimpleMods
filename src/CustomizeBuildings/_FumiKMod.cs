using HarmonyLib;
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
        public override void OnLoad(Harmony harmony)
        {
            CustomizeBuildingsState.LoadStrings();

            Miscellaneous.OnLoad();
            Speed_Patch.OnLoad();

            new POptions().RegisterOptions(this, typeof(CustomizeBuildingsState));

            //var state = POptions.ReadSettings<CustomizeBuildingsState>();
            //if (CustomizeBuildingsState.CheckUpdate(state))
            //    POptions.WriteSettings(state);

            base.OnLoad(harmony);
        }
    }
}
