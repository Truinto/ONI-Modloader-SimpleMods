using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.IO;
using PeterHan.PLib.Options;
using Newtonsoft.Json;
using Common;

namespace CustomizePlants
{
    [ConfigFile("CustomizePlants.json", true, true, typeof(Config.TranslationResolver))]
    [RestartRequired]
    public class CustomizePlantsState
    {
        public int version { get; set; } = 28;

        #region $Buttons
        [Option("CustomizePlants.LOCSTRINGS.ResetToCustomDefault_Title", "CustomizePlants.LOCSTRINGS.ResetToCustomDefault_ToolTip", "", null)]
        [JsonIgnore]
        public System.Action<object> ResetToCustomDefault => delegate (object nix)
        {
            StateManager.TrySaveConfigurationState(new CustomizePlantsState());

            OptionsDialog.Last?.CloseDialog();
            OptionsDialog.Last?.CheckForRestart();
            OptionsDialog.Last = null;
        };

        [Option("CustomizePlants.LOCSTRINGS.PresetClear_Title", "CustomizePlants.LOCSTRINGS.PresetClear_ToolTip", "", null)]
        [JsonIgnore]
        public System.Action<object> PresetClear => delegate (object nix)
        {
            StateManager.State.PlantSettings.Clear();

            StateManager.TrySaveConfigurationState();

            OptionsDialog.Last?.CloseDialog();
            OptionsDialog.Last?.CheckForRestart();
            OptionsDialog.Last = null;
        };

        [Option("CustomizePlants.LOCSTRINGS.PresetBasic_Title", "CustomizePlants.LOCSTRINGS.PresetBasic_ToolTip", "", null)]
        [JsonIgnore]
        public System.Action<object> PresetBasic => delegate (object nix)
        {
            foreach (string plant in PLANTS.NAMES)
            {
                if (!StateManager.State.PlantSettings.Any(a => a.id == plant))
                    StateManager.State.PlantSettings.Add(new PlantData(plant));
            }

            StateManager.TrySaveConfigurationState();

            OptionsDialog.Last?.CloseDialog();
            OptionsDialog.Last = null;
        };

        [Option("CustomizePlants.LOCSTRINGS.PresetEverything_Title", "CustomizePlants.LOCSTRINGS.PresetEverything_ToolTip", "", null)]
        [JsonIgnore]
        public System.Action<object> PresetEverything => delegate (object nix)
        {
            foreach (string plant in PLANTS.NAMES)
            {
                if (!StateManager.State.PlantSettings.Any(a => a.id == plant))
                    StateManager.State.PlantSettings.Add(new PlantData(plant));
            }

            foreach (var prefab in Assets.Prefabs)
            {
                var setting = StateManager.State.PlantSettings.FirstOrDefault(f => f.id == prefab.name);
                if (setting != null)
                    PlantHelper.ReadPlant(prefab.gameObject, ref setting);
            }

            StateManager.TrySaveConfigurationState();

            OptionsDialog.Last?.CloseDialog();
            OptionsDialog.Last = null;
        };

        [Option("CustomizePlants.LOCSTRINGS.PresetHalf_Title", "CustomizePlants.LOCSTRINGS.PresetHalf_ToolTip", "", null)]
        [JsonIgnore]
        public System.Action<object> PresetHalf => delegate (object nix)
        {
            foreach (var setting in StateManager.State.PlantSettings)
            {
                foreach (var key in setting.irrigation.Keys.ToList())
                    setting.irrigation[key] *= 0.5f;
            }

            StateManager.TrySaveConfigurationState();

            OptionsDialog.Last?.CloseDialog();
            OptionsDialog.Last = null;
        };
        #endregion

        #region Advanced
        public HashSet<PlantData> PlantSettings { get; set; } = new HashSet<PlantData>() {
            new PlantData(id: BasicSingleHarvestPlantConfig.ID, irrigation: new Dictionary<string, float>() { {"Dirt", 5f} }, temperatures: new float[] { 218.15f, 278.15f, 308.15f, 398.15f }),
            new PlantData(id: MushroomPlantConfig.ID, irrigation: new Dictionary<string, float>() { }, fruitId: MushroomConfig.ID, fruit_amount: 1, fruit_grow_time: 6*600, input_element: "CarbonDioxide", input_rate: 0.001f),
            new PlantData(id: PrickleFlowerConfig.ID, irrigation: new Dictionary<string, float>() { {"Water", 5f} }),
            new PlantData(id: SeaLettuceConfig.ID, irrigation: new Dictionary<string, float>() { {"SaltWater", 5f} }),
            new PlantData(id: BeanPlantConfig.ID, irrigation: new Dictionary<string, float>() { {"Ethanol", 5f} }),
            new PlantData(id: SpiceVineConfig.ID, irrigation: new Dictionary<string, float>() { {"DirtyWater", 15f}, {"Phosphorite", 1f} }),
            new PlantData(id: ForestTreeConfig.ID, irrigation: new Dictionary<string, float>() { {"DirtyWater", 62.5f }, {"Dirt", 10f} }),
            //new PlantData(id: OxyfernConfig.ID, fruitId: OxyfernConfig.SEED_ID, fruit_amount: 1, fruit_grow_time: 20*600, max_age: -1f),
            //new PlantData(id: ColdBreatherConfig.ID, irrigation: new Dictionary<string, float>() { }, fruitId: ColdBreatherConfig.SEED_ID, fruit_amount: 1, fruit_grow_time: 20*600, max_age: -1f),
            new PlantData(id: ColdBreatherConfig.ID, irrigation: new Dictionary<string, float>() { }, radiation: 0f, radiation_radius: 6),
            new PlantData(id: GasGrassConfig.ID, illumination: 100f),
            new PlantData(id: PrickleGrassConfig.ID, safe_elements: new string[] { "Oxygen", "ContaminatedOxygen", "CarbonDioxide", "ChlorineGas" }, input_element: "ChlorineGas", input_rate: 0.01f),
            new PlantData(id: LeafyPlantConfig.ID, irrigation: new Dictionary<string, float>() { {"Water", 10f} }, fruitId: "placeholder1", fruit_grow_time: 4*600f),
            new PlantData(id: CactusPlantConfig.ID, pressures: new float[] { 0f, 0f, 2f, 30f }, safe_elements: new string[] { "Oxygen", "ContaminatedOxygen", "CarbonDioxide", "Methane" }, input_element: "Methane", input_rate: 0.01f, output_element: "CarbonDioxide", output_rate: 0.01f),
            new PlantData(id: EvilFlowerConfig.ID, pressures: new float[] { 0f, 0f, 5f }, safe_elements: new string[] { "CarbonDioxide", "ChlorineGas" }, output_element: "ChlorineGas", output_rate: 0.012f)
        };

        public Dictionary<string, Dictionary<string, int>> SpecialCropSettings { get; set; } = new Dictionary<string, Dictionary<string, int>>() {
            { "placeholder1", new Dictionary<string, int>() { { "Algae", 40 }, { "Clay", 20 } } }
        };

        public bool print_mutations { get; set; } = true;

        public HashSet<PlantMutationData> MutationSettings { get; set; } = new HashSet<PlantMutationData>()
        {
        };
        #endregion

        #region Extras

        [Option("CustomizePlants.LOCSTRINGS.MutantPlantsDropSeeds_Title", "CustomizePlants.LOCSTRINGS.MutantPlantsDropSeeds_ToolTip", "Extras", null)]
        public bool MutantPlantsDropSeeds { get; set; } = true;
        [Option("CustomizePlants.LOCSTRINGS.SeedsGoIntoAnyFlowerPots_Title", "CustomizePlants.LOCSTRINGS.SeedsGoIntoAnyFlowerPots_ToolTip", "Extras", null)]
        public bool SeedsGoIntoAnyFlowerPots { get; set; } = true;
        [Option("CustomizePlants.LOCSTRINGS.WheezewortTempDelta_Title", "CustomizePlants.LOCSTRINGS.WheezewortTempDelta_ToolTip", "Extras", "F0")]
        public float WheezewortTempDelta { get; set; } = -20f;
        [Option("CustomizePlants.LOCSTRINGS.OxyfernOxygenPerSecond_Title", "CustomizePlants.LOCSTRINGS.OxyfernOxygenPerSecond_ToolTip", "Extras", "F5")]
        public float OxyfernOxygenPerSecond { get; set; } = 0.03125f;
        [Option("CustomizePlants.LOCSTRINGS.CheatMutationAnalyze_Title", "CustomizePlants.LOCSTRINGS.CheatMutationAnalyze_ToolTip", "Extras", null)]
        public bool CheatMutationAnalyze { get; set; } = false;
        [Option("CustomizePlants.LOCSTRINGS.CheatFlowerVase_Title", "CustomizePlants.LOCSTRINGS.CheatFlowerVase_ToolTip", "Extras", null)]
        public bool CheatFlowerVase { get; set; } = false;
        [Option("CustomizePlants.LOCSTRINGS.WildFlowerVase_Title", "CustomizePlants.LOCSTRINGS.WildFlowerVase_ToolTip", "Extras", null)]
        public bool WildFlowerVase { get; set; } = false;
        [Option("CustomizePlants.LOCSTRINGS.ApplyBugFixes_Title", "CustomizePlants.LOCSTRINGS.ApplyBugFixes_ToolTip", "Extras", null)]
        public bool ApplyBugFixes { get; set; } = true;
        #endregion

        #region Mod Plants
        public bool AutomaticallyAddModPlants { get; set; } = false;
        public HashSet<string> IgnoreList { get; set; } = new HashSet<string>();
        public HashSet<string> ModPlants { get; set; } = new HashSet<string>();
        #endregion

        public static Config.Manager<CustomizePlantsState> StateManager = new Config.Manager<CustomizePlantsState>(Config.PathHelper.CreatePath("CustomizePlants"), true, UpdateFunction);

        public static bool UpdateFunction(CustomizePlantsState state)
        {
            if (state.version < 17)
            {
                state.AutomaticallyAddModPlants = false;
                state.ModPlants.Clear();
            }
            if (state.version < 23)
            {
                var wheeze = state.PlantSettings.FirstOrDefault(f => f.id == ColdBreatherConfig.ID);
                if (wheeze != null)
                    wheeze.radiation = 0f;
            }
            if (state.version < 24)
            {
                state.MutationSettings.Clear();
            }
            if (state.version < 25)
            {
                string modpath = Path.Combine(Config.PathHelper.ModsDirectory, "config", "Customize Plants.json");
                if (File.Exists(modpath))
                    File.Delete(modpath);
            }
            return true;
        }

    }

    public class CustomStrings
    {
        public static void LoadStrings()
        {
            #region 
            Helpers.StringsAddProperty("CustomizePlants.PROPERTY.version", "version");

            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.ResetToCustomDefault_Title", "Reset: Custom Default");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.ResetToCustomDefault_ToolTip", "Re-installs mod's default settings.");

            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.PresetClear_Title", "Preset: Clear");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.PresetClear_ToolTip", "Clears the list. Restart to apply!");

            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.PresetBasic_Title", "Preset: Basic");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.PresetBasic_ToolTip", "Fills the list with names of all base game plants.");

            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.PresetEverything_Title", "Preset: Read All");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.PresetEverything_ToolTip", "Fills the list with most properties currently applied. If you want the base game properties, run \"Preset: Clear\" and restart before using this.");

            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.PresetHalf_Title", "Preset: Half");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.PresetHalf_ToolTip", "Goes through the list and halfs all irrigations. Restart when you are done.");
            #endregion
            #region Advanced
            Helpers.StringsAddProperty("CustomizePlants.PROPERTY.PlantSettings", "PlantSettings");

            Helpers.StringsAddProperty("CustomizePlants.PROPERTY.SpecialCropSettings", "SpecialCropSettings");

            Helpers.StringsAddProperty("CustomizePlants.PROPERTY.print_mutations", "print_mutations");

            Helpers.StringsAddProperty("CustomizePlants.PROPERTY.MutationSettings", "MutationSettings");
            #endregion
            #region Extras
            Helpers.StringsAddProperty("CustomizePlants.PROPERTY.MutantPlantsDropSeeds", "MutantPlantsDropSeeds");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.MutantPlantsDropSeeds_Title", "Mutant Plants Drop Seeds");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.MutantPlantsDropSeeds_ToolTip", "Mutant plants can also drop seeds.");

            Helpers.StringsAddProperty("CustomizePlants.PROPERTY.SeedsGoIntoAnyFlowerPots", "SeedsGoIntoAnyFlowerPots");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.SeedsGoIntoAnyFlowerPots_Title", "Seeds Go Into Any Flower Pots");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.SeedsGoIntoAnyFlowerPots_ToolTip", "Whenever or not all seeds go into any flower pots / farm plots.");

            Helpers.StringsAddProperty("CustomizePlants.PROPERTY.WheezewortTempDelta", "WheezewortTempDelta");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.WheezewortTempDelta_Title", "Wheezewort Temp Delta");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.WheezewortTempDelta_ToolTip", "How much cooling wheezeworts do. Default is -5 Kelvin.");

            Helpers.StringsAddProperty("CustomizePlants.PROPERTY.OxyfernOxygenPerSecond", "OxyfernOxygenPerSecond");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.OxyfernOxygenPerSecond_Title", "Oxyfern Oxygen Per Second");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.OxyfernOxygenPerSecond_ToolTip", "Amount of oxygen released by oxyferns.");

            Helpers.StringsAddProperty("CustomizePlants.PROPERTY.CheatMutationAnalyze", "CheatMutationAnalyze");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.CheatMutationAnalyze_Title", "Cheat Mutation Analyze");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.CheatMutationAnalyze_ToolTip", "Immediately reveals mutations.");

            Helpers.StringsAddProperty("CustomizePlants.PROPERTY.CheatFlowerVase", "CheatFlowerVase");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.CheatFlowerVase_Title", "Cheat Flower Vase");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.CheatFlowerVase_ToolTip", "When true, the basic Flower Pot for decoration plants does not need any irrigation at all, no matter which plant is in it.");

            Helpers.StringsAddProperty("CustomizePlants.PROPERTY.WildFlowerVase", "WildFlowerVase");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.WildFlowerVase_Title", "Wild Flower Vase");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.WildFlowerVase_ToolTip", "When true, the basic Flower Pot grows plants as if they were wild.");

            Helpers.StringsAddProperty("CustomizePlants.PROPERTY.ApplyBugFixes", "ApplyBugFixes");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.ApplyBugFixes_Title", "Apply Bug Fixes");
            Helpers.StringsAdd("CustomizePlants.LOCSTRINGS.ApplyBugFixes_ToolTip", "Fixes crashes caused by other settings. Leave enabled!");
            #endregion
            #region Mod Plants
            Helpers.StringsAddProperty("CustomizePlants.PROPERTY.AutomaticallyAddModPlants", "AutomaticallyAddModPlants");

            Helpers.StringsAddProperty("CustomizePlants.PROPERTY.IgnoreList", "IgnoreList");

            Helpers.StringsAddProperty("CustomizePlants.PROPERTY.ModPlants", "ModPlants");
            #endregion
        }
    }

}
