using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System;
using Common;
using Klei;

namespace CustomizeBuildings
{
    public class Miscellaneous
    {
        public static void OnLoad()
        {
            #region Tuning

            setTuning(nameof(CustomizeBuildingsState.TuningAtmosuitDecay), ref TUNING.EQUIPMENT.SUITS.ATMOSUIT_DECAY);
            setTuning(nameof(CustomizeBuildingsState.TuningAtmosuitAthletics), ref TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS);
            setTuning(nameof(CustomizeBuildingsState.TuningOxygenMaskDecay), ref TUNING.EQUIPMENT.SUITS.OXYGEN_MASK_DECAY);
            setTuning(nameof(CustomizeBuildingsState.TuningLeadsuitRadiationShielding), ref TUNING.EQUIPMENT.SUITS.LEADSUIT_RADIATION_SHIELDING);
            setTuning(nameof(CustomizeBuildingsState.TuningLeadsuitAthletics), ref TUNING.EQUIPMENT.SUITS.LEADSUIT_ATHLETICS);
            setTuning(nameof(CustomizeBuildingsState.TuningLeadsuitStrength), ref TUNING.EQUIPMENT.SUITS.LEADSUIT_STRENGTH);
            setTuning(nameof(CustomizeBuildingsState.TuningLeadsuitInsulation), ref TUNING.EQUIPMENT.SUITS.LEADSUIT_INSULATION);
            setTuning(nameof(CustomizeBuildingsState.TuningLeadsuitThermalConductivityBarrier), ref TUNING.EQUIPMENT.SUITS.LEADSUIT_THERMAL_CONDUCTIVITY_BARRIER);
            setTuning(nameof(CustomizeBuildingsState.TuningAtmosuitScalding), ref TUNING.EQUIPMENT.SUITS.ATMOSUIT_SCALDING);
            setTuning(nameof(CustomizeBuildingsState.TuningAtmosuitInsulation), ref TUNING.EQUIPMENT.SUITS.ATMOSUIT_INSULATION);
            setTuning(nameof(CustomizeBuildingsState.TuningAtmosuitThermalConductivityBarrier), ref TUNING.EQUIPMENT.SUITS.ATMOSUIT_THERMAL_CONDUCTIVITY_BARRIER);
            setTuning(nameof(CustomizeBuildingsState.TuningMissionDurationScale), ref TUNING.ROCKETRY.MISSION_DURATION_SCALE);
            setTuning(nameof(CustomizeBuildingsState.TuningMassPenaltyExponent), ref TUNING.ROCKETRY.MASS_PENALTY_EXPONENT);
            setTuning(nameof(CustomizeBuildingsState.TuningMassPenaltyDivisor), ref TUNING.ROCKETRY.MASS_PENALTY_DIVISOR);
            setTuning(nameof(CustomizeBuildingsState.TuningResearchEvergreen), ref TUNING.ROCKETRY.DESTINATION_RESEARCH.EVERGREEN);
            setTuning(nameof(CustomizeBuildingsState.TuningResearchBasic), ref TUNING.ROCKETRY.DESTINATION_RESEARCH.BASIC);
            setTuning(nameof(CustomizeBuildingsState.TuningAnalysisDiscovered), ref TUNING.ROCKETRY.DESTINATION_ANALYSIS.DISCOVERED);
            setTuning(nameof(CustomizeBuildingsState.TuningAnalysisComplete), ref TUNING.ROCKETRY.DESTINATION_ANALYSIS.COMPLETE);
            setTuning(nameof(CustomizeBuildingsState.TuningAnalysisDefaultCyclesPerDiscovery), ref TUNING.ROCKETRY.DESTINATION_ANALYSIS.DEFAULT_CYCLES_PER_DISCOVERY);
            setTuning(nameof(CustomizeBuildingsState.TuningThrustCostsLow), ref TUNING.ROCKETRY.DESTINATION_THRUST_COSTS.LOW);
            setTuning(nameof(CustomizeBuildingsState.TuningThrustCostsMid), ref TUNING.ROCKETRY.DESTINATION_THRUST_COSTS.MID);
            setTuning(nameof(CustomizeBuildingsState.TuningThrustCostsHigh), ref TUNING.ROCKETRY.DESTINATION_THRUST_COSTS.HIGH);
            setTuning(nameof(CustomizeBuildingsState.TuningThrustCostsVeryHigh), ref TUNING.ROCKETRY.DESTINATION_THRUST_COSTS.VERY_HIGH);
            setTuning(nameof(CustomizeBuildingsState.TuningClusterFowPointsToReveal), ref TUNING.ROCKETRY.CLUSTER_FOW.POINTS_TO_REVEAL);
            setTuning(nameof(CustomizeBuildingsState.TuningClusterFowDefaultCyclesPerReveal), ref TUNING.ROCKETRY.CLUSTER_FOW.DEFAULT_CYCLES_PER_REVEAL);
            setTuning(nameof(CustomizeBuildingsState.TuningEngineEfficiencyWeak), ref TUNING.ROCKETRY.ENGINE_EFFICIENCY.WEAK);   //CO2; Steam
            setTuning(nameof(CustomizeBuildingsState.TuningEngineEfficiencyMedium), ref TUNING.ROCKETRY.ENGINE_EFFICIENCY.MEDIUM); //Kerosene
            setTuning(nameof(CustomizeBuildingsState.TuningEngineEfficiencyStrong), ref TUNING.ROCKETRY.ENGINE_EFFICIENCY.STRONG); //Hydrogen; Sugar
            setTuning(nameof(CustomizeBuildingsState.TuningEngineEfficiencyBooster), ref TUNING.ROCKETRY.ENGINE_EFFICIENCY.BOOSTER);
            setTuning(nameof(CustomizeBuildingsState.TuningOxidizerEfficiencyVeryLow), ref TUNING.ROCKETRY.DLC1_OXIDIZER_EFFICIENCY.VERY_LOW);  //Fertilizer
            setTuning(nameof(CustomizeBuildingsState.TuningOxidizerEfficiencyLow), ref TUNING.ROCKETRY.DLC1_OXIDIZER_EFFICIENCY.LOW);   //OxyRock
            setTuning(nameof(CustomizeBuildingsState.TuningOxidizerEfficiencyHigh), ref TUNING.ROCKETRY.DLC1_OXIDIZER_EFFICIENCY.HIGH);  //LiquidOxygen
            setTuning(nameof(CustomizeBuildingsState.TuningCargoContainerMassStaticMass), ref TUNING.ROCKETRY.CARGO_CONTAINER_MASS.STATIC_MASS);
            setTuning(nameof(CustomizeBuildingsState.TuningCargoContainerMassPayloadMass), ref TUNING.ROCKETRY.CARGO_CONTAINER_MASS.PAYLOAD_MASS);
            setTuning(nameof(CustomizeBuildingsState.TuningBurdenInsignificant), ref TUNING.ROCKETRY.BURDEN.INSIGNIFICANT);   //Unconstructed
            setTuning(nameof(CustomizeBuildingsState.TuningBurdenMinor), ref TUNING.ROCKETRY.BURDEN.MINOR);   //GasCargo; Nosecone; Oxidizer-Small
            setTuning(nameof(CustomizeBuildingsState.TuningBurdenMinorPlus), ref TUNING.ROCKETRY.BURDEN.MINOR_PLUS);  //CO2; Habitat-Small; LiquidCargo-Small; Scanner; Sugar
            setTuning(nameof(CustomizeBuildingsState.TuningBurdenModerate), ref TUNING.ROCKETRY.BURDEN.MODERATE);    //GasCargo; OrbitalCargo; Pioneer; Scout; SolidCargo-Small; Steam
            setTuning(nameof(CustomizeBuildingsState.TuningBurdenModeratePlus), ref TUNING.ROCKETRY.BURDEN.MODERATE_PLUS);   //Kerosene-Small; LiquidCargo; LiquidFuel; Oxidizer
            setTuning(nameof(CustomizeBuildingsState.TuningBurdenMajor), ref TUNING.ROCKETRY.BURDEN.MAJOR);   //Habitat; Kerosene; SolidCargo
            setTuning(nameof(CustomizeBuildingsState.TuningBurdenMajorPlus), ref TUNING.ROCKETRY.BURDEN.MAJOR_PLUS);  //Hydrogen
            setTuning(nameof(CustomizeBuildingsState.TuningBurdenMega), ref TUNING.ROCKETRY.BURDEN.MEGA);
            setTuning(nameof(CustomizeBuildingsState.TuningEnginePowerEarlyWeak), ref TUNING.ROCKETRY.ENGINE_POWER.EARLY_WEAK);
            setTuning(nameof(CustomizeBuildingsState.TuningEnginePowerEarlyStrong), ref TUNING.ROCKETRY.ENGINE_POWER.EARLY_STRONG);
            setTuning(nameof(CustomizeBuildingsState.TuningEnginePowerMidVeryStrong), ref TUNING.ROCKETRY.ENGINE_POWER.MID_VERY_STRONG);
            setTuning(nameof(CustomizeBuildingsState.TuningEnginePowerMidStrong), ref TUNING.ROCKETRY.ENGINE_POWER.MID_STRONG);
            setTuning(nameof(CustomizeBuildingsState.TuningEnginePowerMidWeak), ref TUNING.ROCKETRY.ENGINE_POWER.MID_WEAK);
            setTuning(nameof(CustomizeBuildingsState.TuningEnginePowerLateStrong), ref TUNING.ROCKETRY.ENGINE_POWER.LATE_STRONG);
            setTuning(nameof(CustomizeBuildingsState.TuningEnginePowerLateVeryStrong), ref TUNING.ROCKETRY.ENGINE_POWER.LATE_VERY_STRONG);


            setTuning(nameof(CustomizeBuildingsState.TuningFuelCostPerDistanceSugar), ref SugarEngineConfig.FUEL_EFFICIENCY);
            setTuning(nameof(CustomizeBuildingsState.TuningFuelCostPerDistanceMedium), ref TUNING.ROCKETRY.FUEL_COST_PER_DISTANCE.MEDIUM);
            setTuning(nameof(CustomizeBuildingsState.TuningFuelCostPerDistanceHigh), ref TUNING.ROCKETRY.FUEL_COST_PER_DISTANCE.HIGH);
            setTuning(nameof(CustomizeBuildingsState.TuningFuelCostPerDistanceVeryHigh), ref TUNING.ROCKETRY.FUEL_COST_PER_DISTANCE.VERY_HIGH);
            setTuning(nameof(CustomizeBuildingsState.TuningFuelCostPerDistanceGasVeryLow), ref TUNING.ROCKETRY.FUEL_COST_PER_DISTANCE.GAS_VERY_LOW);
            setTuning(nameof(CustomizeBuildingsState.TuningFuelCostPerDistanceGasLow), ref TUNING.ROCKETRY.FUEL_COST_PER_DISTANCE.GAS_LOW);
            setTuning(nameof(CustomizeBuildingsState.TuningFuelCostPerDistanceParticles), ref TUNING.ROCKETRY.FUEL_COST_PER_DISTANCE.PARTICLES);
            
            #endregion

            if (CustomizeBuildingsState.StateManager.State.MaterialIgnoreInsufficientMaterial)
                AccessTools.Property(typeof(GenericGameSettings), nameof(GenericGameSettings.allowInsufficientMaterialBuild))
                    .SetValue(GenericGameSettings.instance, true);
        }

        private static void setTuning(string setting, ref float target)
        {
            var pi = AccessTools.Property(typeof(CustomizeBuildingsState), setting);
            float state = (float)pi.GetValue(CustomizeBuildingsState.StateManager.State);

            if (CustomizeBuildingsState.StateManager.State.TuningGlobal && !float.IsNaN(state))
            {
                target = state;
            }
            else
            {
                Helpers.PrintDebug($"TUNING was set from {state} to {target}");
                pi.SetValue(CustomizeBuildingsState.StateManager.State, target);
            }
        }

        private static void setTuning(string setting, ref int target)
        {
            var pi = AccessTools.Property(typeof(CustomizeBuildingsState), setting);
            float state = (float)pi.GetValue(CustomizeBuildingsState.StateManager.State);

            if (CustomizeBuildingsState.StateManager.State.TuningGlobal && !float.IsNaN(state))
            {
                target = (int)state;
            }
            else
            {
                Helpers.PrintDebug($"TUNING was set from {state} to {target}");
                pi.SetValue(CustomizeBuildingsState.StateManager.State, target);
            }
        }
    }

    [HarmonyPatch(typeof(MaterialSelector), nameof(MaterialSelector.AutoSelectAvailableMaterial))]
    public class MaterialAutoSelect_Patch
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.MaterialAutoSelect;
        }

        public static bool Prefix(MaterialSelector __instance, Recipe ___activeRecipe, ref bool __result)
        {
            if (___activeRecipe == null || __instance.ElementToggles.Count == 0)
                return true;

            Tag previousElement = SaveGame.Instance.materialSelectorSerializer.GetPreviousElement(ClusterManager.Instance.activeWorldId, __instance.selectorIndex, ___activeRecipe.Result);
            if (previousElement == null)
                return true;

            __instance.ElementToggles.TryGetValue(previousElement, out KToggle toggle);
            if (toggle == null)
                return true;

            __instance.OnSelectMaterial(previousElement, ___activeRecipe, true);
            __result = true;
            return false;
        }
    }

    public class Electrolyzer_MaxPressure : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            return id == ElectrolyzerConfig.ID && CustomizeBuildingsState.StateManager.State.ElectrolizerMaxPressure != 1.8f
                || id == RustDeoxidizerConfig.ID && CustomizeBuildingsState.StateManager.State.ElectrolizerMaxPressure != 1.8f;
        }

        public void Edit(BuildingDef def)
        {
            Electrolyzer electrolyzer = def.BuildingComplete.GetComponent<Electrolyzer>();
            if (electrolyzer != null)
                electrolyzer.maxMass = CustomizeBuildingsState.StateManager.State.ElectrolizerMaxPressure;

            RustDeoxidizer rustDeoxidizer = def.BuildingComplete.GetComponent<RustDeoxidizer>();
            if (rustDeoxidizer != null)
                rustDeoxidizer.maxMass = CustomizeBuildingsState.StateManager.State.ElectrolizerMaxPressure;
        }

        public void Undo(BuildingDef def)
        {
            throw new NotImplementedException();
        }
    }
}