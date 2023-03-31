using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using static Config.PostBootDialog;
using System.Runtime.Serialization;
using Common;
using Klei.AI;
using System.Reflection;

namespace CustomizePlants
{
    #region Decor plant animation fix
    // the original method attaches a number equal to it's growth statium, however decor plant only have 1 state
    // to make StandardCropPlant compatible with decor plants anims this will skip this attachment
    [HarmonyPatch(typeof(StandardCropPlant.States), "GetWiltAnim")]
    public static class StandardCropPlant_AnimsPatch
    {
        public static bool Prefix(StandardCropPlant.StatesInstance smi, ref string __result)
        {
            if (smi.master.anims.wilt_base.EndsWith("1"))
            {
                __result = smi.master.anims.wilt_base;
                return false;
            }
            return true;
        }
    }
    #endregion

    #region SaltPlant function extentions
    // attaches replant monitor that reduces the element converter/consumer on wild plants
    [HarmonyPatch(typeof(SaltPlant), "OnSpawn")]
    public static class SaltPlant_OnSpawn
    {
        public static void Postfix(SaltPlant __instance)
        {
            __instance.Subscribe((int)GameHashes.PlanterStorage, OnReplanted);
        }

        public static void OnReplanted(object data = null)
        {
            GameObject go = (data as Storage)?.gameObject;
            if (go == null)
                return;

            ReceptacleMonitor replant = go.GetComponent<ReceptacleMonitor>();
            if (replant != null)
            {
                ElementConsumer consumer = go.GetComponent<ElementConsumer>();
                ElementConverter converter = go.GetComponent<ElementConverter>();
                if (replant == null || !replant.Replanted)
                {
                    if (consumer != null) consumer.consumptionRate *= 0.25f;        // NOTE: this doesn't seem to be applied...
                    if (converter != null) converter.SetWorkSpeedMultiplier(0.25f);
                }
            }
        }
    }

    // disables converter/consumer when plant is wilt
    [HarmonyPatch(typeof(SaltPlant), "OnWilt")]
    public static class SaltPlant_OnWilt
    {
        public static bool Prefix(SaltPlant __instance)
        {
            __instance.gameObject.GetComponent<ElementConsumer>()?.EnableConsumption(false);
            ElementConverter converter = __instance.gameObject.GetComponent<ElementConverter>();
            if (converter != null)
            {
                converter.SetWorkSpeedMultiplier(0f);
            }
            return false;
        }
    }

    // enables converter/consumer when plant is no longer wilt
    [HarmonyPatch(typeof(SaltPlant), "OnWiltRecover")]
    public static class SaltPlant_OnWiltRecover
    {
        public static bool Prefix(SaltPlant __instance)
        {
            __instance.gameObject.GetComponent<ElementConsumer>()?.EnableConsumption(true);
            ElementConverter converter = __instance.gameObject.GetComponent<ElementConverter>();
            if (converter != null)
            {
                if (__instance.GetComponent<ReceptacleMonitor>()?.Replanted ?? false)
                    converter.SetWorkSpeedMultiplier(1f);
                else
                    converter.SetWorkSpeedMultiplier(0.25f);
            }
            return false;
        }
    }
    #endregion

    #region Multi-Fruit
    [HarmonyPatch(typeof(Crop), "SpawnConfiguredFruit")]
    public static class Crop_SpawnConfiguredFruit
    {
        public static bool Prefix(Crop __instance)
        {
            Dictionary<string, int> setting;
            if (CustomizePlantsState.StateManager.State.SpecialCropSettings == null ||
                CustomizePlantsState.StateManager.State.SpecialCropSettings.TryGetValue(__instance.cropId, out setting) == false)
                return true;

            foreach (KeyValuePair<string, int> entry in setting)
            {
                __instance.SpawnSomeFruit(entry.Key, entry.Value);
            }

            __instance.Trigger((int)GameHashes.CropPicked, __instance);
            return false;
        }

        public static void SpawnFruit(Crop inst, string fruitId, int amount)
        {
            if (inst == null || string.IsNullOrEmpty(fruitId))
                return;

            GameObject gameObject = Scenario.SpawnPrefab(Grid.PosToCell(inst.gameObject), 0, 0, fruitId, Grid.SceneLayer.Ore);
            if (gameObject != null)
            {
                float y = 0.75f;
                gameObject.transform.SetPosition(gameObject.transform.GetPosition() + new Vector3(0.0f, y, 0.0f));
                gameObject.SetActive(true);
                PrimaryElement component1 = gameObject.GetComponent<PrimaryElement>();
                component1.Units = amount;
                component1.Temperature = inst.gameObject.GetComponent<PrimaryElement>().Temperature;
                Edible component2 = gameObject.GetComponent<Edible>();
                if ((bool)component2)
                    ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, component2.Calories, StringFormatter.Replace(STRINGS.UI.ENDOFDAYREPORT.NOTES.HARVESTED, "{0}", component2.GetProperName()), STRINGS.UI.ENDOFDAYREPORT.NOTES.HARVESTED_CONTEXT);
            }
            else
                Debug.LogWarning("Tried to spawn an invalid crop prefab: " + fruitId);
        }
    }

    [HarmonyPatch(typeof(Crop), "InformationDescriptors")]
    public static class Crop_InformationDescriptors
    {
        public static bool Prefix(Crop __instance, ref List<Descriptor> __result)
        {
            Dictionary<string, int> setting;
            if (CustomizePlantsState.StateManager.State.SpecialCropSettings == null ||
                CustomizePlantsState.StateManager.State.SpecialCropSettings.TryGetValue(__instance.cropId, out setting) == false)
                return true;

            __result = new List<Descriptor>();

            foreach (KeyValuePair<string, int> entry in setting)
            {
                Tag tag = new Tag(entry.Key);
                GameObject prefab = Assets.GetPrefab(tag);
                Edible edible = prefab.GetComponent<Edible>();
                float calories1 = 0.0f;
                string str1 = string.Empty;
                if (edible != null)
                    calories1 = edible.FoodInfo.CaloriesPerUnit;
                float calories2 = calories1 * entry.Value;
                InfoDescription infoDescription = prefab.GetComponent<InfoDescription>();
                if (infoDescription != null)
                    str1 = infoDescription.description;
                string str2 = !GameTags.DisplayAsCalories.Contains(tag) ? (!GameTags.DisplayAsUnits.Contains(tag) ? GameUtil.GetFormattedMass(entry.Value, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}") : GameUtil.GetFormattedUnits(entry.Value, GameUtil.TimeSlice.None, false)) : GameUtil.GetFormattedCalories(calories2, GameUtil.TimeSlice.None, true);

                __result.Add(new Descriptor(string.Format(STRINGS.UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD, prefab.GetProperName(), str2), string.Format(STRINGS.UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.YIELD, str1, GameUtil.GetFormattedCalories(calories1, GameUtil.TimeSlice.None, true), GameUtil.GetFormattedCalories(calories2, GameUtil.TimeSlice.None, true)), Descriptor.DescriptorType.Effect, false));
            }

            if ((__instance.GetComponent<SeedProducer>()?.seedInfo.productionType ?? SeedProducer.ProductionType.DigOnly) == SeedProducer.ProductionType.Harvest)
                __result.Add(new Descriptor(string.Format(STRINGS.UI.UISIDESCREENS.PLANTERSIDESCREEN.BONUS_SEEDS, GameUtil.GetFormattedPercent(10f, GameUtil.TimeSlice.None)), string.Format(STRINGS.UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.BONUS_SEEDS, GameUtil.GetFormattedPercent(10f, GameUtil.TimeSlice.None)), Descriptor.DescriptorType.Effect, false));

            return false;
        }
    }
    #endregion

    //[HarmonyPatch(typeof(Growing.StatesInstance), MethodType.Constructor, typeof(Growing))]
    public class TEST
    {
        public static void Prefix(Growing master)
        {
            var x = AccessTools.Field(typeof(Growing), "maturity").GetValue(master) as AmountInstance;
        }
    }

    public static class PlantHelper
    {
        public static PropertyInfo _AttributeModifierValue = AccessTools.Property(typeof(AttributeModifier), "Value");
        public static StandardCropPlant.AnimSet DecorAnim = new() { grow = "idle", grow_pst = "idle", harvest = "idle", idle_full = "idle", wilt_base = "wilt1" };   //wilt1, grow_seed

        public static void ProcessPlant(GameObject plant)
        {
            var match = PlantLookupPatch.FindBetweenLink.Match(plant.GetProperName() ?? "null");
            string displayName = match.Success ? match.Groups[1].Value : null;

            PlantData setting = CustomizePlantsState.StateManager.State.PlantSettings?.FirstOrDefault(t => t.id == plant.PrefabID() || t.id == displayName);
            if (setting == null) return;

            #region trait fix
            plant.AddOrGet<Traits>();
            Modifiers modifiers = plant.AddOrGet<Modifiers>();
            Trait baseTrait;
            {
                string traitName = modifiers.initialTraits.FirstOrDefault();
                if (traitName == null)
                {
                    traitName = plant.PrefabID() + "Modded";
                    modifiers.initialTraits.Add(traitName);
                    baseTrait = Db.Get().CreateTrait(traitName, plant.name, plant.name, null, false, null, true, true);
                    Helpers.Print("New trait: " + traitName);
                }
                else
                {
                    baseTrait = Db.Get().traits.Get(traitName) ?? throw new Exception(plant.PrefabID() + " adds a trait that doesn't exist.");
                }
            }
            #endregion
            #region decor plant fixes; including seeds
            if (setting.fruitId != null)    //decor plant fixes
            {
                PrickleGrass grass = plant.GetComponent<PrickleGrass>();
                if (grass != null || plant.PrefabID() == ColdBreatherConfig.ID || plant.PrefabID() == EvilFlowerConfig.ID)
                {
                    UnityEngine.Object.DestroyImmediate(grass); //what happens if this is null?
                    plant.AddOrGet<StandardCropPlant>();

                    KPrefabID prefab = plant.GetComponent<KPrefabID>();
                    prefab.prefabInitFn += (inst =>
                    {
                        StandardCropPlant stdcrop2 = inst.GetComponent<StandardCropPlant>();
                        stdcrop2.anims = PlantHelper.DecorAnim;
                    });

                    //KBatchedAnimController kbatchedAnimController = plant.AddOrGet<KBatchedAnimController>();
                    //kbatchedAnimController.AnimFiles = new KAnimFile[1]
                    //{
                    //  Assets.GetAnim("bristleblossom_kanim")
                    //};
                    //kbatchedAnimController.initialAnim = "idle_empty";
                }

                SeedProducer seedProducer = plant.GetComponent<SeedProducer>(); // this fixes the missing mutantion on deco plant seeds
                if (seedProducer != null)
                {
                    seedProducer.seedInfo.productionType = SeedProducer.ProductionType.Harvest;
                    seedProducer.seedInfo.newSeedsProduced = 1;

                    var mutantseed = Assets.TryGetPrefab(seedProducer.seedInfo.seedId)?.AddOrGet<MutantPlant>();
                    if (mutantseed != null)
                    {
                        mutantseed.SpeciesID = plant.GetComponent<KPrefabID>().PrefabTag;
                    }

                    var mutantcompost = Assets.TryGetPrefab("Compost" + plant.GetComponent<KPrefabID>().name)?.AddOrGet<MutantPlant>();
                    if (mutantcompost != null)
                    {
                        mutantcompost.SpeciesID = mutantseed.SpeciesID;
                    }
                }
            }
            #endregion

            #region fruitId
            if (setting.fruitId != null || setting.fruit_grow_time != null || setting.fruit_amount != null)    //actual setting fruit
            {
                GeneratedBuildings.RegisterWithOverlay(OverlayScreen.HarvestableIDs, plant.PrefabID().ToString());
                Crop crop = plant.AddOrGet<Crop>();
                Crop.CropVal cropval = crop.cropVal;   //this is a copy
                if (setting.fruitId != null) cropval.cropId = setting.fruitId;
                if (cropval.cropId == "") cropval.cropId = "WoodLog";
                if (setting.fruit_grow_time != null) cropval.cropDuration = (float)setting.fruit_grow_time;
                if (cropval.cropDuration < 1f) cropval.cropDuration = 1f;
                if (setting.fruit_amount != null) cropval.numProduced = (int)setting.fruit_amount;
                if (cropval.numProduced < 1) cropval.numProduced = 1;
                crop.Configure(cropval);

                EnsureAttribute(modifiers, baseTrait, Db.Get().PlantAttributes.YieldAmount.Id, cropval.numProduced);
                EnsureAmount(modifiers, baseTrait, Db.Get().Amounts.Maturity, maxValue: cropval.cropDuration / 600f);

                plant.AddOrGet<Growing>();
                if (setting.id != ForestTreeConfig.ID)  // don't harvest arbor trees directly
                    plant.AddOrGet<Harvestable>();
                plant.AddOrGet<HarvestDesignatable>();
                if (DlcManager.FeaturePlantMutationsEnabled())
                {
                    var mutant = plant.AddOrGet<MutantPlant>();
                    mutant.SpeciesID = plant.GetComponent<KPrefabID>().PrefabTag;
                    Helpers.Print("Set mutant.SpeciesID to " + mutant.SpeciesID);
                    SymbolOverrideControllerUtil.AddToPrefab(plant);
                }

                plant.AddOrGet<StandardCropPlant>();
            }
            #endregion
            #region irrigation
            if (setting.irrigation != null)
            {
                RemoveIrrigation(plant);

                List<PlantElementAbsorber.ConsumeInfo> irrigation = new List<PlantElementAbsorber.ConsumeInfo>(3);
                List<PlantElementAbsorber.ConsumeInfo> fertilization = new List<PlantElementAbsorber.ConsumeInfo>(3);
                foreach (KeyValuePair<string, float> entry in setting.irrigation)
                {
                    Tag tag = entry.Key;
                    var element = ElementLoader.elements.FirstOrDefault(f => f.tag == tag || f.oreTags.Contains(tag));

                    if (element.IsSolid)
                        fertilization.Add(new PlantElementAbsorber.ConsumeInfo(tag, entry.Value / 600f));
                    else if (element.IsLiquid)
                        irrigation.Add(new PlantElementAbsorber.ConsumeInfo(tag, entry.Value / 600f));
                    else
                        Debug.Log(ToDialog("Irrigation for " + setting.id + " defines bad element: " + tag));
                }
                if (irrigation.Count > 0)
                    EntityTemplates.ExtendPlantToIrrigated(plant, irrigation.ToArray());
                if (fertilization.Count > 0)
                    EntityTemplates.ExtendPlantToFertilizable(plant, fertilization.ToArray());
            }
            #endregion
            #region illumination
            if (setting.illumination != null)
            {
                IlluminationVulnerable illumination = plant.GetComponent<IlluminationVulnerable>();
                CropSleepingMonitor.Def cropSleep = plant.GetDef<CropSleepingMonitor.Def>();

                if (setting.illumination == 0f)
                {
                    if (illumination != null)
                        UnityEngine.Object.DestroyImmediate(illumination);
                    if (cropSleep != null)
                        Helpers.RemoveDef(plant, cropSleep);
                }
                else if (setting.illumination < 0f)
                {
                    if (illumination == null)
                        illumination = plant.AddOrGet<IlluminationVulnerable>();
                    if (cropSleep != null)
                        Helpers.RemoveDef(plant, cropSleep);

                    illumination.SetPrefersDarkness(true);
                }
                else if (setting.illumination == 1f)
                {
                    if (illumination == null)
                        illumination = plant.AddOrGet<IlluminationVulnerable>();
                    if (cropSleep != null)
                        Helpers.RemoveDef(plant, cropSleep);

                    illumination.SetPrefersDarkness(false);
                }
                else
                {
                    if (illumination != null)
                        UnityEngine.Object.DestroyImmediate(illumination);
                    if (cropSleep == null)
                        cropSleep = plant.AddOrGetDef<CropSleepingMonitor.Def>();
                    cropSleep.prefersDarkness = false;

                    EnsureAttribute(modifiers, baseTrait, Db.Get().PlantAttributes.MinLightLux.Id, setting.illumination.Value);
                }
            }
            #endregion
            #region safe_elements
            if (setting.safe_elements != null)
            {
                PressureVulnerable pressure = plant.AddOrGet<PressureVulnerable>();
                pressure.safe_atmospheres.Clear();
                foreach (string safe_element in setting.safe_elements)
                {
                    Element element = ElementLoader.FindElementByName(safe_element);
                    if (element == null)
                        Helpers.PrintDialog("Element for safe_element does not exist: " + safe_element);
                    else
                        pressure.safe_atmospheres.Add(element);
                }

                plant.GetComponent<KPrefabID>().prefabInitFn += delegate (GameObject inst)
                {
                    PressureVulnerable pressure2 = inst.GetComponent<PressureVulnerable>();
                    if (pressure2 != null)
                    {
                        pressure2.safe_atmospheres.Clear();
                        foreach (var sh in setting.safe_elements)
                        {
                            Element element = ElementLoader.FindElementByName(sh);
                            if (element != null)
                                pressure2.safe_atmospheres.Add(element);
                        }
                    }
                };
            }
            #endregion
            #region pressure
            if (setting.pressures != null)
            {
                PressureVulnerable pressure = plant.AddOrGet<PressureVulnerable>();
                pressure.pressureLethal_Low = 0f;
                pressure.pressureWarning_Low = 0f;
                pressure.pressureWarning_High = float.MaxValue;
                pressure.pressureLethal_High = float.MaxValue;
                pressure.pressure_sensitive = false;

                for (int i = 0; i < setting.pressures.Length; i++)
                {
                    switch (i)
                    {
                        case 0: pressure.pressureLethal_Low = setting.pressures[i]; pressure.pressure_sensitive = true; break;
                        case 1: pressure.pressureWarning_Low = setting.pressures[i]; break;
                        case 2: pressure.pressureWarning_High = setting.pressures[i]; break;
                        case 3: pressure.pressureLethal_High = setting.pressures[i]; break;
                    }
                }
            }
            #endregion
            #region decor
            try
            {
                if (setting.decor_value != null)
                {
                    plant.GetComponent<DecorProvider>().baseDecor = (float)setting.decor_value;
                }

                if (setting.decor_radius != null)
                {
                    plant.GetComponent<DecorProvider>().baseRadius = (float)setting.decor_radius;
                }
            }
            catch (Exception)
            {
                Debug.LogWarning("[CustomizePlants] For some weird reason " + plant.PrefabID() + " has no DecorProvider.");
            }
            #endregion
            #region temperatures
            if (setting.temperatures != null)
            {
                TemperatureVulnerable temperature = plant.AddOrGet<TemperatureVulnerable>();

                switch (setting.temperatures.Length)
                {
                    case 1:
                        temperature.Configure(
                            tempLethalLow: setting.temperatures[0],
                            tempWarningLow: setting.temperatures[0],
                            tempWarningHigh: 9999f,
                            tempLethalHigh: 9999f);
                        break;
                    case 2:
                        temperature.Configure(
                            tempLethalLow: setting.temperatures[0],
                            tempWarningLow: setting.temperatures[1],
                            tempWarningHigh: 9999f,
                            tempLethalHigh: 9999f);
                        break;
                    case 3:
                        temperature.Configure(
                            tempLethalLow: setting.temperatures[0],
                            tempWarningLow: setting.temperatures[1],
                            tempWarningHigh: setting.temperatures[2],
                            tempLethalHigh: 9999f);
                        break;
                    case 4:
                        temperature.Configure(
                            tempLethalLow: setting.temperatures[0],
                            tempWarningLow: setting.temperatures[1],
                            tempWarningHigh: setting.temperatures[2],
                            tempLethalHigh: setting.temperatures[3]);
                        break;
                    default:
                        break;
                }
            }
            #endregion
            #region submerged_threshold
            if (setting.submerged_threshold != null)
            {
                DrowningMonitor drowning = plant.AddOrGet<DrowningMonitor>();

                if (setting.submerged_threshold == 0f)   //doesn't care about water
                    UnityEngine.Object.DestroyImmediate(drowning);
                else if (setting.submerged_threshold < 0f)   //needs water
                {
                    drowning.livesUnderWater = true;
                    drowning.canDrownToDeath = false;
                }
                else    //if(setting.submerged_threshold > 0f)  //hates water
                {
                    drowning.livesUnderWater = false;
                    drowning.canDrownToDeath = false;
                }

            }
            #endregion
            #region can_tinker
            if (setting.can_tinker != null)
            {
                if (setting.can_tinker == true)
                    Tinkerable.MakeFarmTinkerable(plant);
            }
            #endregion
            #region require_solid_tile
            if (setting.require_solid_tile != null)
            {
                UprootedMonitor uproot = plant.AddOrGet<UprootedMonitor>();
                if (setting.require_solid_tile == false)
                    UnityEngine.Object.DestroyImmediate(uproot);
            }
            #endregion
            #region max_age
            if (setting.max_age != null && plant.GetComponent<StandardCropPlant>() != null) //only if plant has fruit
            {
                Growing growing = plant.AddOrGet<Growing>();
                if (setting.max_age <= 0)
                    growing.shouldGrowOld = false;
                else
                {
                    growing.shouldGrowOld = true;
                    growing.maxAge = (float)setting.max_age;
                }
            }
            #endregion
            #region disease
            if (setting.disease != null || setting.disease_amount != null)
            {
                DiseaseDropper.Def def = plant.AddOrGetDef<DiseaseDropper.Def>();
                if (setting.disease != null)
                    def.diseaseIdx = Db.Get().Diseases.GetIndex(setting.disease);
                if (setting.disease_amount != null)
                    def.singleEmitQuantity = (int)setting.disease_amount;

                if (def.diseaseIdx == byte.MaxValue || def.singleEmitQuantity == 0)
                    Common.Helpers.RemoveDef(plant, def);
            }
            #endregion
            #region input_element
            if (setting.input_element != null)
            {
                ElementConsumer consumer = plant.AddOrGet<ElementConsumer>();
                Element element = ElementLoader.FindElementByName(setting.input_element);

                if (element == null || element.IsSolid)                //invalid element
                {
                    Debug.Log(ToDialog("input_element is bad element: " + setting.input_element));
                    UnityEngine.Object.DestroyImmediate(consumer);
                }
                else if (setting.input_rate <= 0f)   //delete consumer
                {
                    UnityEngine.Object.DestroyImmediate(consumer);
                }
                else
                {
                    consumer.configuration = ElementConsumer.Configuration.Element;
                    consumer.consumptionRadius = 2;
                    consumer.sampleCellOffset = new Vector3(0f, 0f);
                    consumer.EnableConsumption(true);
                    consumer.showInStatusPanel = true;
                    consumer.storeOnConsume = false;    //consumer deletes elements; output_element might overrides this
                    consumer.consumptionRate = (float)setting.input_rate;
                    consumer.elementToConsume = element.id;
                    consumer.capacityKG = (float)setting.input_rate * 10;

                    plant.AddOrGet<Storage>().capacityKg = consumer.capacityKG;
                    plant.AddOrGet<SaltPlant>();
                }
            }
            #endregion
            #region output_element
            if (setting.output_element != null)
            {
                ElementConsumer consumer = plant.GetComponent<ElementConsumer>();
                ElementConverter converter = plant.AddOrGet<ElementConverter>();
                Element element = ElementLoader.FindElementByName(setting.output_element);

                if (element == null)                //invalid element
                {
                    Debug.Log(ToDialog("output_element is bad element: " + setting.output_element));
                    UnityEngine.Object.DestroyImmediate(converter);
                }
                else if (setting.output_rate <= 0f)  //delete converter
                {
                    UnityEngine.Object.DestroyImmediate(converter);
                    if (consumer != null) consumer.storeOnConsume = false;
                }
                else
                {
                    if (consumer != null)   //transform elements
                    {
                        consumer.storeOnConsume = true;
                        converter.consumedElements = new ElementConverter.ConsumedElement[1] { new ElementConverter.ConsumedElement(consumer.elementToConsume.CreateTag(), consumer.consumptionRate) };
                        converter.OutputMultiplier = (float)setting.output_rate / consumer.consumptionRate;
                        //Debug.Log("TAG is: " + consumer.elementToConsume.CreateTag().Name + " SimHash is: " + consumer.elementToConsume.ToString());
                    }
                    else    //create from nothing
                    {
                        converter.consumedElements = new ElementConverter.ConsumedElement[0];
                        converter.OutputMultiplier = 1f;
                    }
                    converter.outputElements = new ElementConverter.OutputElement[1] { new ElementConverter.OutputElement((float)setting.output_rate, element.id, 0f, true, false, 0f, 1f) };

                    plant.AddOrGet<Storage>();
                    plant.AddOrGet<SaltPlant>();
                }

            }
            #endregion
            #region radiation
            if (setting.radiation != null)
            {
                var radiation = plant.AddOrGet<RadiationEmitter>();
                radiation.emitRads = setting.radiation.Value;
            }
            if (setting.radiation_radius != null)
            {
                var radiation = plant.GetComponent<RadiationEmitter>();
                if (radiation != null)
                {
                    radiation.emitRadiusX = (short)setting.radiation_radius.Value;
                    radiation.emitRadiusY = radiation.emitRadiusX;
                }
            }
            #endregion
            #region radiation_threshold
            if (setting.radiation_threshold_min != null)
            {
                EnsureAttribute(modifiers, baseTrait, Db.Get().PlantAttributes.MinRadiationThreshold.Id, setting.radiation_threshold_min.Value);
            }
            if (setting.radiation_threshold_max != null)
            {
                EnsureAttribute(modifiers, baseTrait, Db.Get().PlantAttributes.MaxRadiationThreshold.Id, setting.radiation_threshold_max.Value);
            }
            #endregion
        }

        public static void ReadPlant(GameObject plant, ref PlantData setting)
        {
            if (setting == null) return;

            #region fruitId
            Crop crop = plant.GetComponent<Crop>();
            if (crop != null)
            {
                setting.fruitId = crop.cropVal.cropId;
                setting.fruit_amount = crop.cropVal.numProduced;
                setting.fruit_grow_time = crop.cropVal.cropDuration;
            }
            #endregion
            #region irrigation
            setting.irrigation = new Dictionary<string, float>();

            var def1 = plant.GetDef<IrrigationMonitor.Def>();
            if (def1 != null)
                foreach (var irrigation in def1.consumedElements)
                    setting.irrigation.Add(irrigation.tag.ToString(), irrigation.massConsumptionRate * 600f);

            var def2 = plant.GetDef<FertilizationMonitor.Def>();
            if (def2 != null)
                foreach (var irrigation in def2.consumedElements)
                    setting.irrigation.Add(irrigation.tag.ToString(), irrigation.massConsumptionRate * 600f);
            #endregion
            #region safe_elements & pressure
            var pressure = plant.GetComponent<PressureVulnerable>();
            if (pressure != null && pressure.safe_atmospheres != null)
            {
                setting.safe_elements = pressure.safe_atmospheres.Select(s => s.tag.ToString()).ToArray();

                if (pressure.pressure_sensitive)
                {
                    setting.pressures = new float[] { pressure.pressureLethal_Low, pressure.pressureWarning_Low, pressure.pressureWarning_High, pressure.pressureLethal_High };
                }
            }
            #endregion
            #region temperatures
            var temperature = plant.GetComponent<TemperatureVulnerable>();
            if (temperature != null)
            {
                setting.temperatures = new float[] { temperature.TemperatureLethalLow, temperature.TemperatureWarningLow, temperature.TemperatureWarningHigh, temperature.TemperatureLethalHigh };
            }
            #endregion
        }

        public static void RemoveIrrigation(GameObject plant)
        {
            StateMachineController controller = plant.GetComponent<StateMachineController>();
            if (controller != null)
            {
                IrrigationMonitor.Def def = plant.GetDef<IrrigationMonitor.Def>();
                if (def != null)
                    controller.cmpdef.defs.Remove(def);

                FertilizationMonitor.Def def2 = plant.GetDef<FertilizationMonitor.Def>();
                if (def2 != null)
                    controller.cmpdef.defs.Remove(def2);
            }

            ManualDeliveryKG[] delivers = plant.GetComponents<ManualDeliveryKG>();
            foreach (ManualDeliveryKG deliver in delivers)
                UnityEngine.Object.DestroyImmediate(deliver);
        }

        public static void EnsureAttribute(Modifiers modifiers, Trait baseTrait, string attributeId, float value, bool isMultiplier = false)
        {
            Helpers.PrintDebug($"EnsureAttribute {modifiers.PrefabID()}");

            if (!modifiers.initialAttributes.Contains(attributeId))
            {
                modifiers.initialAttributes.Add(attributeId);
                Helpers.PrintDebug($" add Attribute {attributeId}");
            }

            var attribute = baseTrait.SelfModifiers.Find(s => s.AttributeId == attributeId);
            if (attribute == null)
                baseTrait.Add(new AttributeModifier(attributeId, value, null, isMultiplier, false, true));
            else
                _AttributeModifierValue.SetValue(attribute, value, null);
        }

        public static void EnsureAmount(Modifiers modifiers, Trait baseTrait, Amount amount, float? maxValue = null, float? value = null, bool isMultiplier = false)
        {
            Helpers.PrintDebug($"EnsureAmount {modifiers.PrefabID()}");

            if (!modifiers.initialAmounts.Contains(amount.Id))
            {
                modifiers.initialAmounts.Add(amount.Id);
                Helpers.PrintDebug($" add Amount {amount.Id}");
            }

            if (value != null)
            {
                var attribute = baseTrait.SelfModifiers.Find(s => s.AttributeId == amount.Id);
                if (attribute == null)
                    baseTrait.Add(new AttributeModifier(amount.Id, value.Value, null, isMultiplier, false, true));
                else
                    _AttributeModifierValue.SetValue(attribute, value.Value, null);
            }

            if (maxValue != null)
            {
                var maxAttribute = baseTrait.SelfModifiers.Find(s => s.AttributeId == amount.maxAttribute.Id);
                if (maxAttribute == null)
                    baseTrait.Add(new AttributeModifier(amount.maxAttribute.Id, maxValue.Value, null, isMultiplier, false, true));
                else
                    _AttributeModifierValue.SetValue(maxAttribute, maxValue.Value, null);
            }
        }

    }
}
