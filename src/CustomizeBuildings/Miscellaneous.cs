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
            try
            {
                #region Tuning
                if (CustomizeBuildingsState.StateManager.State.TuningGlobal)
                {
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningAtmosuitDecay))
                        TUNING.EQUIPMENT.SUITS.ATMOSUIT_DECAY = CustomizeBuildingsState.StateManager.State.TuningAtmosuitDecay;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningAtmosuitAthletics))
                        TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS = (int)CustomizeBuildingsState.StateManager.State.TuningAtmosuitAthletics;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningOxygenMaskDecay))
                        TUNING.EQUIPMENT.SUITS.OXYGEN_MASK_DECAY = CustomizeBuildingsState.StateManager.State.TuningOxygenMaskDecay;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningLeadsuitRadiationShielding))
                        TUNING.EQUIPMENT.SUITS.LEADSUIT_RADIATION_SHIELDING = CustomizeBuildingsState.StateManager.State.TuningLeadsuitRadiationShielding;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningLeadsuitAthletics))
                        TUNING.EQUIPMENT.SUITS.LEADSUIT_ATHLETICS = CustomizeBuildingsState.StateManager.State.TuningLeadsuitAthletics;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningLeadsuitStrength))
                        TUNING.EQUIPMENT.SUITS.LEADSUIT_STRENGTH = CustomizeBuildingsState.StateManager.State.TuningLeadsuitStrength;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningLeadsuitInsulation))
                        TUNING.EQUIPMENT.SUITS.LEADSUIT_INSULATION = CustomizeBuildingsState.StateManager.State.TuningLeadsuitInsulation;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningLeadsuitThermalConductivityBarrier))
                        TUNING.EQUIPMENT.SUITS.LEADSUIT_THERMAL_CONDUCTIVITY_BARRIER = CustomizeBuildingsState.StateManager.State.TuningLeadsuitThermalConductivityBarrier;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningAtmosuitScalding))
                        TUNING.EQUIPMENT.SUITS.ATMOSUIT_SCALDING = (int)CustomizeBuildingsState.StateManager.State.TuningAtmosuitScalding;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningAtmosuitInsulation))
                        TUNING.EQUIPMENT.SUITS.ATMOSUIT_INSULATION = (int)CustomizeBuildingsState.StateManager.State.TuningAtmosuitInsulation;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningAtmosuitThermalConductivityBarrier))
                        TUNING.EQUIPMENT.SUITS.ATMOSUIT_THERMAL_CONDUCTIVITY_BARRIER = CustomizeBuildingsState.StateManager.State.TuningAtmosuitThermalConductivityBarrier;

                    // replace regex to add condition
                    // (.*?) = (.*);
                    // if \(!float\.IsNaN\(\2\)\)\r\n\t\1 = \2

                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningMissionDurationScale))
                        TUNING.ROCKETRY.MISSION_DURATION_SCALE = CustomizeBuildingsState.StateManager.State.TuningMissionDurationScale;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningMassPenaltyExponent))
                        TUNING.ROCKETRY.MASS_PENALTY_EXPONENT = CustomizeBuildingsState.StateManager.State.TuningMassPenaltyExponent;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningMassPenaltyDivisor))
                        TUNING.ROCKETRY.MASS_PENALTY_DIVISOR = CustomizeBuildingsState.StateManager.State.TuningMassPenaltyDivisor;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningResearchEvergreen))
                        TUNING.ROCKETRY.DESTINATION_RESEARCH.EVERGREEN = CustomizeBuildingsState.StateManager.State.TuningResearchEvergreen;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningResearchBasic))
                        TUNING.ROCKETRY.DESTINATION_RESEARCH.BASIC = CustomizeBuildingsState.StateManager.State.TuningResearchBasic;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningAnalysisDiscovered))
                        TUNING.ROCKETRY.DESTINATION_ANALYSIS.DISCOVERED = CustomizeBuildingsState.StateManager.State.TuningAnalysisDiscovered;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningAnalysisComplete))
                        TUNING.ROCKETRY.DESTINATION_ANALYSIS.COMPLETE = CustomizeBuildingsState.StateManager.State.TuningAnalysisComplete;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningAnalysisDefaultCyclesPerDiscovery))
                        TUNING.ROCKETRY.DESTINATION_ANALYSIS.DEFAULT_CYCLES_PER_DISCOVERY = CustomizeBuildingsState.StateManager.State.TuningAnalysisDefaultCyclesPerDiscovery;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningThrustCostsLow))
                        TUNING.ROCKETRY.DESTINATION_THRUST_COSTS.LOW = CustomizeBuildingsState.StateManager.State.TuningThrustCostsLow;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningThrustCostsMid))
                        TUNING.ROCKETRY.DESTINATION_THRUST_COSTS.MID = CustomizeBuildingsState.StateManager.State.TuningThrustCostsMid;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningThrustCostsHigh))
                        TUNING.ROCKETRY.DESTINATION_THRUST_COSTS.HIGH = CustomizeBuildingsState.StateManager.State.TuningThrustCostsHigh;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningThrustCostsVeryHigh))
                        TUNING.ROCKETRY.DESTINATION_THRUST_COSTS.VERY_HIGH = CustomizeBuildingsState.StateManager.State.TuningThrustCostsVeryHigh;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningClusterFowPointsToReveal))
                        TUNING.ROCKETRY.CLUSTER_FOW.POINTS_TO_REVEAL = CustomizeBuildingsState.StateManager.State.TuningClusterFowPointsToReveal;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningClusterFowDefaultCyclesPerReveal))
                        TUNING.ROCKETRY.CLUSTER_FOW.DEFAULT_CYCLES_PER_REVEAL = CustomizeBuildingsState.StateManager.State.TuningClusterFowDefaultCyclesPerReveal;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningEngineEfficiencyWeak))
                        TUNING.ROCKETRY.ENGINE_EFFICIENCY.WEAK = CustomizeBuildingsState.StateManager.State.TuningEngineEfficiencyWeak;   //CO2; Steam
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningEngineEfficiencyMedium))
                        TUNING.ROCKETRY.ENGINE_EFFICIENCY.MEDIUM = CustomizeBuildingsState.StateManager.State.TuningEngineEfficiencyMedium; //Kerosene
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningEngineEfficiencyStrong))
                        TUNING.ROCKETRY.ENGINE_EFFICIENCY.STRONG = CustomizeBuildingsState.StateManager.State.TuningEngineEfficiencyStrong; //Hydrogen; Sugar
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningEngineEfficiencyBooster))
                        TUNING.ROCKETRY.ENGINE_EFFICIENCY.BOOSTER = CustomizeBuildingsState.StateManager.State.TuningEngineEfficiencyBooster;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningOxidizerEfficiencyVeryLow))
                        TUNING.ROCKETRY.OXIDIZER_EFFICIENCY.VERY_LOW = CustomizeBuildingsState.StateManager.State.TuningOxidizerEfficiencyVeryLow;  //Fertilizer
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningOxidizerEfficiencyLow))
                        TUNING.ROCKETRY.OXIDIZER_EFFICIENCY.LOW = CustomizeBuildingsState.StateManager.State.TuningOxidizerEfficiencyLow;   //OxyRock
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningOxidizerEfficiencyHigh))
                        TUNING.ROCKETRY.OXIDIZER_EFFICIENCY.HIGH = CustomizeBuildingsState.StateManager.State.TuningOxidizerEfficiencyHigh;  //LiquidOxygen
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningCargoContainerMassStaticMass))
                        TUNING.ROCKETRY.CARGO_CONTAINER_MASS.STATIC_MASS = CustomizeBuildingsState.StateManager.State.TuningCargoContainerMassStaticMass;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningCargoContainerMassPayloadMass))
                        TUNING.ROCKETRY.CARGO_CONTAINER_MASS.PAYLOAD_MASS = CustomizeBuildingsState.StateManager.State.TuningCargoContainerMassPayloadMass;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningBurdenInsignificant))
                        TUNING.ROCKETRY.BURDEN.INSIGNIFICANT = CustomizeBuildingsState.StateManager.State.TuningBurdenInsignificant;   //Unconstructed
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningBurdenMinor))
                        TUNING.ROCKETRY.BURDEN.MINOR = CustomizeBuildingsState.StateManager.State.TuningBurdenMinor;   //GasCargo; Nosecone; Oxidizer-Small
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningBurdenMinorPlus))
                        TUNING.ROCKETRY.BURDEN.MINOR_PLUS = CustomizeBuildingsState.StateManager.State.TuningBurdenMinorPlus;  //CO2; Habitat-Small; LiquidCargo-Small; Scanner; Sugar
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningBurdenModerate))
                        TUNING.ROCKETRY.BURDEN.MODERATE = CustomizeBuildingsState.StateManager.State.TuningBurdenModerate;    //GasCargo; OrbitalCargo; Pioneer; Scout; SolidCargo-Small; Steam
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningBurdenModeratePlus))
                        TUNING.ROCKETRY.BURDEN.MODERATE_PLUS = CustomizeBuildingsState.StateManager.State.TuningBurdenModeratePlus;   //Kerosene-Small; LiquidCargo; LiquidFuel; Oxidizer
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningBurdenMajor))
                        TUNING.ROCKETRY.BURDEN.MAJOR = CustomizeBuildingsState.StateManager.State.TuningBurdenMajor;   //Habitat; Kerosene; SolidCargo
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningBurdenMajorPlus))
                        TUNING.ROCKETRY.BURDEN.MAJOR_PLUS = CustomizeBuildingsState.StateManager.State.TuningBurdenMajorPlus;  //Hydrogen
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningBurdenMega))
                        TUNING.ROCKETRY.BURDEN.MEGA = CustomizeBuildingsState.StateManager.State.TuningBurdenMega;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningEnginePowerEarlyWeak))
                        TUNING.ROCKETRY.ENGINE_POWER.EARLY_WEAK = CustomizeBuildingsState.StateManager.State.TuningEnginePowerEarlyWeak;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningEnginePowerEarlyStrong))
                        TUNING.ROCKETRY.ENGINE_POWER.EARLY_STRONG = CustomizeBuildingsState.StateManager.State.TuningEnginePowerEarlyStrong;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningEnginePowerMidVeryStrong))
                        TUNING.ROCKETRY.ENGINE_POWER.MID_VERY_STRONG = CustomizeBuildingsState.StateManager.State.TuningEnginePowerMidVeryStrong;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningEnginePowerMidStrong))
                        TUNING.ROCKETRY.ENGINE_POWER.MID_STRONG = CustomizeBuildingsState.StateManager.State.TuningEnginePowerMidStrong;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningEnginePowerMidWeak))
                        TUNING.ROCKETRY.ENGINE_POWER.MID_WEAK = CustomizeBuildingsState.StateManager.State.TuningEnginePowerMidWeak;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningEnginePowerLateStrong))
                        TUNING.ROCKETRY.ENGINE_POWER.LATE_STRONG = CustomizeBuildingsState.StateManager.State.TuningEnginePowerLateStrong;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningFuelCostPerDistanceVeryLow))
                        TUNING.ROCKETRY.FUEL_COST_PER_DISTANCE.VERY_LOW = CustomizeBuildingsState.StateManager.State.TuningFuelCostPerDistanceVeryLow;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningFuelCostPerDistanceLow))
                        TUNING.ROCKETRY.FUEL_COST_PER_DISTANCE.LOW = CustomizeBuildingsState.StateManager.State.TuningFuelCostPerDistanceLow;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningFuelCostPerDistanceMedium))
                        TUNING.ROCKETRY.FUEL_COST_PER_DISTANCE.MEDIUM = CustomizeBuildingsState.StateManager.State.TuningFuelCostPerDistanceMedium;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningFuelCostPerDistanceHigh))
                        TUNING.ROCKETRY.FUEL_COST_PER_DISTANCE.HIGH = CustomizeBuildingsState.StateManager.State.TuningFuelCostPerDistanceHigh;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningFuelCostPerDistanceVeryHigh))
                        TUNING.ROCKETRY.FUEL_COST_PER_DISTANCE.VERY_HIGH = CustomizeBuildingsState.StateManager.State.TuningFuelCostPerDistanceVeryHigh;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningFuelCostPerDistanceGasLow))
                        TUNING.ROCKETRY.FUEL_COST_PER_DISTANCE.GAS_LOW = CustomizeBuildingsState.StateManager.State.TuningFuelCostPerDistanceGasLow;
                    if (!float.IsNaN(CustomizeBuildingsState.StateManager.State.TuningFuelCostPerDistanceGasHigh))
                        TUNING.ROCKETRY.FUEL_COST_PER_DISTANCE.GAS_HIGH = CustomizeBuildingsState.StateManager.State.TuningFuelCostPerDistanceGasHigh;
                }
                #endregion

                if (CustomizeBuildingsState.StateManager.State.MaterialIgnoreInsufficientMaterial)
                    AccessTools.Property(typeof(GenericGameSettings), nameof(GenericGameSettings.allowInsufficientMaterialBuild))
                        .SetValue(GenericGameSettings.instance, true);
            }
            catch (System.Exception e)
            {
                Helpers.Print(e.Message);
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

            Tag previousElement = SaveGame.Instance.materialSelectorSerializer.GetPreviousElement(__instance.selectorIndex, ___activeRecipe.Result);
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

    public class RecipePatch
    {
        public static ComplexRecipe.RecipeElement[] EmptyIngredient = Array.Empty<ComplexRecipe.RecipeElement>();

        public static void Load()
        {
#if DEBUG
            foreach (var recipe in ComplexRecipeManager.Get().recipes)
                Helpers.Print($"id: {recipe.id}, description: {recipe.description}, building: {recipe.fabricators.FirstOrDefault()}");
#endif
            //foreach (var setting in CustomizeBuildingsState.StateManager.State.RecipeSettings)
            //    Process(setting);
        }

        public static void Process(RecipeData setting)
        {
            if (setting.Id == null && setting.Building != null && setting.Inputs != null && setting.Outputs != null)
                setting.Id = ComplexRecipeManager.MakeRecipeID(setting.Building, setting.Inputs, setting.Outputs);

            if (setting.Id == null)
                return;

            var recipe = ComplexRecipeManager.Get().recipes.Find(f => f.id == setting.Id);
            if (recipe == null)
            {
                recipe = new ComplexRecipe(setting.Id, EmptyIngredient, EmptyIngredient)
                {
                    time = 40f,
                    fabricators = new List<Tag>() { setting.Building.ToTag() },
                    nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult
                };
            }

            if (setting.Description != null)
                recipe.description = setting.Description;
            if (setting.Inputs != null)
                recipe.ingredients = setting.Inputs.ToArray();
            if (setting.Outputs != null)
                recipe.results = setting.Outputs.ToArray();
            if (setting.Time != null)
                recipe.time = setting.Time.Value;
            if (setting.HEP != null)
                recipe.consumedHEP = setting.HEP.Value;
        }

        public class RecipeData //need [Id] or [Building, Inputs, Outputs]; note that you always need Id, if you are changing any elements of existing recipes
        {
            public string Id;
            public string Building;
            public string Description;
            public List<ComplexRecipe.RecipeElement> Inputs;
            public List<ComplexRecipe.RecipeElement> Outputs;
            public float? Time;
            public int? HEP;
        }
    }
}