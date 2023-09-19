//#define LOCALE
using System.Collections.Generic;
using Newtonsoft.Json;
using Common;
using HarmonyLib;
using PeterHan.PLib.Options;
using System.IO;
using System;

namespace SharedStorage
{
    [ConfigFile("SharedStorage.json", true, true)]
    [ModInfo(null, collapse: false)]
    public class SharedStorageState : IManualConfig
    {
        public int version { get; set; } = 1;

        //[JsonIgnore]
        //[Option("Title", "Tooltip", "", null)]
        //public System.Action<object> Button => delegate (object nix)
        //{
        //};

        [Option("Enabled", "If false, stops buildings from trading their contents.", "", null)]
        public bool Enabled { get; set; } = false;

        #region _implementation

        public static Config.Manager<SharedStorageState> StateManager;

        public static void BeforeUpdate()
        {
        }

        public static bool OnUpdate(SharedStorageState state)
        {
            return true;
        }

        public object ReadSettings()
        {
            return StateManager.State;
        }

        public void WriteSettings(object settings)
        {
            if (settings is SharedStorageState state)
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

    public class CustomStrings
    {
        public static void LoadStrings()
        {
          
        }
    }
}
