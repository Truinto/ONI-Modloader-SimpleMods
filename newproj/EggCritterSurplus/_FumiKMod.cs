using Common;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EggCritterSurplus
{
    public class FumiKMod : KMod.UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            Helpers.ModName = "SweepThoseEggs";
            Patches.OnLoad();
            base.OnLoad(harmony);
        }
    }
}