using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection.Emit;
using System.Reflection;
using Common;

namespace CarePackageMod
{
    [HarmonyPatch(typeof(HeadquartersConfig), nameof(HeadquartersConfig.ConfigureBuildingTemplate))]
    public class PrintingPod
    {
        public static void Postfix(GameObject go)
        {
            var prefab = go.GetComponent<KPrefabID>();

            prefab.prefabInitFn += delegate (GameObject inst)
            {
                if (go != null)
                {
                    Helpers.Print("PrintingPod OnRefreshUserMenu was not NULL!");
                    go.Unsubscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenu);
                }
                PrintingPod.go = inst;
                inst.Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenu);
            };
        }

        public static GameObject go;

        public static void OnRefreshUserMenu(object data)
        {
            if (go != null)
                Helpers.AddButton(go, Strings.Get("CarePackageMod.LOCSTRINGS.CarepackageOption"), "", ButtonPress, 20f);
        }

        public static void ButtonPress()
        {
            PeterHan.PLib.Options.POptions.ShowDialog(typeof(CarePackageState), OptionChanged.ReloadConfig);
            OptionChanged.ReloadConfig();
        }
    }

    [HarmonyPatch(typeof(ModsScreen), "OnActivate")]
    public class OptionChanged
    {
        public static void Postfix(KButton ___closeButton)
        {
            ___closeButton.onClick += ReloadConfig;
        }

        public static void ReloadConfig(object @null) => ReloadConfig();
        public static void ReloadConfig()
        {
            CarePackageState.StateManager.TryReloadConfiguratorState();
            Reshuffle4.Prepare();
            InitializeContainers.Total = CarePackageState.StateManager.State.rosterDupes + CarePackageState.StateManager.State.rosterPackages;
            InitializeContainers.CarePackages = CarePackageState.StateManager.State.rosterPackages;
            CarePackageMod.carePackages = null;
        }
    }
}