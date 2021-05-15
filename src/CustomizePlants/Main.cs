using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using Harmony;
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
#if DLC1
    [HarmonyPatch(typeof(Crop), "SpawnConfiguredFruit")]
#else
    [HarmonyPatch(typeof(Crop), "SpawnFruit")]
#endif
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
#if DLC1
                __instance.SpawnSomeFruit(entry.Key, entry.Value);
#else
                SpawnFruit(__instance, entry.Key, entry.Value);
#endif
            }

            __instance.Trigger((int)GameHashes.CropPicked);
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

            Helpers.Print($"HELLO WORLD {master.gameObject.PrefabID()} x is {x}");
        }
    }

    public static class PlantHelper
    {
        public static PropertyInfo _AttributeModifierValue = AccessTools.Property(typeof(AttributeModifier), "Value");
        public static StandardCropPlant.AnimSet DecorAnim = new StandardCropPlant.AnimSet() { grow = "idle", grow_pst = "idle", harvest = "idle", idle_full = "idle", wilt_base = "wilt1" };   //wilt1, grow_seed

        public static void ProcessPlant(GameObject plant)
        {
            PlantData setting = CustomizePlantsState.StateManager.State.PlantSettings?.FirstOrDefault(t => t.id == plant.PrefabID());
            if (setting == null) return;

            #region trait fix
#if DLC1
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
#endif
            #endregion
            #region decor plant fixes
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

                SeedProducer seed = plant.GetComponent<SeedProducer>();
                if (seed != null)
                {
                    seed.seedInfo.productionType = SeedProducer.ProductionType.Harvest;
                    seed.seedInfo.newSeedsProduced = 1;
                }
            }
            #endregion

            #region fruitId
#if DLC1
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
                    plant.AddOrGet<MutantPlant>();
                plant.AddOrGet<StandardCropPlant>();
            }
#else
            if (setting.fruitId != null || setting.fruit_grow_time != null || setting.fruit_amount != null)    //actual setting fruit
            {
                Crop crop = plant.AddOrGet<Crop>();
                Crop.CropVal cropval = crop.cropVal;   //this is a copy
                if (setting.fruitId != null) cropval.cropId = setting.fruitId;
                if (cropval.cropId == "") cropval.cropId = "WoodLog";
                if (setting.fruit_grow_time != null) cropval.cropDuration = (float)setting.fruit_grow_time;
                if (cropval.cropDuration < 1f) cropval.cropDuration = 1f;
                if (setting.fruit_amount != null) cropval.numProduced = (int)setting.fruit_amount;
                if (cropval.numProduced < 1) cropval.numProduced = 1;
                crop.Configure(cropval);

                KPrefabID prefab = plant.GetComponent<KPrefabID>();
                GeneratedBuildings.RegisterWithOverlay(OverlayScreen.HarvestableIDs, prefab.PrefabID().ToString());
                Growing growing = plant.AddOrGet<Growing>();
                growing.growthTime = cropval.cropDuration;
                if (setting.id != ForestTreeConfig.ID)  // don't harvest arbor trees directly
                    plant.AddOrGet<Harvestable>();
                plant.AddOrGet<HarvestDesignatable>();
                plant.AddOrGet<StandardCropPlant>();
            }
#endif
            #endregion
            #region irrigation
            if (setting.irrigation != null)
            {
                RemoveIrrigation(plant);

                List<PlantElementAbsorber.ConsumeInfo> irrigation = new List<PlantElementAbsorber.ConsumeInfo>(3);
                List<PlantElementAbsorber.ConsumeInfo> fertilization = new List<PlantElementAbsorber.ConsumeInfo>(3);
                foreach (KeyValuePair<string, float> entry in setting.irrigation)
                {
                    if (GameTags.LiquidElements.Contains(entry.Key))
                        irrigation.Add(new PlantElementAbsorber.ConsumeInfo(entry.Key, entry.Value / 600f));
                    else if (GameTags.SolidElements.Contains(entry.Key))
                        fertilization.Add(new PlantElementAbsorber.ConsumeInfo(entry.Key, entry.Value / 600f));
                    else
                        Debug.Log(ToDialog("Irrigation for " + setting.id + " defines bad element: " + entry.Key));
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
                        Common.Helpers.RemoveDef(plant, cropSleep);
                }
                else if (setting.illumination < 0f)
                {
                    if (illumination == null)
                        illumination = plant.AddOrGet<IlluminationVulnerable>();
                    if (cropSleep != null)
                        Common.Helpers.RemoveDef(plant, cropSleep);

                    illumination.SetPrefersDarkness(true);
                }
                else if (setting.illumination == 1f)
                {
                    if (illumination == null)
                        illumination = plant.AddOrGet<IlluminationVulnerable>();
                    if (cropSleep != null)
                        Common.Helpers.RemoveDef(plant, cropSleep);

                    illumination.SetPrefersDarkness(false);
                }
                else
                {
                    if (illumination != null)
                        UnityEngine.Object.DestroyImmediate(illumination);
                    if (cropSleep == null)
                        cropSleep = plant.AddOrGetDef<CropSleepingMonitor.Def>();
                    cropSleep.prefersDarkness = false;
#if DLC1
                    EnsureAttribute(modifiers, baseTrait, Db.Get().PlantAttributes.MinLightLux.Id, setting.illumination.Value);
#else
                    cropSleep.lightIntensityThreshold = (float)setting.illumination;
#endif
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
#if DLC1
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
#endif
    }

    public class PlantData
    {
        private int hash;
        public string id;
        public string fruitId;
        public float? fruit_grow_time;
        public int? fruit_amount;
        public Dictionary<string, float> irrigation;
        public float? illumination;
        public string[] safe_elements;
        public float[] temperatures;
        public float[] pressures;
        public int? decor_value;
        public int? decor_radius;
        public float? submerged_threshold;
        public bool? can_tinker;
        public bool? require_solid_tile;
        public float? max_age;
        public string disease;
        public int? disease_amount;
        public string input_element;
        public float? input_rate;
        public string output_element;
        public float? output_rate;

        /// <summary>
        /// Holds settings for one plant.
        /// </summary>
        /// <param name="id">Required. Defines which plant is affected.</param>
        /// <param name="fruitId">Must be valid Tag or listed in SpecialCropSettings.</param>
        /// <param name="fruit_grow_time">Time for the crop to grow in seconds.</param>
        /// <param name="fruit_amount">Number of objects or amount in kg, depends on Tag.</param>
        /// <param name="irrigation">List of irrigation needed. May be any liquid or solid element. Amount in kg per cycle</param>
        /// <param name="illumination">If equal 0 removes existing component. If less than 0 requires darkness. Otherwise number is light threshold required.</param>
        /// <param name="safe_elements">List of gas elements plant has to be in. If empty all elements are suitable.</param>
        /// <param name="temperatures">Array of temperatures in Kelvin. 1) death if lower 2) wilt if lower 3) wilt if higher 4) death if higher; entries after 4 are ignored; may have less than 4 entries</param>
        /// <param name="pressures">Array of pressures in kg. 1) death if lower 2) wilt if lower 3) wilt if higher 4) death if higher; entries after 4 are ignored; may have less than 4 entries</param>
        /// <param name="decor_value">Decor score.</param>
        /// <param name="decor_radius">Range at which the decor score is applied to.</param>
        /// <param name="submerged_threshold">If equal 0 ignores water. If less than 0 hates water. If higher than 0 needs water. Plant will wilt in bad conditions.</param>
        /// <param name="can_tinker">Whenever plant can be interacted with farming station.</param>
        /// <param name="require_solid_tile">Not sure...</param>
        /// <param name="max_age">If 0 or less, will never auto-harvest itself. Otherwise time in seconds for the plant to auto-harvest and plays bristled animation when at 50%+.</param>
        /// <param name="disease">Type of disease the plant spreads around it. May be: FoodPoisoning, SlimeLung, PollenGerms, or ZombieSpores.</param>
        /// <param name="disease_amount">How much disease is spread around it.</param>
        /// <param name="input_element">Type of gas or liquid plant absorbs from environment.</param>
        /// <param name="input_rate">Amount absorbed per second.</param>
        /// <param name="output_element">Type of gas or liquid plant expels per second.</param>
        /// <param name="output_rate">Amount expelled per second.</param>
        public PlantData(string id, string fruitId = null, float? fruit_grow_time = null, int? fruit_amount = null, Dictionary<string, float> irrigation = null, float? illumination = null, string[] safe_elements = null, float[] temperatures = null, float[] pressures = null, int? decor_value = null, int? decor_radius = null, float? submerged_threshold = null, bool? can_tinker = null, bool? require_solid_tile = null, float? max_age = null, string disease = null, int? disease_amount = null, string input_element = null, float? input_rate = null, string output_element = null, float? output_rate = null)
        {
            this.id = id;
            this.fruitId = fruitId;
            this.fruit_grow_time = fruit_grow_time;
            this.fruit_amount = fruit_amount;
            this.irrigation = irrigation;
            this.illumination = illumination;
            this.safe_elements = safe_elements;
            this.temperatures = temperatures;
            this.pressures = pressures;
            this.decor_value = decor_value;
            this.decor_radius = decor_radius;
            this.submerged_threshold = submerged_threshold;
            this.can_tinker = can_tinker;
            this.require_solid_tile = require_solid_tile;
            this.max_age = max_age;
            this.disease = disease;
            this.disease_amount = disease_amount;
            this.input_element = input_element;
            this.input_rate = input_rate;
            this.output_element = output_element;
            this.output_rate = output_rate;
            //TODO: validate settings
        }

        public PlantData()
        { }

        public override bool Equals(object obj)
        {
            return this.GetHashCode() == (obj as PlantData)?.GetHashCode();
        }

        public override int GetHashCode()
        {
            if (hash == 0)
                hash = Hash.SDBMLower(this.id);
            return hash;
        }

        public override string ToString()
        {
            return base.ToString();
        }

    }

}
