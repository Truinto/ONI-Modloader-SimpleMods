using Harmony;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System;

namespace CustomizeBuildings
{

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