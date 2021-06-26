using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EggCritterSurplus
{
    [HarmonyPatch(typeof(CreatureDeliveryPoint), "IUserControlledCapacity.MaxCapacity", MethodType.Getter)]
    public class PatchCritterPointMax
    {
        public static bool Prefix(ref float __result)
        {
            __result = 100f;
            return false;
        }
    }
}
