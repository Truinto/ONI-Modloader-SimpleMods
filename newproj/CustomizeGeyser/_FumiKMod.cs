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
        public const string ModName = "CustomizeGeyser";
        public static Harmony instance;

        public override void OnLoad(Harmony harmony)
        {
            // init
            instance = harmony;
            Helpers.ModName = ModName;

            // predefine strings
            CustomStrings.LoadStrings();
            CustomStrings.ExtraStrings();
            Helpers.StringsAddClass(typeof(GeyserStruct));
            Helpers.StringsPrint();

            // load translation, if any
            Helpers.ActiveLocale = Helpers.StringsLoad();

            // load settings
            //Helpers.CallSafe(CustomizeBuildingsState.BeforeUpdate);
            CustomizeGeyserState.StateManager = new(CustomizeGeyserState.GetStaticConfigPath(), true, CustomizeGeyserState.OnUpdate, null);

            // init options menu
            new POptions().RegisterOptions(this, typeof(CustomizeGeyserState));

            // call OnLoad methods
            Helpers.CallSafe(CustomizeGeyserState.OnLoad);

            // patch all harmony classes
            base.OnLoad(harmony);
        }
    }
}