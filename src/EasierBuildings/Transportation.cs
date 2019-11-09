using Harmony;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(LadderConfig), "DoPostConfigureComplete")]
    internal class LadderConfig_DoPostConfigureComplete
    {
        private static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.LadderCometInvincibility;
        }
        private static void Prefix(GameObject go)
        {
            KPrefabID prefabID = go.AddOrGet<KPrefabID>();
            if (prefabID != null)
                prefabID.AddTag(GameTags.Bunker, false);
        }
    }
    [HarmonyPatch(typeof(Comet), "DamageThings")]
    internal class Comet_DamageThings
    {
        internal static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.LadderCometInvincibility;
        }

        internal static bool Prefix(int cell)
        {
            GameObject go = Grid.Objects[cell, 1];
            if (go != null)
            {
                KPrefabID id = go.GetComponent<KPrefabID>();
                if (id != null)
                {
                    if (id.HasTag(GameTags.Bunker)) return false;
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(TravelTubeConfig), "CreateBuildingDef")]
    internal class TravelTubeConfig_CreateBuildingDef
    {
        private static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.TransitTubeAnywhere;
        }
        private static void Postfix(BuildingDef __result)
        {
            //__result.SceneLayer = Grid.SceneLayer.Wires;
            //__result.ObjectLayer = ObjectLayer.Wire;
            //__result.TileLayer = ObjectLayer.WireTile;

            __result.BuildLocationRule = BuildLocationRule.Anywhere;            
            __result.ObjectLayer = ObjectLayer.Canvases;
            //__result.TileLayer = ObjectLayer.Canvases;
        }
    }

    [HarmonyPatch(typeof(UtilityNetworkTubesManager), "CanAddConnection")]
    internal class UtilityNetworkTubesManager_CanAddConnection
    {
        private static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.TransitTubeUTurns;
        }
        private static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(TravelTubeEntranceConfig), "ConfigureBuildingTemplate")]
    internal class TravelTubeEntranceConfig_ConfigureBuildingTemplate
    {
        private static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.TransitTubeJoulesPerLaunch != 10000f;
        }
        private static void Postfix(GameObject go)
        {
            TravelTubeEntrance travelTubeEntrance = go.GetComponent<TravelTubeEntrance>();
            if (travelTubeEntrance != null)
            {
                travelTubeEntrance.joulesPerLaunch = CustomizeBuildingsState.StateManager.State.TransitTubeJoulesPerLaunch;
                //travelTubeEntrance.jouleCapacity = 4000f;
            }
        }
    }
    
    //[HarmonyPatch(typeof(BipedTransitionLayer), new Type[] { typeof(Navigator), typeof(float), typeof(float) })]
    //internal class BipedTransitionLayer_BipedTransitionLayer
    //{
    //    private static bool Prepare()
    //    {
    //        Debug.Log("HELLO WORLD1");
    //        return CustomizeBuildingsState.StateManager.State.TransitTubeSpeed != 18f;
    //    }
    //    private static void Prefix(ref float ___tubeSpeed)
    //    {
    //        //___tubeSpeed = CustomizeBuildingsState.StateManager.State.TransitTubeSpeed;
    //        //Debug.Log("Transit Tube speed set to: " + CustomizeBuildingsState.StateManager.State.TransitTubeSpeed.ToString());
    //        Debug.Log("HELLO WORLD2");
    //    }
    //}

}
