using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoRemains
{
    public class NoRemains : KMod.UserMod2
    {
        public const string ModName = "NoRemains";
        public static Harmony instance;

        public override void OnLoad(Harmony harmony)
        {
            // init
            instance = harmony;
            //Helpers.ModName = ModName;

            // predefine strings
            //CustomStrings.LoadStrings();
            //CustomStrings.SkillStationStrings();
            //Helpers.StringsAddClass(typeof(BuildingStruct));
            //Helpers.StringsAddClass(typeof(BuildingAdv));
            //Helpers.StringsAddClass(typeof(ElementConverterContainer));
            //Helpers.StringsPrint();

            // load translation, if any
            //Helpers.ActiveLocale = Helpers.StringsLoad();

            // load settings
            //Helpers.CallSafe(CustomizeBuildingsState.BeforeUpdate);
            //CustomizeBuildingsState.StateManager = new(CustomizeBuildingsState.GetStaticConfigPath(), true, CustomizeBuildingsState.OnUpdate, null);

            // init options menu
            //new POptions().RegisterOptions(this, typeof(CustomizeBuildingsState));

            // call OnLoad methods
            //Helpers.CallSafe(Miscellaneous.OnLoad);
            //Helpers.CallSafe(Speed_Patch.OnLoad);

            // patch all harmony classes
            base.OnLoad(harmony);
        }
    }
}
