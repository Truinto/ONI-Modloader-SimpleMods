using Common;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
    public class OnDbInitialize
    {
        public static void Postfix()
        {
            Helpers.CallSafe(SkillStation_Options_Patch.Init);
#if LOCALE
            Helpers.StringsPrint();
#endif
        }
    }
}
