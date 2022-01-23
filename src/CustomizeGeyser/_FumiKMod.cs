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
            CustomStrings.ExtraStrings();
            Helpers.StringsAddClass(typeof(GeyserStruct));
#if LOCALE
            Helpers.StringsPrint();
#endif
            Helpers.StringsLoad();

            new POptions().RegisterOptions(this, typeof(CustomizeGeyserState));

            CustomizeGeyserState.OnLoad();
            base.OnLoad(harmony);
        }
    }
}