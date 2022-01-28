//#define LOCALE
using UnityEngine;
using PeterHan.PLib.Options;
using Common;
using System.IO;

namespace CarePackageMod
{
    [ConfigFile("Care Package Manager.json", true, true, typeof(Config.TranslationResolver))]
    public class CarePackageState
    {
        public int version { get; set; } = 14;

        #region $Settings
        [Option("CarePackageMod.LOCSTRINGS.biggerRoster_Title", "CarePackageMod.LOCSTRINGS.biggerRoster_ToolTip")]
        public bool biggerRoster { get; set; } = true;

        [Option("CarePackageMod.LOCSTRINGS.rosterDupes_Title", "CarePackageMod.LOCSTRINGS.rosterDupes_ToolTip")]
        [Limit(0, 6)]
        public int rosterDupes { get; set; } = 3;

        [Option("CarePackageMod.LOCSTRINGS.rosterPackages_Title", "CarePackageMod.LOCSTRINGS.rosterPackages_ToolTip")]
        [Limit(0, 6)]
        public int rosterPackages { get; set; } = 3;

        [Option("CarePackageMod.LOCSTRINGS.attributeBonusChance_Title", "CarePackageMod.LOCSTRINGS.attributeBonusChance_ToolTip")]
        public int attributeBonusChance { get; set; } = 0;

        [Option("CarePackageMod.LOCSTRINGS.minNumberofInterests_Title", "CarePackageMod.LOCSTRINGS.minNumberofInterests_ToolTip", "", null)]
        [Limit(0, 3)]
        public int minNumberofInterests { get; set; } = 1;
        
        [Option("CarePackageMod.LOCSTRINGS.maxNumberofInterests_Title", "CarePackageMod.LOCSTRINGS.maxNumberofInterests_ToolTip", "", null)]
        [Limit(0, 3)]
        public int maxNumberofInterests { get; set; } = 3;

        [Option("CarePackageMod.LOCSTRINGS.removeStarterRestriction_Title", "CarePackageMod.LOCSTRINGS.removeStarterRestriction_ToolTip")]
        public bool removeStarterRestriction { get; set; } = true;

        [Option("CarePackageMod.LOCSTRINGS.allowReshuffle_Title", "CarePackageMod.LOCSTRINGS.allowReshuffle_ToolTip")]
        public bool allowReshuffle { get; set; } = false;

        [Option("CarePackageMod.LOCSTRINGS.rosterIsOrdered_Title", "CarePackageMod.LOCSTRINGS.rosterIsOrdered_ToolTip")]
        public bool rosterIsOrdered { get; set; } = false;

        [Option("CarePackageMod.LOCSTRINGS.multiplier_Title", "CarePackageMod.LOCSTRINGS.multiplier_ToolTip")]
        public float multiplier { get; set; } = 1f;

        [Option("CarePackageMod.LOCSTRINGS.allowOnlyDiscoveredElements_Title", "CarePackageMod.LOCSTRINGS.allowOnlyDiscoveredElements_ToolTip")]
        public bool allowOnlyDiscoveredElements { get; set; } = false;

        [Option("CarePackageMod.LOCSTRINGS.labelPackages_Title", "CarePackageMod.LOCSTRINGS.labelPackages_ToolTip")]
        public LocString labelPackages { get; set; }

        [Option("CarePackageMod.LOCSTRINGS.loadPackages_Title", "CarePackageMod.LOCSTRINGS.loadPackages_ToolTip", "", null)]
        public bool loadPackages { get; set; } = true;

        public CarePackageContainer[] CarePackages { get; set; } = CarePackageList.GetPackages();
        #endregion

        public static Config.Manager<CarePackageState> StateManager = new Config.Manager<CarePackageState>(Config.PathHelper.CreatePath("Care Package Manager"), true, UpdateFunction);

        public static bool UpdateFunction(CarePackageState state)
        {
            if (state.version < 13)
            {
                string modpath = Path.Combine(Config.PathHelper.ModsDirectory, "config", "Care Package Manager.json");
                if (File.Exists(modpath))
                    File.Delete(modpath);
                modpath = Path.Combine(Config.PathHelper.ModsDirectory, "config", "CarePackageModMerged");
                if (Directory.Exists(modpath))
                    Directory.Delete(modpath, true);
            }
            return true;
        }
    }

    public class CustomStrings
    {
        public static void LoadStrings()
        {
            #region 
            Helpers.StringsAddProperty("CarePackageMod.PROPERTY.version", "version");

            Helpers.StringsAddProperty("CarePackageMod.PROPERTY.biggerRoster", "biggerRoster");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.biggerRoster_Title", "Change amount");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.biggerRoster_ToolTip", "If true apply amount changes. Set to false, if mod crashes. Game must be restarted to apply setting!");

            Helpers.StringsAddProperty("CarePackageMod.PROPERTY.rosterDupes", "rosterDupes");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.rosterDupes_Title", "Amount of Duplicants");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.rosterDupes_ToolTip", "Best less than 6 packages total.");

            Helpers.StringsAddProperty("CarePackageMod.PROPERTY.rosterPackages", "rosterPackages");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.rosterPackages_Title", "Amount of Care-Packages");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.rosterPackages_ToolTip", "Best less than 6 packages total.");

            Helpers.StringsAddProperty("CarePackageMod.PROPERTY.attributeBonusChance", "attributeBonusChance");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.attributeBonusChance_Title", "Attribute Bonus Chance");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.attributeBonusChance_ToolTip", "Positive number increases overall attributes, negative number reduces them.");

            Helpers.StringsAddProperty("CarePackageMod.PROPERTY.minNumberofInterests", "minNumberofInterests");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.minNumberofInterests_Title", "Min Number Of Interests");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.minNumberofInterests_ToolTip", "Any new dupe will have at least this number of interests");

            Helpers.StringsAddProperty("CarePackageMod.PROPERTY.maxNumberofInterests", "maxNumberofInterests");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.maxNumberofInterests_Title", "Max Number Of Interests");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.maxNumberofInterests_ToolTip", "Any new dupe will have at most this number of interests");

            Helpers.StringsAddProperty("CarePackageMod.PROPERTY.removeStarterRestriction", "removeStarterRestriction");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.removeStarterRestriction_Title", "Remove Starter Restriction");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.removeStarterRestriction_ToolTip", "If true duplicants can start with more than 2 traits and more names are available.");

            Helpers.StringsAddProperty("CarePackageMod.PROPERTY.allowReshuffle", "allowReshuffle");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.allowReshuffle_Title", "Allow Reshuffle");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.allowReshuffle_ToolTip", "If true shows reroll button when using the printing pot (duplicants only).");

            Helpers.StringsAddProperty("CarePackageMod.PROPERTY.rosterIsOrdered", "rosterIsOrdered");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.rosterIsOrdered_Title", "In Order");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.rosterIsOrdered_ToolTip", "(Only DLC) If true duplicants and care packages will be ordered by type. Game must be restarted to apply setting!");

            Helpers.StringsAddProperty("CarePackageMod.PROPERTY.multiplier", "multiplier");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.multiplier_Title", "Multiplier");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.multiplier_ToolTip", "Multiply all care packages by amount.");

            Helpers.StringsAddProperty("CarePackageMod.PROPERTY.allowOnlyDiscoveredElements", "allowOnlyDiscoveredElements");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.allowOnlyDiscoveredElements_Title", "Allow Only Discovered Elements");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.allowOnlyDiscoveredElements_ToolTip", "If true, only elements / items that were discovered at least once will show up in care packages.");

            Helpers.StringsAddProperty("CarePackageMod.PROPERTY.labelPackages", "labelPackages");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.labelPackages_Title", "Care Packages must be edited manually");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.labelPackages_ToolTip", "Open config file to edit Care Packages.");

            Helpers.StringsAddProperty("CarePackageMod.PROPERTY.loadPackages", "loadPackages");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.loadPackages_Title", "Load packages from config file");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.loadPackages_ToolTip", "If false will not apply any logic to care packages (use this if you have other mods managing packages).");
            #endregion
        }
    }
}
