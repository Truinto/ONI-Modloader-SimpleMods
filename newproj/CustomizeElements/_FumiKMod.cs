using Common;
using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeElements
{
    public class FumiKMod : KMod.UserMod2
    {
        public const string ModName = "CustomizeElements";
        public static Harmony instance;

        public override void OnLoad(Harmony harmony)
        {
            // init
            instance = harmony;
            Helpers.ModName = ModName;

            // todo add translation support

            // load settings
            CustomizeElementsState.StateManager = new(Config.PathHelper.CreatePath(Helpers.ModName), true, CustomizeElementsState.OnUpdate, null);

            // patch all harmony classes
            base.OnLoad(harmony);
        }
    }
}
