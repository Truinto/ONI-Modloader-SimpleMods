using Common;
using HarmonyLib;
using PeterHan.PLib.Options;

namespace CustomizePlants
{
    public class FumiKMod : KMod.UserMod2
    {
        public const string ModName = "CustomizePlants";
        public static Harmony instance;

        public override void OnLoad(Harmony harmony)
        {
            // init
            instance = harmony;
            Helpers.ModName = ModName;

            // predefine strings
            CustomStrings.LoadStrings();
            Helpers.StringsAddClass(typeof(PlantData));
            Helpers.StringsAddClass(typeof(PlantMutationData));
            Helpers.StringsPrint();

            // load translation, if any
            Helpers.ActiveLocale = Helpers.StringsLoad();

            // load settings
            CustomizePlantsState.StateManager = new(CustomizePlantsState.GetStaticConfigPath(), true, CustomizePlantsState.OnUpdate, null);

            // init options menu
            new POptions().RegisterOptions(this, typeof(CustomizePlantsState));

            // call OnLoad methods
            OnLoadPatch.OnLoad();

            // patch all harmony classes
            base.OnLoad(harmony);
        }
    }
}