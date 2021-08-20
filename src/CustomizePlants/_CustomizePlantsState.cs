using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.IO;
using PeterHan.PLib.Options;
using Newtonsoft.Json;

namespace CustomizePlants
{
    [ConfigFile("Customize Plants", true, true)]
    [RestartRequired]
    public class CustomizePlantsState
    {
        public int version { get; set; } = 25;

        #region Buttons
        [Option("Reset: Custom Default", "Re-installs mod's default settings.")]
        [JsonIgnore]
        public System.Action<object> ResetToCustomDefault => delegate (object nix)
        {
            StateManager.TrySaveConfigurationState(new CustomizePlantsState());

            OptionsDialog.Last?.CloseDialog();
            OptionsDialog.Last?.CheckForRestart();
            OptionsDialog.Last = null;
        };

        [Option("Preset: Clear", "Clears the list. Restart to apply!")]
        [JsonIgnore]
        public System.Action<object> PresetClear => delegate (object nix)
        {
            PlantSettings.Clear();

            StateManager.TrySaveConfigurationState();

            OptionsDialog.Last?.CloseDialog();
            OptionsDialog.Last?.CheckForRestart();
            OptionsDialog.Last = null;
        };

        [Option("Preset: Basic", "Fills the list with names of all base game plants.")]
        [JsonIgnore]
        public System.Action<object> PresetBasic => delegate (object nix)
        {
            foreach (string plant in PLANTS.NAMES)
            {
                if (!PlantSettings.Any(a => a.id == plant))
                    PlantSettings.Add(new PlantData(plant));
            }

            StateManager.TrySaveConfigurationState();

            OptionsDialog.Last?.CloseDialog();
            OptionsDialog.Last = null;
        };

        [Option("Preset: Read All", "Fills the list with most properties currently applied. If you want the base game properties, run \"Preset: Clear\" and restart before using this.")]
        [JsonIgnore]
        public System.Action<object> PresetEverything => delegate (object nix)
        {
            foreach (string plant in PLANTS.NAMES)
            {
                if (!PlantSettings.Any(a => a.id == plant))
                    PlantSettings.Add(new PlantData(plant));
            }

            foreach (var prefab in Assets.Prefabs)
            {
                var setting = PlantSettings.FirstOrDefault(f => f.id == prefab.name);
                if (setting != null)
                    PlantHelper.ReadPlant(prefab.gameObject, ref setting);
            }

            StateManager.TrySaveConfigurationState();

            OptionsDialog.Last?.CloseDialog();
            OptionsDialog.Last = null;
        };

        [Option("Preset: Half", "Goes through the list and halfs all irrigations. Restart when you are done.")]
        [JsonIgnore]
        public System.Action<object> PresetHalf => delegate (object nix)
        {
            foreach (var setting in PlantSettings)
            {
                foreach (var irr in setting.irrigation)
                    setting.irrigation[irr.Key] *= 0.5f;
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

        public HashSet<PlantMutationData> MutationSettings { get; set; } = new HashSet<PlantMutationData>() {
        };
        #endregion

        #region Extras
        [Option("Seeds Go Into Any Flower Pots", "Whenever or not all seeds go into any flower pots / farm plots.")]
        public bool SeedsGoIntoAnyFlowerPots { get; set; } = true;
        [Option("Wheezewort Temp Delta", "How much cooling wheezeworts do. Default is -5 Kelvin.")]
        public float WheezewortTempDelta { get; set; } = -20f;
        [Option("Oxyfern Oxygen Per Second", "Amount of oxygen released by oxyferns.")]
        public float OxyfernOxygenPerSecond { get; set; } = 0.03125f;
        [Option("Cheat Mutation Analyze", "Immediately reveals mutations.")]
        public bool CheatMutationAnalyze { get; set; } = false;
        [Option("Cheat Flower Vase", "When true, the basic Flower Pot for decoration plants does not need any irrigation at all, no matter which plant is in it.")]
        public bool CheatFlowerVase { get; set; } = false;
        [Option("Wild Flower Vase", "When true, the basic Flower Pot grows plants as if they were wild.")]
        public bool WildFlowerVase { get; set; } = false;
        [Option("Apply Bug Fixes", "Fixes crashes caused by other settings. Leave enabled!")]
        public bool ApplyBugFixes { get; set; } = true;
        #endregion

        public bool AutomaticallyAddModPlants { get; set; } = false;
        public HashSet<string> IgnoreList { get; set; } = new HashSet<string>();
        public HashSet<string> ModPlants { get; set; } = new HashSet<string>();

        public static Config.Manager<CustomizePlantsState> StateManager = new Config.Manager<CustomizePlantsState>(Config.PathHelper.CreatePath("Customize Plants"), true, UpdateFunction);

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
}