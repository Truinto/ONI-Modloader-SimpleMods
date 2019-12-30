using System;
using System.Linq;
using Harmony;
using System.Collections.Generic;
using static BootDialog.PostBootDialog;

namespace CustomizeGeyser
{
    public class GeyserInfo
    {
        /// <summary>
        /// Reference to the geyser configs.
        /// </summary>
        public static List<GeyserGenericConfig.GeyserPrefabParams> config;
    }


    [HarmonyPatch(typeof(GeyserGenericConfig), "GenerateConfigs")]
    internal class GeyserGenericConfig_GenerateConfigs
    {
        internal static bool Prepare()
        {
            return CustomizeGeyserState.StateManager.State.Enabled;
        }

        public static readonly string[] GeyserKAnims = { "geyser_gas_steam_kanim", "geyser_gas_steam_hot_kanim", "geyser_liquid_water_hot_kanim", "geyser_liquid_water_slush_kanim", "geyser_liquid_water_filthy_kanim", "geyser_liquid_salt_water_kanim", "geyser_molten_volcano_small_kanim", "geyser_molten_volcano_big_kanim", "geyser_liquid_co2_kanim", "geyser_gas_co2_hot_kanim", "geyser_gas_hydrogen_hot_kanim", "geyser_gas_po2_hot_kanim", "geyser_gas_po2_slimy_kanim", "geyser_gas_chlorine_kanim", "geyser_gas_methane_kanim", "geyser_molten_copper_kanim", "geyser_molten_iron_kanim", "geyser_molten_gold_kanim", "geyser_liquid_oil_kanim" };

        internal static void Postfix(ref List<GeyserGenericConfig.GeyserPrefabParams> __result)
        {
            GeyserInfo.config = __result;

            for (int j = 0; j < CustomizeGeyserState.StateManager.State.Geysers.Count; j++)
            {
                CustomizeGeyserState.GeyserStruct modifier = CustomizeGeyserState.StateManager.State.Geysers[j];
                if (modifier.id == null) continue;

                Debug.Log("[GeyserModifier] Processing " + modifier.id + " ...");
                
                #region Error checks
                {
                    if (modifier.anim != null && !GeyserKAnims.Any(s => s == modifier.anim))
                    {
                        Debug.LogWarning(ToDialog("Warning: Geyser " + modifier.id + " has non standard kAnim type " + modifier.anim));
                        modifier.anim = null;    // TODO: find a way to validate custom kAnims
                    }
                    if (modifier.width != null && modifier.width < 1 || modifier.width > 10)
                    {
                        Debug.LogWarning(ToDialog("Warning: Geyser " + modifier.id + " has bad width"));
                        modifier.width = null;
                    }
                    if (modifier.height != null && modifier.height < 1 || modifier.height > 10)
                    {
                        Debug.LogWarning(ToDialog("Warning: Geyser " + modifier.id + " has bad height"));
                        modifier.height = null;
                    }
                    if (modifier.element != null && ElementLoader.FindElementByName(modifier.element) == null)
                    {
                        Debug.LogWarning(ToDialog("Warning: Geyser " + modifier.id + " element " + modifier.element + " does not exist"));
                        modifier.element = null;
                    }
                    if (modifier.temperature != null && modifier.temperature < 1f || modifier.temperature > 8000f)
                    {
                        Debug.LogWarning(ToDialog("Warning: Geyser " + modifier.id + " has bad temperature"));
                        modifier.temperature = null;
                    }
                    if (modifier.minRatePerCycle != null && modifier.minRatePerCycle < 0f)
                    {
                        Debug.LogWarning(ToDialog("Warning: Geyser " + modifier.id + " has bad minRatePerCycle"));
                        modifier.minRatePerCycle = null;
                    }
                    // maxRatePerCycle later check for min
                    if (modifier.maxPressure != null && modifier.maxPressure < 0f)
                    {
                        Debug.LogWarning(ToDialog("Warning: Geyser " + modifier.id + " has bad maxPressure"));
                        modifier.maxPressure = null;
                    }
                    if (modifier.minIterationLength != null && modifier.minIterationLength < 0f)
                    {
                        Debug.LogWarning(ToDialog("Warning: Geyser " + modifier.id + " has bad minIterationLength"));
                        modifier.minIterationLength = null;
                    }
                    // maxIterationLength later check for min
                    if (modifier.minIterationPercent != null && modifier.minIterationPercent < 0f || modifier.minIterationPercent > 1f)
                    {
                        Debug.LogWarning(ToDialog("Warning: Geyser " + modifier.id + " has bad minIterationPercent"));
                        modifier.minIterationPercent = null;
                    }
                    if (modifier.maxIterationPercent != null && modifier.maxIterationPercent > 1f) // maxIterationPercent later check for min
                    {
                        Debug.LogWarning(ToDialog("Warning: Geyser " + modifier.id + " has bad maxIterationPercent"));
                        modifier.maxIterationPercent = null;
                    }
                    if (modifier.minYearLength != null && modifier.minYearLength < 10f)
                    {
                        Debug.LogWarning(ToDialog("Warning: Geyser " + modifier.id + " has bad minYearLength"));
                        modifier.minYearLength = null;
                    }
                    // maxYearLength later check for min
                    if (modifier.minYearPercent != null && modifier.minYearPercent < 0f || modifier.minYearPercent > 1f)
                    {
                        Debug.LogWarning(ToDialog("Warning: Geyser " + modifier.id + " has bad minYearPercent"));
                        modifier.minYearPercent = null;
                    }
                    // maxYearPercent later check for min
                }
                #endregion

                int i = __result.FindIndex(x => x.geyserType.id == modifier.id);

                #region existing geyser
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
                        __result[i].geyserType.maxRatePerCycle = Math.Max((float)modifier.maxRatePerCycle, __result[i].geyserType.minRatePerCycle);
                    if (modifier.maxPressure != null)
                        __result[i].geyserType.maxPressure = (float)modifier.maxPressure;
                    if (modifier.minIterationLength != null)
                        __result[i].geyserType.minIterationLength = (float)modifier.minIterationLength;
                    if (modifier.maxIterationLength != null)
                        __result[i].geyserType.maxIterationLength = Math.Max((float)modifier.maxIterationLength, __result[i].geyserType.minIterationLength);
                    if (modifier.minIterationPercent != null)
                        __result[i].geyserType.minIterationPercent = (float)modifier.minIterationPercent;
                    if (modifier.maxIterationPercent != null)
                        __result[i].geyserType.maxIterationPercent = Math.Max((float)modifier.maxIterationPercent, __result[i].geyserType.minIterationPercent);
                    if (modifier.minYearLength != null)
                        __result[i].geyserType.minYearLength = (float)modifier.minYearLength;
                    if (modifier.maxYearLength != null)
                        __result[i].geyserType.maxYearLength = Math.Max((float)modifier.maxYearLength, __result[i].geyserType.minYearLength);
                    if (modifier.minYearPercent != null)
                        __result[i].geyserType.minYearPercent = (float)modifier.minYearPercent;
                    if (modifier.maxYearPercent != null)
                        __result[i].geyserType.maxYearPercent = Math.Max((float)modifier.maxYearPercent, __result[i].geyserType.minYearPercent);

                    if (modifier.Name != null)
                    {
                        Strings.Add("STRINGS.CREATURES.SPECIES.GEYSER." + modifier.id.ToUpper() + ".NAME", modifier.Name);
                    }
                    if (modifier.Description != null)
                    {
                        Strings.Add("STRINGS.CREATURES.SPECIES.GEYSER." + modifier.id.ToUpper() + ".DESC", modifier.Description);
                    }

                    if (modifier.Disease != null || modifier.DiseaseCount != null)
                    {
                        byte diseaseIndex;
                        if (modifier.Disease == null)
                            diseaseIndex = __result[i].geyserType.diseaseInfo.idx;
                        else
                            diseaseIndex = Db.Get().Diseases.GetIndex((HashedString)modifier.Disease);

                        if (modifier.DiseaseCount == null)
                            modifier.DiseaseCount = __result[i].geyserType.diseaseInfo.count;

                        if (diseaseIndex == byte.MaxValue || modifier.DiseaseCount <= 0)
                            __result[i].geyserType.diseaseInfo = Klei.SimUtil.DiseaseInfo.Invalid;
                        else
                            __result[i].geyserType.diseaseInfo = new Klei.SimUtil.DiseaseInfo() { idx = diseaseIndex, count = (int)modifier.DiseaseCount };
                    }

                    Debug.Log("[GeyserModifier] Changed geyser with id: " + modifier.id);
                }
                #endregion
                #region new geyser
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
                        else if (element.name.ToLower().Contains("molten"))
                            modifier.anim = "geyser_molten_iron_kanim";
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

                    if (modifier.temperature == null)
                        modifier.temperature = 373.15f;

                    if (modifier.minRatePerCycle == null)
                        modifier.minRatePerCycle = 3000f;

                    if (modifier.maxRatePerCycle == null || modifier.maxRatePerCycle < modifier.minRatePerCycle)
                        modifier.maxRatePerCycle = modifier.minRatePerCycle;

                    if (modifier.maxPressure == null)
                        modifier.maxPressure = 500f;

                    if (modifier.minIterationLength == null)
                        modifier.minIterationLength = 600f;

                    if (modifier.maxIterationLength == null || modifier.maxIterationLength < modifier.minIterationLength)
                        modifier.maxIterationLength = modifier.minIterationLength;

                    if (modifier.minIterationPercent == null)
                        modifier.minIterationPercent = 0.5f;

                    if (modifier.maxIterationPercent == null || modifier.maxIterationPercent < modifier.minIterationPercent)
                        modifier.maxIterationPercent = modifier.minIterationPercent;

                    if (modifier.minYearLength == null)
                        modifier.minYearLength = 75000f;

                    if (modifier.maxYearLength == null || modifier.maxYearLength < modifier.minYearLength)
                        modifier.maxYearLength = modifier.minYearLength;

                    if (modifier.minYearPercent == null)
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
                        byte diseaseIndex = Db.Get().Diseases.GetIndex((HashedString)modifier.Disease);
                        if (diseaseIndex == byte.MaxValue || modifier.DiseaseCount <= 0)
                            diseaseInfo = Klei.SimUtil.DiseaseInfo.Invalid;
                        else
                            diseaseInfo = new Klei.SimUtil.DiseaseInfo() { idx = diseaseIndex, count = (int)modifier.DiseaseCount };
                    }

                    __result.Add(
                        new GeyserGenericConfig.GeyserPrefabParams(
                            modifier.anim,
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
                #endregion
            }
        }
    }

}