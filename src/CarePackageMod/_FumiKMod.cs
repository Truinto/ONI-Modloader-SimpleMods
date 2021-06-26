using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarePackageMod
{
    public class FumiKMod : KMod.UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);

            CarePackageState.LoadStrings();
            new PeterHan.PLib.Options.POptions().RegisterOptions(this, typeof(CarePackageState));
        }
    }
}
