using Common;
using HarmonyLib;
using PeterHan.PLib.Options;

namespace CustomizeRecipe
{
    public class FumiKMod : KMod.UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            Helpers.ModName = "CustomizeRecipe";
            CustomStrings.LoadStrings();
            Helpers.StringsAddClass(typeof(RecipeData));
            Helpers.StringsAddClass(typeof(RecipeData.RecipeElement));
#if LOCALE
            Helpers.StringsPrint();
#else
            Helpers.StringsLoad();
#endif

            new POptions().RegisterOptions(this, typeof(CustomizeRecipeState));

            base.OnLoad(harmony);
        }
    }
}
