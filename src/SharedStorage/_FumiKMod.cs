using Common;
using HarmonyLib;
using KMod;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedStorage
{
    public class FumiKMod : KMod.UserMod2
    {
        public const string ModName = "SharedStorage";
        public static Harmony instance;

        public override void OnLoad(Harmony harmony)
        {
            // init
            instance = harmony;
            Helpers.ModName = ModName;

            // predefine strings
            CustomStrings.LoadStrings();
            Helpers.StringsPrint();

            // load translation, if any
            Helpers.ActiveLocale = Helpers.StringsLoad();

            // load settings
            SharedStorageState.StateManager = new(SharedStorageState.GetStaticConfigPath(), true, SharedStorageState.OnUpdate, null);

            // init options menu
            new POptions().RegisterOptions(this, typeof(SharedStorageState));

            // call OnLoad methods

            // patch all harmony classes
            base.OnLoad(harmony);
        }
    }
}
