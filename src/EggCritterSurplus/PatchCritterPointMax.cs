using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EggCritterSurplus
{
#pragma warning disable CS0618
    [HarmonyPatch(typeof(CreatureDeliveryPoint))]
    [HarmonyPatch("IUserControlledCapacity.MaxCapacity", PropertyMethod.Getter)]
    public class PatchCritterPointMax
    {
        public static bool Prefix(ref float __result)
        {
            __result = 100f;
            return false;
        }
    }
}
