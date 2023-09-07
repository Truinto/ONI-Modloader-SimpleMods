//#define LOCALE
using System.Collections.Generic;
using Newtonsoft.Json;
using Common;
using HarmonyLib;
using System.IO;
using System;
using UnityEngine;

namespace PipedEverything
{
    public class PipedEverythingState
    {
        public int version { get; set; } = 1;

        public List<PipeConfig> Configs { get; set; } = new()
        {
            new PipeConfig() { Id = ElectrolyzerConfig.ID, OffsetX = 0, OffsetY = 1, Filter = new string[] { SimHashes.Oxygen.ToString() }, Color = Color.blue },
            new PipeConfig() { Id = ElectrolyzerConfig.ID, OffsetX = 1, OffsetY = 1, Filter = new string[] { SimHashes.Hydrogen.ToString() }, Color = Color.magenta },
        };

        #region _implementation

        public static Config.Manager<PipedEverythingState> StateManager;

        public static void BeforeUpdate()
        {
        }

        public static bool OnUpdate(PipedEverythingState state)
        {
            return true;
        }

        public object ReadSettings()
        {
            return StateManager.State;
        }

        public void WriteSettings(object settings)
        {
            if (settings is PipedEverythingState state)
                StateManager.TrySaveConfigurationState(state);
            else
                StateManager.TrySaveConfigurationState();
        }

        public string GetConfigPath()
        {
            return GetStaticConfigPath();
        }

        public static string GetStaticConfigPath()
        {
            string path = FumiKMod.ModName;
            if (Helpers.ActiveLocale.NotEmpty() && Helpers.ActiveLocale != "en")
                path += "_" + Helpers.ActiveLocale;
            return Config.PathHelper.CreatePath(path);
        }

        #endregion
    }
}
