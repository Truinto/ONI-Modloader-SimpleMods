using Common;
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
        public static Harmony instance;

        public override void OnLoad(Harmony harmony)
        {
            instance = harmony;
            Helpers.ModName = "CarePackageMod";

            CustomStrings.LoadStrings();
            CustomStrings.ExtraStrings();
#if LOCALE
            Helpers.StringsPrint();
#endif
            Helpers.StringsLoad();

            new PeterHan.PLib.Options.POptions().RegisterOptions(this, typeof(CarePackageState));

            DuplicantStats.OnLoad();
            base.OnLoad(harmony);
        }
    }
}
