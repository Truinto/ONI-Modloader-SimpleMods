using System.Collections.Generic;
using ONI_Common.Json;
using UnityEngine;
using System;
using ModHelper;

namespace LessIrrigation
{
    public class LessIrrigationState
    {
        public string[] RemoveIrrigationFromPlants { get; set; } = {
            ColdBreatherConfig.ID,
            PrickleFlowerConfig.ID,
            ColdWheatConfig.ID,
            SpiceVineConfig.ID,
            ForestTreeConfig.ID,
            BeanPlantConfig.ID,
            OxyfernConfig.ID
        };

        public Dictionary<string, Dictionary<string, float>> AddIrrigation { get; set; } = new Dictionary<string, Dictionary<string, float>> {
            { PrickleFlowerConfig.ID, new Dictionary<string, float> { { SimHashes.Water.CreateTag().Name, 5f } }},
            { ColdWheatConfig.ID, new Dictionary<string, float> { { SimHashes.Water.CreateTag().Name, 5f } }},
            { SpiceVineConfig.ID, new Dictionary<string, float> { { SimHashes.DirtyWater.CreateTag().Name, 7f }, { SimHashes.Phosphorite.CreateTag().Name, 1f } }},
            { ForestTreeConfig.ID, new Dictionary<string, float> { { SimHashes.DirtyWater.CreateTag().Name, 62.5f }, { SimHashes.Dirt.CreateTag().Name, 10f } }},
            { BeanPlantConfig.ID, new Dictionary<string, float> { { SimHashes.Ethanol.CreateTag().Name, 5f }, { SimHashes.Dirt.CreateTag().Name, 5f } }},
            { OxyfernConfig.ID, new Dictionary<string, float> { { SimHashes.Water.CreateTag().Name, 18f } }}
        };

        public Dictionary<string, int> SetIllumination { get; set; } = new Dictionary<string, int> { { MushroomConfig.ID, -1 }, { PrickleFlowerConfig.ID, 1 } };

        public Dictionary<string, StructTempAtmosphere> SetPlantTempAtmosphere { get; set; } = new Dictionary<string, StructTempAtmosphere> {
            { PrickleFlowerConfig.ID, new StructTempAtmosphere(218.15f, 273.15f, 308.15f, 398.15f,
                    new string[] {SimHashes.Oxygen.CreateTag().Name, SimHashes.ContaminatedOxygen.CreateTag().Name, SimHashes.CarbonDioxide.CreateTag().Name },
                    true, 0f, 0.10f) }
            };

        public Dictionary<string, Crop.CropVal> SetPlantCrop { get; set; } = new Dictionary<string, Crop.CropVal> {
            { ColdWheatConfig.SEED_ID, new Crop.CropVal(ColdWheatConfig.SEED_ID, 10800f, 18, true)}
        };

        public bool SeedsGoIntoAnyFlowerPots { get; set; } = true;
        
        public float WheezewortTempDelta { get; set; } = -20f;

        public float OxyfernOxygenPerSecond { get; set; } = 0.03125f;

        public static BaseStateManager<LessIrrigationState> StateManager
			= new BaseStateManager<LessIrrigationState>(new ModFolderPathHelper("LessIrrigation", 1818145851L).path,
                ONI_Common.Paths.GetLogsPath() + ModFolderPathHelper.sep + "LessIrrigationLog.txt");
        //public static BaseStateManager<LessIrrigation> StateManager = new BaseStateManager<LessIrrigation>("LessIrrigation");

        //returns true when error needed fixing
        public bool FixErrors(string[] checkArray)
        {
            bool flag = false;
            for (int index = 0; index < checkArray.Length; index++)
            {
                if ( FixError(ref checkArray[index]) )
                    flag = true;
            }
            return flag;
        }

        //returns true when error needed fixing
        public bool FixError(ref string checkString)
        {
            try
            {
                checkString = PlantHelper.NameTranslation[checkString];
            }
            catch (Exception) { return true; }

            return false;
        }
        
        public struct StructTempAtmosphere
        {
            public float temperature_lethal_low;
            public float temperature_warning_low;
            public float temperature_warning_high;
            public float temperature_lethal_high;
            public string[] safe_elements;
            public bool pressure_sensitive;
            public float pressure_lethal_low;
            public float pressure_warning_low;
            public string crop_id;
            public bool can_drown;
            public bool can_tinker;
            public bool require_solid_tile;
            public bool should_grow_old;
            public float max_age;

            public StructTempAtmosphere(float temperature_lethal_low, float temperature_warning_low, float temperature_warning_high, float temperature_lethal_high, string[] safe_elements, bool pressure_sensitive = true, float pressure_lethal_low = 0f, float pressure_warning_low = 0.15f, string crop_id = "", bool can_drown = true, bool can_tinker = true, bool require_solid_tile = true, bool should_grow_old = true, float max_age = 2400f)
            {
                this.temperature_lethal_low = temperature_lethal_low;
                this.temperature_warning_low = temperature_warning_low;
                this.temperature_warning_high = temperature_warning_high;
                this.temperature_lethal_high = temperature_lethal_high;
                this.safe_elements = safe_elements;
                this.pressure_sensitive = pressure_sensitive;
                this.pressure_lethal_low = pressure_lethal_low;
                this.pressure_warning_low = pressure_warning_low;
                this.crop_id = crop_id;
                this.can_drown = can_drown;
                this.can_tinker = can_tinker;
                this.require_solid_tile = require_solid_tile;
                this.should_grow_old = should_grow_old;
                this.max_age = max_age;
            }
        }
    }
}