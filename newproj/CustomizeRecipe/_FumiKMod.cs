using Common;
using HarmonyLib;
using PeterHan.PLib.Options;

namespace CustomizeRecipe
{
    public class FumiKMod : KMod.UserMod2
    {
        public const string ModName = "CustomizeRecipe";
        public static Harmony instance;

        public override void OnLoad(Harmony harmony)
        {
            // init
            instance = harmony;
            Helpers.ModName = ModName;

            // predefine strings
            CustomStrings.LoadStrings();
            Helpers.StringsAddClass(typeof(RecipeData));
            Helpers.StringsAddClass(typeof(RecipeData.RecipeElement));
            Helpers.StringsPrint();

            // load translation, if any
            Helpers.ActiveLocale = Helpers.StringsLoad();

            // load settings
            CustomizeRecipeState.StateManager = new(CustomizeRecipeState.GetStaticConfigPath(), true, CustomizeRecipeState.OnUpdate, null);

            // init options menu
            new POptions().RegisterOptions(this, typeof(CustomizeRecipeState));

            // call OnLoad methods

            // patch all harmony classes
            base.OnLoad(harmony);
        }
    }
}
