using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common;
using HarmonyLib;

namespace CustomizeElements
{
    [HarmonyPatch(typeof(ElementLoader), "FinaliseElementsTable")]
    public class ElementLoader_FinaliseElementsTable
    {
        public static void Prefix()
        {
            foreach (var element in ElementLoader.elements)
            {
                var setting = CustomizeElements.CustomizeElementsState.StateManager.State.Elements.FirstOrDefault(f => f.Id == element.tag);
                if (setting == null)
                    continue;

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
                    element.lowTempTransitionTarget = setting.lowTempTransitionTarget.ToSimHash();

                if (setting.lowTempTransitionOreID != null)
                    element.lowTempTransitionOreID = setting.lowTempTransitionOreID.ToSimHash();

                if (setting.lowTempTransitionOreMassConversion != null)
                    element.lowTempTransitionOreMassConversion = setting.lowTempTransitionOreMassConversion.Value;

                if (setting.highTemp != null)
                    element.highTemp = setting.highTemp.Value;

                if (setting.highTempTransitionTarget != null)
                    element.highTempTransitionTarget = setting.highTempTransitionTarget.ToSimHash();

                if (setting.highTempTransitionOreID != null)
                    element.highTempTransitionOreID = setting.highTempTransitionOreID.ToSimHash();

                if (setting.highTempTransitionOreMassConversion != null)
                    element.highTempTransitionOreMassConversion = setting.highTempTransitionOreMassConversion.Value;

                if (setting.sublimateId != null)
                    element.sublimateId = setting.sublimateId.ToSimHash();

                if (setting.convertId != null)
                    element.convertId = setting.convertId.ToSimHash();

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

                if (setting.toxicity != null)
                    element.toxicity = setting.toxicity.Value;

                if (setting.buildMenuSort != null)
                    element.buildMenuSort = setting.buildMenuSort.Value;

                if (setting.materialCategory != null)
                    element.materialCategory = setting.materialCategory;

                if (setting.oreTags != null)
                    CreateOreTags(null, element.materialCategory, (Tag)element.state.ToString(), setting.oreTags.ToArray());
            }
        }

        public static FastInvokeHandler CreateOreTags = MethodInvoker.GetHandler(AccessTools.Method("ElementLoader:CreateOreTags"));
    }
}