using Harmony;
using STRINGS;
using System;
using System.Collections.Generic;


namespace Techs
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
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.NAME", UI.FormatAsLink(name, buildingId));
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.DESC", description);
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.EFFECT", effect);
        }

        public static void AddPlantStrings(string plantId, string name, string description, string domesticatedDescription)
        {
            Strings.Add($"STRINGS.CREATURES.SPECIES.{plantId.ToUpperInvariant()}.NAME", UI.FormatAsLink(name, plantId));
            Strings.Add($"STRINGS.CREATURES.SPECIES.{plantId.ToUpperInvariant()}.DESC", description);
            Strings.Add($"STRINGS.CREATURES.SPECIES.{plantId.ToUpperInvariant()}.DOMESTICATEDDESC", domesticatedDescription);
        }

        public static void AddPlantSeedStrings(string plantId, string name, string description)
        {
            Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{plantId.ToUpperInvariant()}.NAME", UI.FormatAsLink(name, plantId));
            Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{plantId.ToUpperInvariant()}.DESC", description);
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

        public static void AddBuildingToPlanScreen(string buildingId, PlanScreens category)
        {
            //ModUtil.AddBuildingToPlanScreen(buildingId, category.ToString());
            //Debug.Log("HELLO WORLD: Printout Planorder");
            //foreach (var order in TUNING.BUILDINGS.PLANORDER)
            //    Debug.Log($"Category={order.category} as string {ConvertCategory(order.category)}; {order.data.Join()}");

            bool flag = false;
            for (int i = 0; i < TUNING.BUILDINGS.PLANORDER.Count; i++)
            {
                if (TUNING.BUILDINGS.PLANORDER[i].category == category.ToString())
                {
                    (TUNING.BUILDINGS.PLANORDER[i].data as IList<string>).Add(buildingId);
                    Debug.Log($"[TECHHELPER] Added {buildingId} to {category}");
                    flag = true;
                    break;
                }
            }

            if (!flag)
                Debug.Log($"[TECHHELPER] Category {category} not found");
        }

        public static void AddBuildingToTechnology(string buildingId, TechGroups group)
        {
#if DLC1
            var tech = Db.Get().Techs.TryGet(group.ToString());//"Ranching"
            tech.unlockedItemIDs.Add(buildingId);
#else
            List<string> stringList = new List<string>(Database.Techs.TECH_GROUPING[group.ToString()])
            {
                buildingId
            };
            Database.Techs.TECH_GROUPING[group.ToString()] = stringList.ToArray();
#endif
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