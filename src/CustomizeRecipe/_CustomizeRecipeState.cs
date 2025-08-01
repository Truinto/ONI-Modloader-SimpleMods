using System.Collections.Generic;
using Newtonsoft.Json;
using Common;
using PeterHan.PLib.Options;
using System.IO;
using System;
using System.Linq;

namespace CustomizeRecipe
{
    [ConfigFile("CustomizeRecipe.json", true, true, typeof(Config.TranslationResolver))]
    [RestartRequired]
    public class CustomizeRecipeState : IManualConfig
    {
        public int version { get; set; } = 2;

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
            new RecipeData("RockCrusher_NewExampleRecipe1", RockCrusherConfig.ID, Time: 5f, Description: "This is the description.")
                .In(SimHashes.Water, 100f).In(SimHashes.Dirt, 50f)
                .Out(SimHashes.Lime, 25f).Out(SimHashes.SedimentaryRock, 10f),
            new RecipeData(null, RockCrusherConfig.ID, Time: 40f, Description: "Turns Regolith into Sand and Dirt.")
                .In(SimHashes.Regolith, 100f)
                .Out(SimHashes.Sand, 100f),
            new RecipeData(null, GlassForgeConfig.ID, Time: 40f, Description: "Extracts pure <link=\"LIQUIDPHOSPHORUS\">Phosphorus</link> from <link=\"PHOSPHORITE\">Phosphorite</link>.")
                .In(SimHashes.Phosphorite, 100f)
                .Out(SimHashes.LiquidPhosphorus, 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Melted)
        };

        #region _implementation

        public static Config.Manager<CustomizeRecipeState> StateManager = null!;

        public static bool OnUpdate(CustomizeRecipeState state)
        {
            if (state.version < 2)
            {
                var rs = state.RecipeSettings;
                for (int i = rs.Count - 1; i >= 0; i--)
                {
                    if (rs[i].Inputs.Any(a => a.material is "0"))
                        rs.RemoveAt(i);
                }
            }
            return true;
        }

        public static void OnLoaded(CustomizeRecipeState state)
        {
            // if no zero inputs, skip patches for zero inputs
            if (state.AllowZeroInput)
            {
                foreach (var recipe in state.RecipeSettings)
                {
                    foreach (var input in recipe.Inputs)
                    {
                        if (input.amount <= 0f || input.amounts != null && input.amounts.Any(a => a <= 0f))
                            goto exit_1;
                    }
                }
                state.AllowZeroInput = false;
            exit_1:;
            }
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
            //if (Helpers.ActiveLocale.NotEmpty() && Helpers.ActiveLocale != "en")
            //    path += "_" + Helpers.ActiveLocale;
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
