using Common;
using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipedEverything
{
    public class FumiKMod : KMod.UserMod2
    {
        public const string ModName = "PipedEverything";
        public static Harmony instance;

        public override void OnLoad(Harmony harmony)
        {
            // init
            instance = harmony;
            Helpers.ModName = ModName;

            // predefine strings

            // load translation, if any

            // load settings
            PipedEverythingState.StateManager = new(PipedEverythingState.GetStaticConfigPath(), true, PipedEverythingState.OnUpdate, null);

            // init options menu

            // call OnLoad methods

            // patch all harmony classes
            base.OnLoad(harmony);
        }
    }
}
