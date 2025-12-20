using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common;
using HarmonyLib;

namespace CustomizeElements
{
    [HarmonyPatch(typeof(ElementLoader), nameof(ElementLoader.FinaliseElementsTable))]
    public static class ElementLoader_FinaliseElementsTable
    {
        public static void Prefix()
        {
            foreach (var element in ElementLoader.elements)
            {
                var setting = CustomizeElementsState.StateManager.State.Elements.FirstOrDefault(f => f.Id == element.tag);
                if (setting == null)
                    continue;

                Helpers.Print("Applying changes to " + element.name);

                if (setting.specificHeatCapacity != null)
                    element.specificHeatCapacity = setting.specificHeatCapacity.Value;

                if (setting.thermalConductivity != null)
                    element.thermalConductivity = setting.thermalConductivity.Value;

                if (setting.molarMass != null)
                    element.molarMass = setting.molarMass.Value;

                if (setting.strength != null)
                    element.strength = setting.strength.Value;

                if (setting.flow != null)
                    element.flow = setting.flow.Value;

                if (setting.maxMass != null)
                    element.maxMass = setting.maxMass.Value;

                if (setting.maxCompression != null)
                    element.maxCompression = setting.maxCompression.Value;

                if (setting.viscosity != null)
                    element.viscosity = setting.viscosity.Value;

                if (setting.minHorizontalFlow != null)
                    element.minHorizontalFlow = setting.minHorizontalFlow.Value;

                if (setting.minVerticalFlow != null)
                    element.minVerticalFlow = setting.minVerticalFlow.Value;

                if (setting.solidSurfaceAreaMultiplier != null)
                    element.solidSurfaceAreaMultiplier = setting.solidSurfaceAreaMultiplier.Value;

                if (setting.liquidSurfaceAreaMultiplier != null)
                    element.liquidSurfaceAreaMultiplier = setting.liquidSurfaceAreaMultiplier.Value;

                if (setting.gasSurfaceAreaMultiplier != null)
                    element.gasSurfaceAreaMultiplier = setting.gasSurfaceAreaMultiplier.Value;

                if (setting.state != null)
                    element.state = setting.state.Value;

                if (setting.hardness != null)
                    element.hardness = setting.hardness.Value;

                if (setting.lowTemp != null)
                    element.lowTemp = setting.lowTemp.Value;

                if (setting.lowTempTransitionTarget != null)
                    element.lowTempTransitionTarget = setting.lowTempTransitionTarget.ToSimHash(0);

                if (setting.lowTempTransitionOreID != null)
                    element.lowTempTransitionOreID = setting.lowTempTransitionOreID.ToSimHash(0);

                if (setting.lowTempTransitionOreMassConversion != null)
                    element.lowTempTransitionOreMassConversion = setting.lowTempTransitionOreMassConversion.Value;

                if (setting.highTemp != null)
                    element.highTemp = setting.highTemp.Value;

                if (setting.highTempTransitionTarget != null)
                    element.highTempTransitionTarget = setting.highTempTransitionTarget.ToSimHash(0);

                if (setting.highTempTransitionOreID != null)
                    element.highTempTransitionOreID = setting.highTempTransitionOreID.ToSimHash(0);

                if (setting.highTempTransitionOreMassConversion != null)
                    element.highTempTransitionOreMassConversion = setting.highTempTransitionOreMassConversion.Value;

                if (setting.sublimateId != null)
                    element.sublimateId = setting.sublimateId.ToSimHash(0);

                if (setting.convertId != null)
                    element.convertId = setting.convertId.ToSimHash(0);

                if (setting.sublimateFX != null)
                    element.sublimateFX = (SpawnFXHashes)Hash.SDBMLower(setting.sublimateFX);

                if (setting.sublimateRate != null)
                    element.sublimateRate = setting.sublimateRate.Value;

                if (setting.sublimateEfficiency != null)
                    element.sublimateEfficiency = setting.sublimateEfficiency.Value;

                if (setting.sublimateProbability != null)
                    element.sublimateProbability = setting.sublimateProbability.Value;

                if (setting.offGasPercentage != null)
                    element.offGasPercentage = setting.offGasPercentage.Value;

                if (setting.lightAbsorptionFactor != null)
                    element.lightAbsorptionFactor = setting.lightAbsorptionFactor.Value;

                if (setting.radiationAbsorptionFactor != null)
                    element.radiationAbsorptionFactor = setting.radiationAbsorptionFactor.Value;

                if (setting.radiationPer1000Mass != null)
                    element.radiationPer1000Mass = setting.radiationPer1000Mass.Value;

                if (setting.default_temperature != null)
                    element.defaultValues.temperature = setting.default_temperature.Value;

                if (setting.default_mass != null)
                    element.defaultValues.mass = setting.default_mass.Value;

                if (setting.default_pressure != null)
                    element.defaultValues.pressure = setting.default_pressure.Value;

                if (setting.refinedMetalTarget != null)
                    element.refinedMetalTarget = setting.refinedMetalTarget.ToSimHash(0);

                if (setting.toxicity != null)
                    element.toxicity = setting.toxicity.Value;

                if (setting.buildMenuSort != null)
                    element.buildMenuSort = setting.buildMenuSort.Value;

                if (setting.materialCategory != null)
                    element.materialCategory = setting.materialCategory;

                if (setting.disabled != null)
                    element.disabled = setting.disabled.Value;

                if (setting.oreTags != null)
                {
                    var list = new List<Tag>();
                    if (element.materialCategory.IsValid)
                        list.Add(element.materialCategory);
                    list.Add(element.state.ToString());
                    foreach (var oreTag in setting.oreTags)
                    {
                        if (oreTag == null || oreTag.Length == 0)
                            continue;

                        Tag tag2 = oreTag;
                        if (!TagManager.ProperNames.ContainsKey(tag2))
                        {
                            TagManager.ProperNames[tag2] = oreTag;
                            TagManager.ProperNamesNoLinks[tag2] = oreTag;
                        }
                        list.Add(oreTag);
                    }
                    element.oreTags = list.ToArray();
                    //element.oreTags = ElementLoader.CreateOreTags(element.materialCategory, element.state.ToString().ToTag(), setting.oreTags.ToArray());
                }

                if (setting.attributeModifiers != null)
                {
                    foreach (var attribute in setting.attributeModifiers)
                    {
                        attribute.Description ??= element.name;
                        element.attributeModifiers.Add(attribute);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Assets), nameof(Assets.AddPrefab))]
    public static class Assets_AddPrefab
    {
        public static void Prefix(KPrefabID prefab)
        {
            var go = prefab.gameObject;
            if (go?.GetComponent<ElementChunk>() == null)
                return;
            var setting = CustomizeElementsState.StateManager.State.Elements.FirstOrDefault(f => f.Id == prefab.PrefabTag);
            if (setting == null)
                return;

            var sublimate = go.GetComponent<Sublimates>();
            if (sublimate == null)
            {
                if (string.IsNullOrEmpty(setting.sublimateId))
                    return;

                // create new if missing (with placeholders)
                sublimate = go.AddComponent<Sublimates>();
                sublimate.info = new Sublimates.Info(0.00002f, 0.05f, 1.8f, 0.5f, SimHashes.Vacuum);
            }
            else if (setting.sublimateId == "" || setting.sublimateRate == 0)
            {
                // remove if set to nil
                go.RemoveComponent<Sublimates>();
                return;
            }

            // overwrite values
            if (setting.sublimateFX != null && Enum.TryParse<SpawnFXHashes>(setting.sublimateFX, out var sfx))
                sublimate.spawnFXHash = sfx;
            var info = sublimate.info;
            info.sublimatedElement = setting.sublimateId.ToSimHash(info.sublimatedElement);
            info.sublimationRate = setting.sublimateRate ?? info.sublimationRate;
            info.minSublimationAmount = setting.sublimate_min ?? info.minSublimationAmount;
            info.maxDestinationMass = setting.sublimate_overpressure ?? info.maxDestinationMass;
            info.massPower = setting.sublimate_pwr ?? info.massPower;
            sublimate.info = info;
        }
    }
}
