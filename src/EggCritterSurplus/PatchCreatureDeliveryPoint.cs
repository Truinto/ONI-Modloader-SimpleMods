using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EggCritterSurplus
{
    [HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.CreateAndRegisterBaggedCreature))]
    public class PatchCapturable
    {
        public static void Prefix(ref bool allow_mark_for_capture)
        {
            allow_mark_for_capture = true;
        }
    }

    [HarmonyPatch(typeof(CreatureDeliveryPoint), "IUserControlledCapacity.MaxCapacity", MethodType.Getter)]
    public class PatchCritterPointMax
    {
        public static bool Prefix(ref float __result)
        {
            __result = 100f;
            return false;
        }
    }

    [HarmonyPatch(typeof(CreatureDeliveryPointConfig), nameof(CreatureDeliveryPointConfig.DoPostConfigureComplete))]
    [HarmonyPriority(Priority.VeryLow)]
    public class PatchFishDelivery
    {
        public static void Postfix(GameObject go)
        {
            var storage = go.GetComponent<Storage>();
            if (storage.storageFilters.Contains(GameTags.SwimmingCreature))
                return;

            storage.storageFilters = storage.storageFilters.ToList();
            storage.storageFilters.Add(GameTags.SwimmingCreature);
        }
    }
}
