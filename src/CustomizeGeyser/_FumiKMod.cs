using Common;
using HarmonyLib;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeGeyser
{
    public class FumiKMod : KMod.UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            Helpers.ModName = "CustomizeGeyser";
            CustomStrings.LoadStrings();
            Helpers.StringsAddClass(typeof(GeyserStruct));
#if LOCALE
            Helpers.StringsPrint();
#else
            Helpers.StringsLoad();
#endif

            new POptions().RegisterOptions(this, typeof(CustomizeGeyserState));

            CustomizeGeyserState.OnLoad();
            base.OnLoad(harmony);
        }
    }
}