using Harmony;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System;

namespace CustomizeBuildings
{
    public class Miscellaneous
    {
        public static void OnLoad()
        {
            TUNING.EQUIPMENT.SUITS.ATMOSUIT_DECAY = 0f;
            TUNING.EQUIPMENT.SUITS.ATMOSUIT_ATHLETICS = -2;
            TUNING.EQUIPMENT.SUITS.OXYGEN_MASK_DECAY = 0f;
            TUNING.EQUIPMENT.SUITS.ATMOSUIT_SCALDING = 4000;
            TUNING.EQUIPMENT.SUITS.ATMOSUIT_INSULATION = 4000;
            TUNING.EQUIPMENT.SUITS.ATMOSUIT_THERMAL_CONDUCTIVITY_BARRIER = 1f;
        }
    }

    //[HarmonyPatch(typeof(TileConfig), "ConfigureBuildingTemplate")]
    //internal class TileDryWallReplacement
    //{
    //    internal static bool Prepare()
    //    {
    //        return true;
    //    }

    //    internal static void Postfix(GameObject go)
    //    {

    //    }
    //}


    //[HarmonyPatch(typeof(ExteriorWallConfig), "CreateBuildingDef", new Type[] { })]
    //internal class TileDryWallReplacement
    //{
    //    internal static bool Prepare()
    //    {
    //        return true;
    //    }

    //    internal static void Postfix(ref BuildingDef __result)
    //    {
    //        BuildingTemplates.CreateLadderDef(__result);
    //    }
    //}

}