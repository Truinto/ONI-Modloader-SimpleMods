using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace NoRemains
{
    [HarmonyPatch(typeof(BuildingConfigManager), nameof(BuildingConfigManager.RegisterBuilding))]
    public class Patch_Test
    {
        public static void Postfix()
        {
            try
            {
                //Debug.Log($"HELLO WORLD Test patch {BuildingConfigManager.Instance.baseTemplate?.GetType()}");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

    [HarmonyPatch(typeof(DeathMonitor.Instance), "ApplyDeath")]
    public class DeathMonitor_ApplyDeath
    {
        public static bool Prefix(DeathMonitor.Instance __instance, bool ___isDuplicant)
        {
            if (!___isDuplicant)
                return true;

            if (App.IsExiting || KMonoBehaviour.isLoadingScene)
                return true;

            Util.KDestroyGameObject(__instance.gameObject);
            return false;
        }
    }

    
    [HarmonyPatch(typeof(SuffocationMonitor.Instance), "Kill")]
    public class SuffocationPatch
    {
        public static bool Prefix()
        {
            return false;
        }
    }

    //[HarmonyPatch(typeof(OxygenBreather))]
    //[HarmonyPatch("mouthCell", MethodType.Getter)]
    //public class MouthCellPatch
    //{
    //    public static bool Prefix(ref int __result, OxygenBreather __instance)
    //    {
    //        __result = Grid.InvalidCell;
    //        //__result = __instance.GetMouthCellAtCell(Grid.PosToCell((KMonoBehaviour)__instance), __instance.breathableCells);
    //        return false;
    //    }
    //}
}