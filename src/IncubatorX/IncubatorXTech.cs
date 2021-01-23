using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;


namespace IncubatorXTech
{

    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    internal class IncubatorXTech
    {
        private static void Prefix()
        {
            Debug.Log(" === GeneratedBuildings Prefix === " + IncubatorXConfig.ID);

            Strings.Add("STRINGS.BUILDINGS.PREFABS.INCUBATORX.NAME", "Improved Incubator");
            Strings.Add("STRINGS.BUILDINGS.PREFABS.INCUBATORX.DESC", "Stores multiple eggs.");
            Strings.Add("STRINGS.BUILDINGS.PREFABS.INCUBATORX.EFFECT", "Stores multiple eggs.");

            ModUtil.AddBuildingToPlanScreen("Utilities", IncubatorXConfig.ID);

        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    internal class IncubatorXTechMod
    {
        private static void Prefix(Db __instance)
        {
            List<string> ls1 = new List<string>((string[])Database.Techs.TECH_GROUPING["ImprovedLiquidPiping"]);
            ls1.Add(IncubatorXConfig.ID);
            Database.Techs.TECH_GROUPING["ImprovedLiquidPiping"] = (string[])ls1.ToArray();
        }
    }
}