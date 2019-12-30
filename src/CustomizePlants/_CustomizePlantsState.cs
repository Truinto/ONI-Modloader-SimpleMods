using System.Collections.Generic;
using ONI_Common.Json;
using UnityEngine;
using System;

namespace CustomizePlants
{
    public class CustomizePlantsState
    {
		public int version { get; set; } = 7;
        
        public List<PlantData> PlantSettings { get; set; } = new List<PlantData>() {
            new PlantData(id: BasicSingleHarvestPlantConfig.ID, irrigation: new Dictionary<string, float>() { {"Dirt", 5f} }),
            new PlantData(id: MushroomPlantConfig.ID, irrigation: new Dictionary<string, float>() { }, fruitId: MushroomConfig.ID, fruit_amount: 1, fruit_grow_time: 6*600, input_element: "CarbonDioxide", input_rate: 0.001f),
            new PlantData(id: PrickleFlowerConfig.ID, irrigation: new Dictionary<string, float>() { {"Water", 5f} }),
            new PlantData(id: SeaLettuceConfig.ID, irrigation: new Dictionary<string, float>() { {"SaltWater", 5f} }),
            new PlantData(id: BeanPlantConfig.ID, irrigation: new Dictionary<string, float>() { {"Ethanol", 5f} }),
            new PlantData(id: SpiceVineConfig.ID, irrigation: new Dictionary<string, float>() { {"DirtyWater", 15f}, {"Phosphorite", 1f} }),
            new PlantData(id: ForestTreeConfig.ID, irrigation: new Dictionary<string, float>() { {"DirtyWater", 62.5f }, {"Dirt", 10f} }),
            new PlantData(id: OxyfernConfig.ID, fruitId: OxyfernConfig.SEED_ID, fruit_amount: 1, fruit_grow_time: 20*600, max_age: -1f),
            new PlantData(id: ColdBreatherConfig.ID, irrigation: new Dictionary<string, float>() { }, fruitId: ColdBreatherConfig.SEED_ID, fruit_amount: 1, fruit_grow_time: 20*600, max_age: -1f),
            new PlantData(id: PrickleGrassConfig.ID, safe_elements: new string[] { "Oxygen", "ContaminatedOxygen", "CarbonDioxide", "ChlorineGas" }, input_element: "ChlorineGas", input_rate: 0.01f),
            new PlantData(id: LeafyPlantConfig.ID, irrigation: new Dictionary<string, float>() { {"Water", 10f} }, fruitId: "placeholder1", fruit_grow_time: 4*600f),
            new PlantData(id: CactusPlantConfig.ID, pressures: new float[] { 0f, 0f, 2f, 30f }, safe_elements: new string[] { "Oxygen", "ContaminatedOxygen", "CarbonDioxide", "Methane" }, input_element: "Methane", input_rate: 0.01f, output_element: "CarbonDioxide", output_rate: 0.01f),
            new PlantData(id: EvilFlowerConfig.ID, pressures: new float[] { 0f, 0f, 5f }, safe_elements: new string[] { "CarbonDioxide", "ChlorineGas" }, output_element: "ChlorineGas", output_rate: 0.012f)
            //new PlantData(id: EvilFlowerConfig.ID, irrigation: new Dictionary<string, float>() { {"DirtyWater", 2f}, { "SlimeMold", 2f} },
            //        fruitId: "placeholder", fruit_amount: 100, fruit_grow_time: 600,
            //        illumination: 0f, safe_elements: new string[] { }, temperatures: new float[] { 1f, 253.15f },
            //        pressures: new float[] { 0f, 0.1f }, submerged_threshold: 0f, disease: "FoodPoisoning", disease_amount: 1000,
            //        max_age: 2f, can_tinker: true,
            //        input_element: "Oxygen", input_rate: 0.01f,
            //        output_element: "ChlorineGas", output_rate: 0.01f)
        };

        public Dictionary<string, Dictionary<string, int>> SpecialCropSettings { get; set; } = new Dictionary<string, Dictionary<string, int>>() {
            { "placeholder1", new Dictionary<string, int>() { { "Algae", 40 }, { "Clay", 20 } } }
        };

        public bool SeedsGoIntoAnyFlowerPots { get; set; } = true;
        public float WheezewortTempDelta { get; set; } = -20f;
        public float OxyfernOxygenPerSecond { get; set; } = 0.03125f;
        public bool CheatFlowerVase { get; set; } = false;

        public bool AutomaticallyAddModPlants { get; set; } = false;

        public List<string> ModPlants { get; set; } = new List<string>();

        public static Config.Manager<CustomizePlantsState> StateManager = new Config.Manager<CustomizePlantsState>(Config.Helper.CreatePath("Customize Plants"), true);
        
    }
}