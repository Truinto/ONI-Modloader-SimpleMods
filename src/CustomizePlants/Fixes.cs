using Common;
using HarmonyLib;
using Shared.CollectionNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomizePlants
{
    [HarmonyPatch]
    public class Fixes
    {
        /// <summary>
        /// Makes it so components with [MyCmpReq] do not crash the game, if that component is missing.
        /// </summary>
        public static void SuppressMyCmpReqException()
        {
            var mycmp = MyAttributes.s_attributeMgrs.Get<MyCmp>();
            mycmp.m_methodInfosByAttribute[typeof(MyCmpReq)] = mycmp.m_methodInfosByAttribute[typeof(MyCmpGet)];
        }

        [HarmonyPatch(typeof(FallingWater), nameof(FallingWater.AddParticle),
            typeof(Vector2), typeof(ushort), typeof(float), typeof(float), typeof(byte), typeof(int), typeof(bool), typeof(bool), typeof(bool), typeof(bool))]
        public static class Patch_ElementConsumerCrashWithGases
        {
            public static bool Prepare()
            {
                return CustomizePlantsState.StateManager.State.ApplyBugFixes;
            }

            public static bool Prefix(float base_mass)
            {
                if (base_mass <= 0f)
                    return false;
                return true;
            }
        }

        [HarmonyPatch(typeof(ColdBreather), nameof(ColdBreather.OnReplanted))]
        public static class Patch_ColdBreatherOnReplanted
        {
            public static float? _cache = CustomizePlantsState.StateManager.State.PlantSettings.FirstOrDefault(f => f.id == ColdBreatherConfig.ID)?.radiation;

            public static bool Prepare()
            {
                return CustomizePlantsState.StateManager.State.ApplyBugFixes;
            }

            public static void Postfix(RadiationEmitter ___radiationEmitter)
            {
                if (_cache != null)
                    ___radiationEmitter.emitRads = _cache.Value;
            }
        }

        //[HarmonyPatch(typeof(GameUtil), nameof(GameUtil.ActionToBinding))]
        public static class Patch_GameInputBindings
        {
            public static bool Printed = false;
            public static void Prefix()
            {
                if (!Printed)
                {
                    foreach (BindingEntry b in GameInputMapping.KeyBindings)
                    {
                        Debug.Log($"{b}: {b.mGroup} {b.mRebindable} {b.mIgnoreRootConflics} {b.mButton} {b.mKeyCode} {b.mAction} {b.mModifier}");
                    }
                    Printed = true;
                }

            }

            public static Exception Finalizer(Exception __exception)
            {
                if (__exception != null)
                    Debug.Log(__exception.StackTrace);
                return null;
            }
        }
    }
}
