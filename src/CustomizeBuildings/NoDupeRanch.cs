using Harmony;
using UnityEngine;
using System;
using Klei.AI;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(ModifierSet), "LoadEffects")]
    internal class ModifierSet_LoadEffects
    {
        private static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeBuildingsGlobal && CustomizeBuildingsState.StateManager.State.NoDupeBuildings.GetValueSafe(CustomizeBuildingsState.IDRanchStation);
        }

        private static void Postfix(ModifierSet __instance)
        {
            //Klei.AI.Effect resource1 = new Klei.AI.Effect("Ranched", (string)STRINGS.CREATURES.MODIFIERS.RANCHED.NAME, (string)STRINGS.CREATURES.MODIFIERS.RANCHED.TOOLTIP, 1200f, true, true, false, (string)null, 0.0f, (string)null);
            //resource1.Add(new AttributeModifier(Db.Get().Amounts.Wildness.deltaAttribute.Id, -0.09166667f, (string)STRINGS.CREATURES.MODIFIERS.RANCHED.NAME, false, false, true));
            //__instance.effects.Remove("Ranched");
            //for (int i = 0; i < __instance.effects.Count; i++)
            //{
            //    Klei.AI.Effect effect = (Klei.AI.Effect)__instance.effects.GetResource(i);
            //    if (effect == null)
            //    {
            //        Debug.LogWarning("CRITICAL");
            //        continue;
            //    }
            //    if (effect.Id == "Ranched")
            //        effect.duration = 1200f;
            //}
            //__instance.effects.Add(resource1);

            __instance.effects.Get("Ranched").duration = 60000f;

        }
    }
}
