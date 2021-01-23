using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ForceFieldTileTech
{

    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    internal class ForceFieldTileTech
    {
        private static void Prefix()
        {
            Debug.Log(" === GeneratedBuildings Prefix === " + ForceFieldTileConfig.ID);

            Strings.Add("STRINGS.BUILDINGS.PREFABS.FORCEFIELDTILE.NAME", "Force Field Tile");
            Strings.Add("STRINGS.BUILDINGS.PREFABS.FORCEFIELDTILE.DESC", "Allows dupes to pass, but not gasses nor liquids.");
            Strings.Add("STRINGS.BUILDINGS.PREFABS.FORCEFIELDTILE.EFFECT", "Allows dupes to pass, but not gasses nor liquids.");

            ModUtil.AddBuildingToPlanScreen("Utilities", ForceFieldTileConfig.ID);

        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    internal class ForceFieldTileTechMod
    {
        private static void Prefix(Db __instance)
        {
            List<string> ls1 = new List<string>((string[])Database.Techs.TECH_GROUPING["ImprovedLiquidPiping"]);
            ls1.Add(ForceFieldTileConfig.ID);
            Database.Techs.TECH_GROUPING["ImprovedLiquidPiping"] = (string[])ls1.ToArray();
        }
    }
}