using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;


namespace LiquidReservoirSmartMod
{    
    
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    internal class LiquidReservoirSmartMod
	{
        private static void Prefix()
        {
            Debug.Log(" === GeneratedBuildings Prefix === "+ LiquidReservoirSmartConfig.ID);			

			Strings.Add("STRINGS.BUILDINGS.PREFABS.LIQUIDRESERVOIRSMART.NAME", "Smart Liquid Reservoir");
            Strings.Add("STRINGS.BUILDINGS.PREFABS.LIQUIDRESERVOIRSMART.DESC", "Gives logical output when full");
            //Strings.Add("STRINGS.BUILDINGS.PREFABS.LIQUIDRESERVOIRSMART.EFFECT", "Stores liquids");
            Strings.Add("STRINGS.BUILDINGS.PREFABS.LIQUIDRESERVOIRSMART.EFFECT", "Stores " + STRINGS.UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID"));

            ModUtil.AddBuildingToPlanScreen("Utilities", LiquidReservoirSmartConfig.ID);

		}
    }

	[HarmonyPatch(typeof(Db), "Initialize")]
	internal class LiquidReservoirSmartTechMod
	{
        private static void Prefix(Db __instance)
        {
            List<string> ls1 = new List<string>((string[])Database.Techs.TECH_GROUPING["ImprovedLiquidPiping"]);
            ls1.Add(LiquidReservoirSmartConfig.ID);
            Database.Techs.TECH_GROUPING["ImprovedLiquidPiping"] = (string[])ls1.ToArray();
        }
	}

    /*[HarmonyPatch(typeof(KSerialization.Manager))] //probably need something like this for my filters
    [HarmonyPatch("GetType")]
    [HarmonyPatch(new[] { typeof(string) })]
    public static class KSerializationManager_GetType_Patch
    {
        public static void Postfix(string type_name, ref Type __result)
        {
            if (type_name == "ConveyorRailUtilities.ConveyorFilter.ConveyorFilter")
            {
                __result = typeof(ConveyorFilter);
            }

            if (type_name == "ConveyorRailUtilities.ConveyorShutoff.ConveyorShutoff")
            {
                __result = typeof(ConveyorShutoff);
            }
        }
    }*/
}