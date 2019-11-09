using Klei.AI;
using System;
using STRINGS;
using UnityEngine;
using Harmony;
using System.Linq;
using System.Collections.Generic;

namespace LessIrrigation
{
    public static class PlantHelper
    {
        public static string[] crop_ids = { "BasicPlantFood", PrickleFruitConfig.ID, MushroomConfig.ID, "ColdWheatSeed", SpiceNutConfig.ID, BasicFabricConfig.ID, SwampLilyFlowerConfig.ID, "GasGrassHarvested", "WoodLog", "Lettuce", "BeanPlantSeed", "OxyfernSeed", SimHashes.Salt.ToString() };

        public static Dictionary<string, string> plantNameConvert = new Dictionary<string, string> { { PrickleFlowerConfig.ID, PrickleFruitConfig.ID } };

        public static bool IsPlantToRemove(string plantID)
        {
            return LessIrrigationState.StateManager.State.RemoveIrrigationFromPlants.Contains(plantID);
        }

        public static void ProcessPlant(GameObject plant)
        {
            ProcessNewIrrigation(plant);
            ProcessLightDark(plant);
        }

        public static void ProcessNewIrrigation(GameObject plant)
        {
            Dictionary<string, float> entry;

            //Debug.Log("ProcessNewIrrigation plant.name:" + plant.name);

            try { entry = LessIrrigationState.StateManager.State.AddIrrigation[plant.name]; }
            catch (Exception) { return; }

            
            int amountIrrigation = 0;
            int amountFertilization = 0;
            for (int i = 0; i < entry.Count; i++)
            {
                if (GameTags.LiquidElements.Contains(new Tag(entry.ElementAt(i).Key)))
                {
                    amountIrrigation++;
                }
                else
                {
                    amountFertilization++;
                }
            }

            PlantElementAbsorber.ConsumeInfo[] consumeIrrigation = new PlantElementAbsorber.ConsumeInfo[amountIrrigation];
            PlantElementAbsorber.ConsumeInfo[] consumeFertilization = new PlantElementAbsorber.ConsumeInfo[amountFertilization];

            amountIrrigation = 0;
            amountFertilization = 0;

            for (int i = 0; i < entry.Count; i++)
            {
                if (GameTags.LiquidElements.Contains(new Tag(entry.ElementAt(i).Key)))
                {
                    consumeIrrigation[amountIrrigation].tag = entry.ElementAt(i).Key;
                    consumeIrrigation[amountIrrigation].massConsumptionRate = entry.ElementAt(i).Value / 600f;
                    amountIrrigation++;

                    //Debug.Log("ProcessNewIrrigation added irrigation at: " + amountIrrigation);
                    //Debug.Log("ProcessNewIrrigation added irrigation TAG: " + entry.ElementAt(i).Key);
                    //Debug.Log("ProcessNewIrrigation added irrigation MASS: " + entry.ElementAt(i).Value);
                }
                else
                {
                    consumeFertilization[amountFertilization].tag = entry.ElementAt(i).Key;
                    consumeFertilization[amountFertilization].massConsumptionRate = entry.ElementAt(i).Value / 600f;
                    amountFertilization++;

                    //Debug.Log("ProcessNewIrrigation added fertilization at: " + amountFertilization);
                    //Debug.Log("ProcessNewIrrigation added fertilization TAG: " + entry.ElementAt(i).Key);
                    //Debug.Log("ProcessNewIrrigation added fertilization MASS: " + entry.ElementAt(i).Value);
                }
            }

            if (amountIrrigation > 0)
                PlantHelper.ExtendPlantToIrrigated(plant, consumeIrrigation);
            if (amountFertilization > 0)
                PlantHelper.ExtendPlantToFertilizable(plant, consumeFertilization);

        }

        public static void ProcessLightDark(GameObject plant)
        {
            int setting = 0;

            //Debug.Log("ProcessLightDark plant.name:" + plant.name);

            try { setting = LessIrrigationState.StateManager.State.SetIllumination[plant.name]; }
            catch (Exception) { return; }

            ChangeLightDark(plant, setting);
        }

        public static bool ProcessTempAtmosphere(ref GameObject template, ref float temperature_lethal_low, ref float temperature_warning_low, ref float temperature_warning_high, ref float temperature_lethal_high, ref SimHashes[] safe_elements, ref bool pressure_sensitive, ref float pressure_lethal_low, ref float pressure_warning_low, ref string crop_id, ref bool can_drown, ref bool can_tinker, ref bool require_solid_tile, ref bool should_grow_old, ref float max_age)
        {
            //Debug.Log("ProcessAtmosphere plant.name:" + plant.name);

            LessIrrigationState.StructTempAtmosphere arguments;

            try { arguments = LessIrrigationState.StateManager.State.SetPlantTempAtmosphere[template.name]; }
            catch (Exception) { return false; }

            temperature_lethal_low = arguments.temperature_lethal_low;
            temperature_warning_low = arguments.temperature_warning_low;
            temperature_warning_high = arguments.temperature_warning_high;
            temperature_lethal_high = arguments.temperature_lethal_high;
            pressure_sensitive = arguments.pressure_sensitive;
            pressure_lethal_low = arguments.pressure_lethal_low;
            pressure_warning_low = arguments.pressure_warning_low;
            if (arguments.crop_id != "") crop_id = arguments.crop_id;
            can_drown = arguments.can_drown;
            can_tinker = arguments.can_tinker;
            require_solid_tile = arguments.require_solid_tile;
            should_grow_old = arguments.should_grow_old;
            max_age = arguments.max_age;

            SimHashes[] safe_elementsHash = new SimHashes[arguments.safe_elements.Count()];

            for (int i = 0; i < arguments.safe_elements.Count(); i++)
            {
                safe_elementsHash[i] = (SimHashes)arguments.safe_elements[i].ToTag().GetHash();
            }

            safe_elements = safe_elementsHash;

            return true;

        }

        public static void ProcessCropVal()
        {
            //Debug.Log("ProcessCropVal start: " + TUNING.CROPS.CROP_TYPES.Count);
            //foreach (Crop.CropVal x in LessIrrigation.StateManager.State.SetPlantCrop.Values)
            //{
            //    Debug.Log("ProcessCropVal states to load: ---" + x.cropId + "---");
            //}

            List<Crop.CropVal> newList = new List<Crop.CropVal>(TUNING.CROPS.CROP_TYPES);

            foreach (KeyValuePair<string, Crop.CropVal> cropval in LessIrrigationState.StateManager.State.SetPlantCrop)
            {
                int index = newList.FindIndex(x => x.cropId == cropval.Key);

                if (index >= 0)
                {
                    newList[index] = cropval.Value;
                }
                else
                {
                    newList.Add(cropval.Value);
                    Debug.Log("ProcessCropVal adds new crop type: " + cropval.Value.cropId);
                }

            }

            TUNING.CROPS.CROP_TYPES = newList;

        }

        public static void ProcessCropVal2()
        {
            //Debug.Log("ProcessCropVal start: " + TUNING.CROPS.CROP_TYPES.Count);
            //foreach (Crop.CropVal x in LessIrrigation.StateManager.State.SetPlantCrop.Values)
            //{
            //    Debug.Log("ProcessCropVal states to load: ---" + x.cropId + "---");
            //}

            List<Crop.CropVal> newList = new List<Crop.CropVal>(TUNING.CROPS.CROP_TYPES.Count);

            for (int i = 0; i < TUNING.CROPS.CROP_TYPES.Count; i++)
            {
                Crop.CropVal newCropVal = TUNING.CROPS.CROP_TYPES[i];

                try
                {
                    //Debug.Log("ProcessCropVal looking for: ---" + newCropVal.cropId + "---");
                    newCropVal = LessIrrigationState.StateManager.State.SetPlantCrop[newCropVal.cropId];
                    Debug.Log("ProcessCropVal applying new CropVal");
                }
                catch (Exception) { } //Debug.Log("ProcessCropVal: " + newCropVal.cropId + " -> "  + e.Message ); }

                newList.Add(newCropVal);
            }

            TUNING.CROPS.CROP_TYPES = newList;

        }

        /// <summary>Adds or destroys illumination component</summary>
        /// <param name="plant">GameObject of plant to modify</param>
        /// <param name="setting">0 = remove functionally, 1 = need light, -1 = need darkness</param>
        public static void ChangeLightDark(GameObject plant, int setting = 0)
        {

            if (setting < 0)    //likes darkness
            {
                plant.AddOrGet<IlluminationVulnerable>().SetPrefersDarkness(true);
            }
            else if (setting > 0)    //like brightness
            {
                plant.AddOrGet<IlluminationVulnerable>().SetPrefersDarkness(false);
            }
            else    //remove function
            {
                IlluminationVulnerable illumination = plant.GetComponent<IlluminationVulnerable>();

                if (illumination != null)
                    UnityEngine.Object.DestroyImmediate(illumination);
                //illumination.enabled = false;
            }
        }

        public static GameObject ExtendPlantToFertilizable(GameObject template, PlantElementAbsorber.ConsumeInfo[] fertilizers)
        {
            HashedString idHash = Db.Get().ChoreTypes.FarmFetch.IdHash;
            foreach (PlantElementAbsorber.ConsumeInfo fertilizer in fertilizers)
            {
                ManualDeliveryKG manualDeliveryKg = template.AddComponent<ManualDeliveryKG>();
                manualDeliveryKg.RequestedItemTag = fertilizer.tag;
                manualDeliveryKg.capacity = (float)((double)fertilizer.massConsumptionRate * 600.0 * 3.0);
                manualDeliveryKg.refillMass = (float)((double)fertilizer.massConsumptionRate * 600.0 * 0.5);
                manualDeliveryKg.minimumMass = (float)((double)fertilizer.massConsumptionRate * 600.0 * 0.5);
                manualDeliveryKg.operationalRequirement = FetchOrder2.OperationalRequirement.Functional;
                manualDeliveryKg.choreTypeIDHash = idHash;
            }
            KPrefabID component1 = template.GetComponent<KPrefabID>();
            FertilizationMonitor.Def def = template.AddOrGetDef<FertilizationMonitor.Def>();
            def.wrongFertilizerTestTag = GameTags.Solid;
            def.consumedElements = fertilizers;
            component1.prefabInitFn += (KPrefabID.PrefabFn)(inst =>
            {
                foreach (ManualDeliveryKG component2 in inst.GetComponents<ManualDeliveryKG>())
                    component2.Pause(true, "init");
            });
            return template;
        }

        public static GameObject ExtendPlantToIrrigated(GameObject template, PlantElementAbsorber.ConsumeInfo[] consume_info)
        {
            HashedString idHash = Db.Get().ChoreTypes.FarmFetch.IdHash;
            foreach (PlantElementAbsorber.ConsumeInfo consumeInfo in consume_info)
            {
                ManualDeliveryKG manualDeliveryKg = template.AddComponent<ManualDeliveryKG>();
                manualDeliveryKg.RequestedItemTag = consumeInfo.tag;
                manualDeliveryKg.capacity = (float)((double)consumeInfo.massConsumptionRate * 600.0 * 3.0);
                manualDeliveryKg.refillMass = (float)((double)consumeInfo.massConsumptionRate * 600.0 * 0.5);
                manualDeliveryKg.minimumMass = (float)((double)consumeInfo.massConsumptionRate * 600.0 * 0.5);
                manualDeliveryKg.operationalRequirement = FetchOrder2.OperationalRequirement.Functional;
                manualDeliveryKg.choreTypeIDHash = idHash;
            }
            IrrigationMonitor.Def def = template.AddOrGetDef<IrrigationMonitor.Def>();
            def.wrongIrrigationTestTag = GameTags.Liquid;
            def.consumedElements = consume_info;
            return template;
        }

        public static void ChangeGrowing(GameObject plant, string crop_id, float newGrowTime, float wildMultiplier = 1) //not tested!
        {
            if (plant == null || crop_id == null || newGrowTime < 1f) return;

            Crop.CropVal cropval = TUNING.CROPS.CROP_TYPES.Find((Predicate<Crop.CropVal>)(m => m.cropId == crop_id));
            cropval.cropDuration = newGrowTime;
            //cropval.numProduced = 1;
            //cropval.renewable = true;
            plant.GetComponent<Crop>().Configure(cropval);

            Growing growing = plant.GetComponent<Growing>();
            growing.growthTime = cropval.cropDuration;
            growing.smi.wildGrowingRate = new AttributeModifier(growing.smi.wildGrowingRate.AttributeId, 0.0004166667f * wildMultiplier, (string)CREATURES.STATS.MATURITY.GROWINGWILD, false, false, true);
            //growing.smi.wildGrowingRate.Value = 0.0004166667f * wildMultiplier; // normal 1/4 of 1 game second
        }

        public static void ChangeIrrigation(GameObject plant, Tag elementTag, float newConsumptionRate, bool isSolid)
        {
            //get solid or liquid monitor
            PlantElementAbsorber.ConsumeInfo[] consumedElements = null;
            if (isSolid)
                consumedElements = plant.GetDef<FertilizationMonitor.Def>().consumedElements;
            else
                consumedElements = plant.GetDef<IrrigationMonitor.Def>().consumedElements;

            if (consumedElements == null)
            {
                Debug.LogWarning("ERROR: ChangeIrrigation consumedElements was null!");
                return;
            }

            //find definition of given element
            for (int i = 0; i < consumedElements.Count(); i++) //foreach (PlantElementAbsorber.ConsumeInfo info in consumedElements)
            {
                if (consumedElements[i].tag == elementTag)
                {

                    //return function if no matching element, else remove when new rate = 0, else set new mass
                    if (newConsumptionRate <= 0f)
                    {
                        consumedElements = consumedElements.Where(idx => idx.tag != consumedElements[i].tag).ToArray(); //make new array, but without "info"
                        if (isSolid)
                            plant.GetDef<FertilizationMonitor.Def>().consumedElements = consumedElements;
                        else
                            plant.GetDef<IrrigationMonitor.Def>().consumedElements = consumedElements;
                    }
                    else
                    {
                        consumedElements[i].massConsumptionRate = newConsumptionRate;
                    }

                    break;
                }
            }

            //find delivery order of given element, destroy when rate = 0, else adapt for new mass
            foreach (ManualDeliveryKG delivery in plant.GetComponents<ManualDeliveryKG>())
            {
                if (delivery.RequestedItemTag == elementTag)
                {
                    if (newConsumptionRate <= 0f)
                    {
                        Debug.Log("ChangeIrrigation DestroyImmediate " + elementTag.Name);
                        UnityEngine.Object.DestroyImmediate(delivery);
                    }
                    else
                    {
                        delivery.capacity = (float)((double)newConsumptionRate * 600.0 * 3.0);
                        delivery.refillMass = (float)((double)newConsumptionRate * 600.0 * 0.5);
                        delivery.minimumMass = (float)((double)newConsumptionRate * 600.0 * 0.5);
                    }
                }
            }

        }
        
        #region Dictionary for names
        public static Dictionary<string, string> NameTranslation = new Dictionary<string, string> {
            { "mealwood", "BasicSingleHarvestPlant" },
            { "dusk cap", "MushroomPlant" },
            { "bristle blossom", "PrickleFlower" },
            { "sleet wheat", "ColdWheat" },
            { "waterweed", "SeaLettuce" },
            { "nosh sprout", "BeanPlant" },
            { "pincha pepper", "SpiceVine" },
            { "balm lily", "SwampLily" },
            { "thimble reed", "BasicFabricPlant" },
            { "arbor tree", "ForestTree" },
            { "dasha saltvine", "SaltPlant" },
            { "gas grass", "GasGrass" },
            { "oxyfern", "Oxyfern" },
            { "wheezewort", "ColdBreather" },
            { "bluff briar", "PrickleGrass" },
            { "buddy bud", "BulbPlant" },
            { "mirth leaf", "LeafyPlant" },
            { "jumping joya", "CactusPlant" },
            { "sporechid", "EvilFlower" },

            //Fruits
            { "meal lice", "BasicPlantFood" },
            { "mushroom", "Mushroom" },
            { "bristle berries", "PrickleFruit" },
            { "sleet wheat grain", "ColdWheatSeed" },
            { "lettuce", "Lettuce" },
            { "nosh bean", "BeanPlantSeed" },
            { "pincha peppernut", "SpiceNut" },
            { "balm lily flower", "SwampLilyFlower" },
            { "reed fiber", "BasicFabric" },
            { "lumber", "WoodLog" },
            { "salt", "Salt" },
            { "gas grass fruit", "GasGrassHarvested" }
        };
        #endregion
    }
}