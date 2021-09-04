using Common;
using HarmonyLib;
using PeterHan.PLib.Options;

namespace CustomizePlants
{
    public class FumiKMod : KMod.UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            Helpers.ModName = "CustomizePlants";
            CustomStrings.LoadStrings();
            Helpers.StringsAddClass(typeof(PlantData));
            Helpers.StringsAddClass(typeof(PlantMutationData));
#if LOCALE
            Helpers.StringsPrint();
#else
            Helpers.StringsLoad();
#endif

            new POptions().RegisterOptions(this, typeof(CustomizePlantsState));

            OnLoadPatch.OnLoad();
            base.OnLoad(harmony);
        }
    }
}