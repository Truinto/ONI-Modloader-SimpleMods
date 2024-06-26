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

        [Option("SharedStorage.LOCSTRINGS.Enabled_Title", "SharedStorage.LOCSTRINGS.Enabled_ToolTip", null, null)]
        public bool Enabled { get; set; } = true;

        [Option("SharedStorage.LOCSTRINGS.AllowCrossWorld_Title", "SharedStorage.LOCSTRINGS.AllowCrossWorld_ToolTip", null, null)]
        public bool AllowCrossWorld { get; set; } = false;

        [Option("SharedStorage.LOCSTRINGS.OnlySamePriority_Title", "SharedStorage.LOCSTRINGS.OnlySamePriority_ToolTip", null, null)]
        public bool OnlySamePriority { get; set; } = true;

        [Option("SharedStorage.LOCSTRINGS.AcceptInputAnywhere_Title", "SharedStorage.LOCSTRINGS.AcceptInputAnywhere_ToolTip", "", null)]
        public bool AcceptInputAnywhere { get; set; } = true;

        [Option("SharedStorage.LOCSTRINGS.MinGeneral_Title", "SharedStorage.LOCSTRINGS.MinGeneral_ToolTip", null, "F0")]
        public float MinGeneral { get; set; } = 1000f;

        [Option("SharedStorage.LOCSTRINGS.MinFood_Title", "SharedStorage.LOCSTRINGS.MinFood_ToolTip", null, "F0")]
        public float MinFood { get; set; } = 3f;

        [Option("SharedStorage.LOCSTRINGS.MinClothes_Title", "SharedStorage.LOCSTRINGS.MinClothes_ToolTip", null, "F0")]
        public float MinClothes { get; set; } = 3f;

        [Option("SharedStorage.LOCSTRINGS.RefreshRate_Title", "SharedStorage.LOCSTRINGS.RefreshRate_ToolTip", null, null)]
        [Limit(1, 10)]
        public int RefreshRate { get; set; } = 5;

        public HashSet<string> Blacklist { get; set; } = new() 
        {
            SimHashes.UraniumOre.ToString(),
            SimHashes.DepletedUranium.ToString(),
            SimHashes.EnrichedUranium.ToString(),
        };

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
            //if (Helpers.ActiveLocale.NotEmpty() && Helpers.ActiveLocale != "en")
            //    path += "_" + Helpers.ActiveLocale;
            return Config.PathHelper.CreatePath(path);
        }
        
        #endregion
    }

    public class CustomStrings
    {
        public static void LoadStrings()
        {
            #region 
            Helpers.StringsAddProperty("SharedStorage.PROPERTY.version", "version");

            Helpers.StringsAddProperty("SharedStorage.PROPERTY.Enabled", "Enabled");
            Helpers.StringsAdd("SharedStorage.LOCSTRINGS.Enabled_Title", "Enabled");
            Helpers.StringsAdd("SharedStorage.LOCSTRINGS.Enabled_ToolTip", "If false, stops the background task from transfering items.");

            Helpers.StringsAddProperty("SharedStorage.PROPERTY.AllowCrossWorld", "AllowCrossWorld");
            Helpers.StringsAdd("SharedStorage.LOCSTRINGS.AllowCrossWorld_Title", "Allow Cross World");
            Helpers.StringsAdd("SharedStorage.LOCSTRINGS.AllowCrossWorld_ToolTip", "Allow storages to transfer items across worlds.");

            Helpers.StringsAddProperty("SharedStorage.PROPERTY.OnlySamePriority", "OnlySamePriority");
            Helpers.StringsAdd("SharedStorage.LOCSTRINGS.OnlySamePriority_Title", "Only same priority");
            Helpers.StringsAdd("SharedStorage.LOCSTRINGS.OnlySamePriority_ToolTip", "If false, allow transfer between any storages. If true, allow transfer to equal priority buildings only.");

            Helpers.StringsAddProperty("SharedStorage.PROPERTY.AcceptInputAnywhere", "AcceptInputAnywhere");
            Helpers.StringsAdd("SharedStorage.LOCSTRINGS.AcceptInputAnywhere_Title", "Accept Input Anywhere");
            Helpers.StringsAdd("SharedStorage.LOCSTRINGS.AcceptInputAnywhere_ToolTip", "If true, any storage can accept any item. Misplaced items are transfered to other storages.");

            Helpers.StringsAddProperty("SharedStorage.PROPERTY.MinGeneral", "MinGeneral");
            Helpers.StringsAdd("SharedStorage.LOCSTRINGS.MinGeneral_Title", "Split General");
            Helpers.StringsAdd("SharedStorage.LOCSTRINGS.MinGeneral_ToolTip", "Minimum amount of items to move.");

            Helpers.StringsAddProperty("SharedStorage.PROPERTY.MinFood", "MinFood");
            Helpers.StringsAdd("SharedStorage.LOCSTRINGS.MinFood_Title", "Split Food");
            Helpers.StringsAdd("SharedStorage.LOCSTRINGS.MinFood_ToolTip", "Minimum amount of food to move.");

            Helpers.StringsAddProperty("SharedStorage.PROPERTY.MinClothes", "MinClothes");
            Helpers.StringsAdd("SharedStorage.LOCSTRINGS.MinClothes_Title", "Split Clothes");
            Helpers.StringsAdd("SharedStorage.LOCSTRINGS.MinClothes_ToolTip", "Minimum amount of clothing to move.");

            Helpers.StringsAddProperty("SharedStorage.PROPERTY.RefreshRate", "RefreshRate");
            Helpers.StringsAdd("SharedStorage.LOCSTRINGS.RefreshRate_Title", "Refresh Rate");
            Helpers.StringsAdd("SharedStorage.LOCSTRINGS.RefreshRate_ToolTip", "How often storages try to transfer items. Lower is more often.");

            Helpers.StringsAddProperty("SharedStorage.PROPERTY.Blacklist", "Blacklist");
            #endregion
        }

        public static void ExtraStrings()
        {
            Helpers.StringsAdd("SharedStorage.LOCSTRINGS.ButtonOption", "SharedStorage Option");
        }
    }
}
