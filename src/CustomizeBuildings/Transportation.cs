using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System;
using Common;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(LadderConfig), "ConfigureBuildingTemplate")]
    public class LadderConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.LadderCometInvincibility;
        }
        public static void Postfix(GameObject go)
        {
            try
            {
                KPrefabID prefabID = go.GetComponent<KPrefabID>();
                if (prefabID != null)
                    prefabID.AddTag(GameTags.Bunker, false);
            }
            catch (Exception)
            {
            }
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
            try
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
            }
            catch (Exception)
            {
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
            return CustomizeBuildingsState.StateManager.State.TransitTubeJoulesPerLaunch != 10000f || CustomizeBuildingsState.StateManager.State.TransitTubeJouleCapacity != 40000f;
        }
        private static void Postfix(GameObject go)
        {
            TravelTubeEntrance travelTubeEntrance = go.GetComponent<TravelTubeEntrance>();
            if (travelTubeEntrance != null)
            {
                travelTubeEntrance.joulesPerLaunch = CustomizeBuildingsState.StateManager.State.TransitTubeJoulesPerLaunch;
                travelTubeEntrance.jouleCapacity = CustomizeBuildingsState.StateManager.State.TransitTubeJouleCapacity;
                //travelTubeEntrance.jouleCapacity = 4000f;
            }
        }
    }

    //[HarmonyPatch(typeof(BipedTransitionLayer), nameof(BipedTransitionLayer.BeginTransition))]
    public class TravelTubeSpeed_Patch
    {
        public static bool Prepare()
        {
            return true;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var line in instructions)
            {
                if (line.opcode == OpCodes.Ldc_R4 && (float)line.operand == 18f)
                {
                    line.operand = 30f;
                    Helpers.PrintDebug("Patched BipedTransitionLayer");
                }
                yield return line;
            }
        }
    }

    //[HarmonyPatch(typeof(TUNING.DUPLICANTSTATS.MOVEMENT), MethodType.StaticConstructor)]
    public class Speed_Patch
    {
        public static bool Prepare()
        {
            return true;
        }

        public static void OnLoad()
        {
            return;
            TUNING.DUPLICANTSTATS.MOVEMENT.BONUS_2 = 1.25f;     // Standard Tile
            TUNING.DUPLICANTSTATS.MOVEMENT.BONUS_3 = 1.5f;      // Metal, Plastic Tile
            TUNING.DUPLICANTSTATS.MOVEMENT.PENALTY_2 = 0.75f;   // Carpet
            TUNING.DUPLICANTSTATS.MOVEMENT.PENALTY_3 = 0.5f;    // Travel Tube Bridge
        }
    }

}
