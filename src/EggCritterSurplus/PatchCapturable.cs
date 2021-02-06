using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
}
