using System;
using Harmony;
using System.Collections.Generic;
using static BootDialog.PostBootDialog;

namespace CustomizeGeyser
{
    [HarmonyPatch(typeof(GeyserGenericConfig), "GenerateConfigs")]
    internal class GeyserGenericConfig_GenerateConfigs
    {
        internal static bool Prepare()
        {
            return CustomizeGeyserState.StateManager.State.Enabled;
        }

        internal static void Postfix(ref List<GeyserGenericConfig.GeyserPrefabParams> __result)
        {
            for (int j = 0; j < CustomizeGeyserState.StateManager.State.Geysers.Count; j++)
            {
                CustomizeGeyserState.GeyserStruct modifier = CustomizeGeyserState.StateManager.State.Geysers[j];

                if (modifier.id == null) continue;

                Debug.Log("[GeyserModifier] Processing " + modifier.id + " ...");

                int i = __result.FindIndex(x => x.geyserType.id == modifier.id);

                if (i >= 0)  //edit existing geyser
                {
                    if (modifier.anim != null || modifier.width != null || modifier.height != null)
                    {
                        GeyserGenericConfig.GeyserPrefabParams copy = __result[i];
                        if (modifier.anim != null)
                            copy.anim = modifier.anim;
                        if (modifier.width != null)
                            copy.width = (int)modifier.width;
                        if (modifier.height != null)
                            copy.height = (int)modifier.height;

                        __result[i] = copy;
                    }

                    if (modifier.element != null)
                        __result[i].geyserType.element = (SimHashes)Hash.SDBMLower(modifier.element);
                    if (modifier.temperature != null)
                        __result[i].geyserType.temperature = (float)modifier.temperature;
                    if (modifier.minRatePerCycle != null)
                        __result[i].geyserType.minRatePerCycle = (float)modifier.minRatePerCycle;
                    if (modifier.maxRatePerCycle != null)
                        __result[i].geyserType.maxRatePerCycle = (float)modifier.maxRatePerCycle;
                    if (modifier.maxPressure != null)
                        __result[i].geyserType.maxPressure = (float)modifier.maxPressure;
                    if (modifier.minIterationLength != null)
                        __result[i].geyserType.minIterationLength = (float)modifier.minIterationLength;
                    if (modifier.maxIterationLength != null)
                        __result[i].geyserType.maxIterationLength = (float)modifier.maxIterationLength;
                    if (modifier.minIterationPercent != null)
                        __result[i].geyserType.minIterationPercent = (float)modifier.minIterationPercent;
                    if (modifier.maxIterationPercent != null)
                        __result[i].geyserType.maxIterationPercent = (float)modifier.maxIterationPercent;
                    if (modifier.minYearLength != null)
                        __result[i].geyserType.minYearLength = (float)modifier.minYearLength;
                    if (modifier.maxYearLength != null)
                        __result[i].geyserType.maxYearLength = (float)modifier.maxYearLength;
                    if (modifier.minYearPercent != null)
                        __result[i].geyserType.minYearPercent = (float)modifier.minYearPercent;
                    if (modifier.maxYearPercent != null)
                        __result[i].geyserType.maxYearPercent = (float)modifier.maxYearPercent;
					
					if (modifier.Disease != null || modifier.DiseaseCount != null)
					{						
						byte diseaseIndex;
						if (modifier.Disease == null)
							diseaseIndex = __result[i].geyserType.diseaseInfo.idx;
						else
							diseaseIndex = Db.Get().Diseases.GetIndex((HashedString) modifier.Disease);
						
						if (modifier.DiseaseCount == null)
							modifier.DiseaseCount = __result[i].geyserType.diseaseInfo.count;
						
						if (diseaseIndex == byte.MaxValue || modifier.DiseaseCount == 0)
							__result[i].geyserType.diseaseInfo = Klei.SimUtil.DiseaseInfo.Invalid;
						else
							__result[i].geyserType.diseaseInfo = new Klei.SimUtil.DiseaseInfo() { idx = diseaseIndex, count = (int) modifier.DiseaseCount };
					}
						

                    Debug.Log("[GeyserModifier] Changed geyser with id: " + modifier.id);
                }
                else    //make new geyser
                {
                    if (modifier.element == null)
                    {
                        Debug.LogWarning(ToDialog("[GeyserModifier] Cannot add geyser with no element: " + modifier.id));
                        continue;
                    }

                    if (ElementLoader.FindElementByName(modifier.element) == null)
                    {
                        Debug.LogWarning(ToDialog("[GeyserModifier] Could not add geyser " + modifier.id + " because element does not exist: " + modifier.element));
                        continue;
                    }

                    if (modifier.anim == null)
                    {
                        Element element = ElementLoader.FindElementByName(modifier.element);
                        if (element.IsGas)
                            modifier.anim = "geyser_gas_steam_kanim";
                        else
                            modifier.anim = "geyser_liquid_water_slush_kanim";
                    }

                    if (modifier.width == null || modifier.height == null)
                    {
                        if (modifier.anim.Contains("_gas_"))
                        {
                            modifier.width = 2;
                            modifier.height = 4;
                        }
                        else if (modifier.anim.Contains("_molten_"))
                        {
                            modifier.width = 3;
                            modifier.height = 3;
                        }
                        else
                        {
                            modifier.width = 4;
                            modifier.height = 2;
                        }
                    }

                    if (modifier.temperature == null || modifier.temperature <= 0f)
                        modifier.temperature = 303.15f;

                    if (modifier.minRatePerCycle == null || modifier.minRatePerCycle <= 0f)
                        modifier.minRatePerCycle = 3000f;

                    if (modifier.maxRatePerCycle == null || modifier.maxRatePerCycle < modifier.minRatePerCycle)
                        modifier.maxRatePerCycle = modifier.minRatePerCycle;

                    if (modifier.maxPressure == null || modifier.maxPressure <= 0f)
                        modifier.maxPressure = 500f;

                    if (modifier.minIterationLength == null || modifier.minIterationLength <= 0f)
                        modifier.minIterationLength = 600f;

                    if (modifier.maxIterationLength == null || modifier.maxIterationLength < modifier.minIterationLength)
                        modifier.maxIterationLength = modifier.minIterationLength;

                    if (modifier.minIterationPercent == null || modifier.minIterationPercent < 0f || modifier.minIterationPercent > 1f)
                        modifier.minIterationPercent = 0.5f;

                    if (modifier.maxIterationPercent == null || modifier.maxIterationPercent < modifier.minIterationPercent || modifier.maxIterationPercent > 1f)
                        modifier.maxIterationPercent = modifier.minIterationPercent;

                    if (modifier.minYearLength == null || modifier.minYearLength <= 0f)
                        modifier.minYearLength = 75000f;

                    if (modifier.maxYearLength == null || modifier.maxYearLength < modifier.minYearLength)
                        modifier.maxYearLength = modifier.minYearLength;

                    if (modifier.minYearPercent == null || modifier.minYearPercent < 0f || modifier.minYearPercent > 1f)
                        modifier.minYearPercent = 0.6f;

                    if (modifier.maxYearPercent == null || modifier.maxYearPercent < modifier.minYearPercent || modifier.maxYearPercent > 1f)
                        modifier.maxYearPercent = modifier.minYearPercent;
					
                    if (modifier.Name != null)
                        Strings.Add("STRINGS.CREATURES.SPECIES.GEYSER." + modifier.id.ToUpper() + ".NAME", modifier.Name);
                    if (modifier.Description != null)
                        Strings.Add("STRINGS.CREATURES.SPECIES.GEYSER." + modifier.id.ToUpper() + ".DESC", modifier.Description);

                    Klei.SimUtil.DiseaseInfo diseaseInfo;
					if (modifier.Disease == null || modifier.DiseaseCount == null)
						diseaseInfo = Klei.SimUtil.DiseaseInfo.Invalid;
					else
					{
						byte diseaseIndex = Db.Get().Diseases.GetIndex((HashedString) modifier.Disease);
						if (diseaseIndex == byte.MaxValue || modifier.DiseaseCount == 0)
							diseaseInfo = Klei.SimUtil.DiseaseInfo.Invalid;
						else
							diseaseInfo = new Klei.SimUtil.DiseaseInfo() { idx = diseaseIndex, count = (int) modifier.DiseaseCount };
					}
					
                    __result.Add(
                        new GeyserGenericConfig.GeyserPrefabParams(
                            modifier.anim, //"geyser_liquid_oil_kanim",
                            (int)modifier.width,
                            (int)modifier.height,
                            new GeyserConfigurator.GeyserType(
                                modifier.id,
                                (SimHashes)Hash.SDBMLower(modifier.element),
                                (float)modifier.temperature,
                                (float)modifier.minRatePerCycle,
                                (float)modifier.maxRatePerCycle,
                                (float)modifier.maxPressure,
                                (float)modifier.minIterationLength,
                                (float)modifier.maxIterationLength,
                                (float)modifier.minIterationPercent,
                                (float)modifier.maxIterationPercent,
                                (float)modifier.minYearLength,
                                (float)modifier.maxYearLength,
                                (float)modifier.minYearPercent,
                                (float)modifier.maxYearPercent).AddDisease(diseaseInfo)
						));

                    Debug.Log("[GeyserModifier] Added geyser " + modifier.id + " : " + modifier.element);

                }
            }
        }

        //internal static List<GeyserConfigurator.GeyserType> geyserTypes = (List <GeyserConfigurator.GeyserType>)AccessTools.Field(typeof(GeyserConfigurator), "geyserTypes").GetValue(null);
    }

}