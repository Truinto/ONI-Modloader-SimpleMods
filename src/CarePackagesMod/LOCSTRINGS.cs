using System.Collections.Generic;
using UnityEngine;
using System;
using PeterHan.PLib.Options;

namespace CarePackageMod
{
    public static class LOCSTRINGS
    {
        public static class OPTIONS
        {
            #region 
            public static LocString biggerRoster_Title = new LocString("Change amount");
            public static LocString biggerRoster_ToolTip = new LocString("If true apply amount changes. Set to false, if mod crashes. Game must be restarted to apply setting!");

            public static LocString rosterDupes_Title = new LocString("Amount of Duplicants");
            public static LocString rosterDupes_ToolTip = new LocString("Best less than 6 packages total.");

            public static LocString rosterPackages_Title = new LocString("Amount of Care-Packages");
            public static LocString rosterPackages_ToolTip = new LocString("Best less than 6 packages total.");

            public static LocString attributeBonusChance_Title = new LocString("Attribute Bonus Chance");
            public static LocString attributeBonusChance_ToolTip = new LocString("Positive number increases overall attributes, negative number reduces them.");

            public static LocString always3Interests_Title = new LocString("Always 3 Interests");
            public static LocString always3Interests_ToolTip = new LocString("If true will always roll 3 interests.");

            public static LocString removeStarterRestriction_Title = new LocString("Remove Starter Restriction");
            public static LocString removeStarterRestriction_ToolTip = new LocString("If true duplicants can start with more than 2 traits and more names are available.");

            public static LocString allowReshuffle_Title = new LocString("Allow Reshuffle");
            public static LocString allowReshuffle_ToolTip = new LocString("If true shows reroll button when using the printing pot (duplicants only).");

            public static LocString rosterIsOrdered_Title = new LocString("In Order");
            public static LocString rosterIsOrdered_ToolTip = new LocString("(Only DLC) If true duplicants and care packages will be ordered by type. Game must be restarted to apply setting!");

            public static LocString multiplier_Title = new LocString("Multiplier");
            public static LocString multiplier_ToolTip = new LocString("Multiply all care packages by amount.");

            public static LocString labelPackages_Title = new LocString("Care Packages must be edited manually");
            public static LocString labelPackages_ToolTip = new LocString("Open config file to edit Care Packages.");
            #endregion
        }
    }
}