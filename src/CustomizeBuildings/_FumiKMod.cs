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
            Helpers.ModName = "CustomizeBuildings";
            CustomStrings.LoadStrings();
            Helpers.StringsAddClass(typeof(BuildingStruct));
            Helpers.StringsAddClass(typeof(BuildingAdv));
            Helpers.StringsAddClass(typeof(ElementConverterContainer));
#if LOCALE
            Helpers.StringsPrint();
#else
            Helpers.StringsLoad();
#endif

            new POptions().RegisterOptions(this, typeof(CustomizeBuildingsState));

            Miscellaneous.OnLoad();
            Speed_Patch.OnLoad();
            base.OnLoad(harmony);
        }
    }
}
