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

        [Option("Min Number Of Interests", "Any new dupe will have at least this number of interests")]
        [Limit(0, 3)]
        public int minNumberofInterests { get; set; } = 1;
        
        [Option("Number Of Interests", "Any new dupe will have at most this number of interests")]
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

        [Option("Load packages from config file", "If false will not apply any logic to care packages (use this if you have other mods managing packages).")]
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
            #region $Settings
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.biggerRoster_Title", "Change amount");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.biggerRoster_ToolTip", "If true apply amount changes. Set to false, if mod crashes. Game must be restarted to apply setting!");

            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.rosterDupes_Title", "Amount of Duplicants");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.rosterDupes_ToolTip", "Best less than 6 packages total.");

            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.rosterPackages_Title", "Amount of Care-Packages");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.rosterPackages_ToolTip", "Best less than 6 packages total.");

            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.attributeBonusChance_Title", "Attribute Bonus Chance");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.attributeBonusChance_ToolTip", "Positive number increases overall attributes, negative number reduces them.");

            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.removeStarterRestriction_Title", "Remove Starter Restriction");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.removeStarterRestriction_ToolTip", "If true duplicants can start with more than 2 traits and more names are available.");

            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.allowReshuffle_Title", "Allow Reshuffle");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.allowReshuffle_ToolTip", "If true shows reroll button when using the printing pot (duplicants only).");

            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.rosterIsOrdered_Title", "In Order");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.rosterIsOrdered_ToolTip", "(Only DLC) If true duplicants and care packages will be ordered by type. Game must be restarted to apply setting!");

            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.multiplier_Title", "Multiplier");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.multiplier_ToolTip", "Multiply all care packages by amount.");

            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.labelPackages_Title", "Care Packages must be edited manually");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.labelPackages_ToolTip", "Open config file to edit Care Packages.");

            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.allowOnlyDiscoveredElements_Title", "Allow Only Discovered Elements");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.allowOnlyDiscoveredElements_ToolTip", "If true, only elements / items that were discovered at least once will show up in care packages.");
            #endregion
        }
    }
}