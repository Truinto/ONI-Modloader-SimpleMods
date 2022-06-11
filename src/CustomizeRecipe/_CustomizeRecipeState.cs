using System.Collections.Generic;
using Newtonsoft.Json;
using Common;
using PeterHan.PLib.Options;
using System.IO;
using System;

namespace CustomizeRecipe
{
    [ConfigFile("CustomizeRecipe.json", true, true, typeof(Config.TranslationResolver))]
    [RestartRequired]
    public class CustomizeRecipeState : IManualConfig
    {
        public int version { get; set; } = 1;

        [Option("CustomizeRecipe.LOCSTRINGS.LoadAllRecipesToConfig_Title", "CustomizeRecipe.LOCSTRINGS.LoadAllRecipesToConfig_ToolTip", "", null)]
        [JsonIgnore]
        public System.Action<object> LoadAllRecipesToConfig => delegate (object nix)
        {
            RecipePatch.Print();
            StateManager.TrySaveConfigurationState();

            OptionsDialog.Last?.CloseDialog();
            OptionsDialog.Last = null;
        };

        [Option("CustomizeRecipe.LOCSTRINGS.CheatFast_Title", "CustomizeRecipe.LOCSTRINGS.CheatFast_ToolTip", "", null)]
        public bool CheatFast { get; set; } = false;

        [Option("CustomizeRecipe.LOCSTRINGS.CheatFree_Title", "CustomizeRecipe.LOCSTRINGS.CheatFree_ToolTip", "", null)]
        public bool CheatFree { get; set; } = false;

        [Option("CustomizeRecipe.LOCSTRINGS.AllowZeroInput_Title", "CustomizeRecipe.LOCSTRINGS.AllowZeroInput_ToolTip", "", null)]
        public bool AllowZeroInput { get; set; } = false;

        public List<RecipeData> RecipeSettings { get; set; } = new List<RecipeData>() {
            new RecipeData("RockCrusher_I_Fossil_O_Lime_SedimentaryRock", RockCrusherConfig.ID)
                .In(SimHashes.Fossil, 100f)
                .Out(SimHashes.Lime, 20f).Out(SimHashes.SedimentaryRock, 80f),
            new RecipeData("RockCrusher_NewExampleRecipe1", RockCrusherConfig.ID, 5f, 0, "This is the description.")
                .In(SimHashes.Water, 100f).In(SimHashes.Dirt, 50f)
                .Out(SimHashes.Lime, 25f).Out(SimHashes.SedimentaryRock, 10f),
            new RecipeData(null, RockCrusherConfig.ID, 40f, 0, "Turns Regolith into Sand and Dirt.")
                .In(SimHashes.Regolith, 100f)
                .Out(SimHashes.Sand, 100f)
        };

        #region _implementation

        public static Config.Manager<CustomizeRecipeState> StateManager;

        public static bool OnUpdate(CustomizeRecipeState state)
        {
            return true;
        }

        public object ReadSettings()
        {
            return StateManager.State;
        }

        public void WriteSettings(object settings)
        {
            if (settings is CustomizeRecipeState state)
                StateManager.TrySaveConfigurationState(state);
            else
                StateManager.TrySaveConfigurationState();
        }

        public string GetConfigPath()
        {
            return GetStaticConfigPath();
        }

        public static string GetStaticConfigPath()
        {
            string path = FumiKMod.ModName;
            if (Helpers.ActiveLocale.NotEmpty() && Helpers.ActiveLocale != "en")
                path += "_" + Helpers.ActiveLocale;
            return Config.PathHelper.CreatePath(path);
        }

        #endregion
    }

    public class CustomStrings
    {
        public static void LoadStrings()
        {
            #region 
            Helpers.StringsAddProperty("CustomizeRecipe.PROPERTY.version", "version");

            Helpers.StringsAdd("CustomizeRecipe.LOCSTRINGS.LoadAllRecipesToConfig_Title", "Load All Recipes To Config");
            Helpers.StringsAdd("CustomizeRecipe.LOCSTRINGS.LoadAllRecipesToConfig_ToolTip", "This will look up the current recipes and dump them into the manual config file. It's not needed to use this mod! While this is very verbose, it makes it easier to find existing recipes.");

            Helpers.StringsAddProperty("CustomizeRecipe.PROPERTY.CheatFast", "CheatFast");
            Helpers.StringsAdd("CustomizeRecipe.LOCSTRINGS.CheatFast_Title", "Cheat Fast");
            Helpers.StringsAdd("CustomizeRecipe.LOCSTRINGS.CheatFast_ToolTip", "If true, all recipes will be speeded up.");

            Helpers.StringsAddProperty("CustomizeRecipe.PROPERTY.CheatFree", "CheatFree");
            Helpers.StringsAdd("CustomizeRecipe.LOCSTRINGS.CheatFree_Title", "Cheat Free");
            Helpers.StringsAdd("CustomizeRecipe.LOCSTRINGS.CheatFree_ToolTip", "If true, all recipes need no materials.");

            Helpers.StringsAddProperty("CustomizeRecipe.PROPERTY.AllowZeroInput", "AllowZeroInput");
            Helpers.StringsAdd("CustomizeRecipe.LOCSTRINGS.AllowZeroInput_Title", "Allow Zero Input");
            Helpers.StringsAdd("CustomizeRecipe.LOCSTRINGS.AllowZeroInput_ToolTip", "Set this to true, if you plan to set ingredients to 0kg. This will prevent the game from crashing.");

            Helpers.StringsAddProperty("CustomizeRecipe.PROPERTY.RecipeSettings", "RecipeSettings");
            #endregion
        }
    }
}
