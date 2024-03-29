﻿using HarmonyLib;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public enum PlanScreens
    {
        Base,
        Oxygen,
        Power,
        Food,
        Plumbing,
        HVAC,
        Refining,
        Medical,
        Furniture,
        Equipment,
        Utilities,
        Automation,
        Conveyance,
        Rocketry,
    }

    public enum TechGroups
    {
        FarmingTech,
        FineDining,
        FoodRepurposing,
        FinerDining,
        Agriculture,
        Ranching,
        AnimalControl,
        ImprovedOxygen,
        GasPiping,
        ImprovedGasPiping,
        PressureManagement,
        PortableGasses,
        DirectedAirStreams,
        LiquidFiltering,
        MedicineI,
        MedicineII,
        MedicineIII,
        MedicineIV,
        LiquidPiping,
        ImprovedLiquidPiping,
        PrecisionPlumbing,
        SanitationSciences,
        FlowRedirection,
        AdvancedFiltration,
        Distillation,
        Catalytics,
        PowerRegulation,
        AdvancedPowerRegulation,
        PrettyGoodConductors,
        RenewableEnergy,
        Combustion,
        ImprovedCombustion,
        InteriorDecor,
        Artistry,
        Clothing,
        Acoustics,
        NuclearRefinement,
        FineArt,
        EnvironmentalAppreciation,
        Luxury,
        RefractiveDecor,
        GlassFurnishings,
        Screens,
        RenaissanceArt,
        Plastics,
        ValveMiniaturization,
        HydrocarbonPropulsion,
        Suits,
        Jobs,
        AdvancedResearch,
        SpaceProgram,
        CrashPlan,
        DurableLifeSupport,
        NuclearResearch,
        NotificationSystems,
        ArtificialFriends,
        RoboticTools,
        BasicRefinement,
        RefinedObjects,
        Smelting,
        HighTempForging,
        RadiationProtection,
        TemperatureModulation,
        HVAC,
        LiquidTemperature,
        LogicControl,
        GenericSensors,
        LogicCircuits,
        ParallelAutomation,
        DupeTrafficControl,
        Multiplexing,
        SkyDetectors,
        TravelTubes,
        SmartStorage,
        SolidTransport,
        SolidManagement,
        BasicRocketry,
        CargoI,
        CargoII,
        CargoIII,
        EnginesI,
        EnginesII,
        EnginesIII,
        Jetpacks,
    }

    public class TechContainer
    {
        public string BuildingId;
        public TechGroups TechGroup;

        public TechContainer(string BuildingId, TechGroups TechGroup)
        {
            this.BuildingId = BuildingId;
            this.TechGroup = TechGroup;
        }
    }

    public class TechHelper
    {
        public static void AddBuildingStrings(string buildingId, string name, string description, string effect)
        {
            string str = buildingId.ToUpperInvariant();
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{str}.NAME", UI.FormatAsLink(name, buildingId));
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{str}.DESC", description);
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{str}.EFFECT", effect);
        }

        public static void AddPlantStrings(string plantId, string name, string description, string domesticatedDescription)
        {
            string str = plantId.ToUpperInvariant();
            Strings.Add($"STRINGS.CREATURES.SPECIES.{str}.NAME", UI.FormatAsLink(name, plantId));
            Strings.Add($"STRINGS.CREATURES.SPECIES.{str}.DESC", description);
            Strings.Add($"STRINGS.CREATURES.SPECIES.{str}.DOMESTICATEDDESC", domesticatedDescription);
        }

        public static void AddPlantSeedStrings(string plantId, string name, string description)
        {
            string str = plantId.ToUpperInvariant();
            Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{str}.NAME", UI.FormatAsLink(name, plantId));
            Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{str}.DESC", description);
        }

        public static string ConvertCategory(HashedString hash)
        {
            foreach (var obj in Enum.GetValues(typeof(PlanScreens)))
            {
                if (hash == obj.ToString())
                    return obj.ToString();
            }
            return null;
        }

        public static void AddBuildingToPlanScreen(string buildingId, PlanScreens category, string subcategory = "uncategorized")
        {
            int index = TUNING.BUILDINGS.PLANORDER.FindIndex(f => f.category == category.ToString());
            if (index < 0)
            {
                Debug.Log($"[TECHHELPER] Could not add {buildingId} to {category}");
                return;
            }

            //TUNING.BUILDINGS.PLANORDER[index].buildingAndSubcategoryData.Add(new KeyValuePair<string, string>(buildingId, subcategory));
            ModUtil.AddBuildingToPlanScreen(category.ToString(), buildingId, subcategory);
            Debug.Log($"[TECHHELPER] Added {buildingId} to {category}");
        }

        public static void AddBuildingToTechnology(string buildingId, TechGroups group)
        {
            var tech = Db.Get().Techs.TryGet(group.ToString());//"Ranching"
            tech.unlockedItemIDs.Add(buildingId);
        }

        public static List<TechContainer> AddOnLoad { get; set; } = new List<TechContainer>();

        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public static class Db_Initialize_Patch
        {
            public static void Postfix()
            {
                foreach (var tech in AddOnLoad)
                    AddBuildingToTechnology(tech.BuildingId, tech.TechGroup);
            }
        }
    }
}