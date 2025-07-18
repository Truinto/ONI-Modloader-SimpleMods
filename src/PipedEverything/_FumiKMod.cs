﻿using Common;
using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PipedEverything
{
    public class FumiKMod : KMod.UserMod2
    {
        public const string ModName = "PipedEverything";
        public static Harmony? instance;

        private static float _solidMaxMass = float.NaN;
        public static float SolidMaxMass
        {
            get
            {
                if (_solidMaxMass == float.NaN)
                {
                    var warpStorage = Assets.TryGetPrefab(WarpConduitSenderConfig.ID).GetComponent<WarpConduitSender>().solidStorage;
                    _solidMaxMass = warpStorage.capacityKg / 5f;
                }
                return _solidMaxMass;
            }
        }

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
            var advancedGen = Type.GetType("AdvancedGenerators.Common.AdvancedEnergyGenerator, AdvancedGenerators")
                ?? Type.GetType("AdvancedGenerators.Common.AdvancedEnergyGenerator, Advanced Generators");

            if (advancedGen != null)
            {
                harmony.Patch(
                    advancedGen.GetMethod("EmitElements", Helpers.AllBinding), 
                    transpiler: new HarmonyMethod(typeof(Patches_AdvancedGenerators).GetMethod(nameof(Patches_AdvancedGenerators.Transpiler)))
                    );
            }
            else
            {
                Helpers.Print($"AdvancedGenerators is not installed");
                //foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
                //{
                //    Console.WriteLine($"Assembly: {ass.FullName}");
                //    foreach (Type t in ass.GetTypes())
                //    {
                //        Console.WriteLine($"Assembly: {t.AssemblyQualifiedName}");
                //    }
                //}
            }
        }
    }
}
