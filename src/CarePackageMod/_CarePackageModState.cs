//#define LOCALE
using System.Collections.Generic;
using UnityEngine;
using System;
using PeterHan.PLib.Options;
using Common;

namespace CarePackageMod
{
    [PeterHan.PLib.Options.ConfigFile("Care Package Manager.json", true, true)]
    public class CarePackageState
    {
        public static void OnLoad()
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

            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.always3Interests_Title", "Always 3 Interests");
            Helpers.StringsAdd("CarePackageMod.LOCSTRINGS.always3Interests_ToolTip", "If true will always roll 3 interests.");

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
            #endregion
#if LOCALE
            Helpers.StringsPrint();
#else
            Helpers.StringsLoad();
#endif
            POptions.RegisterOptions(typeof(CarePackageState));
        }

        public int version { get; set; } = 8;

        public bool enabled { get; set; } = true;

        #region $Settings
        [PeterHan.PLib.Option("CarePackageMod.LOCSTRINGS.OPTIONS.biggerRoster_Title", "CarePackageMod.LOCSTRINGS.OPTIONS.biggerRoster_ToolTip")]
        public bool biggerRoster { get; set; } = true;

        [PeterHan.PLib.Option("CarePackageMod.LOCSTRINGS.OPTIONS.rosterDupes_Title", "CarePackageMod.LOCSTRINGS.OPTIONS.rosterDupes_ToolTip")]
        [PeterHan.PLib.Limit(0, 6)]
        public int rosterDupes { get; set; } = 3;

        [PeterHan.PLib.Option("CarePackageMod.LOCSTRINGS.OPTIONS.rosterPackages_Title", "CarePackageMod.LOCSTRINGS.OPTIONS.rosterPackages_ToolTip")]
        [PeterHan.PLib.Limit(0, 6)]
        public int rosterPackages { get; set; } = 3;

        [PeterHan.PLib.Option("CarePackageMod.LOCSTRINGS.OPTIONS.attributeBonusChance_Title", "CarePackageMod.LOCSTRINGS.OPTIONS.attributeBonusChance_ToolTip")]
        public int attributeBonusChance { get; set; } = 0;

        [PeterHan.PLib.Option("CarePackageMod.LOCSTRINGS.OPTIONS.always3Interests_Title", "CarePackageMod.LOCSTRINGS.OPTIONS.always3Interests_ToolTip")]
        public bool always3Interests { get; set; } = true;

        [PeterHan.PLib.Option("CarePackageMod.LOCSTRINGS.OPTIONS.removeStarterRestriction_Title", "CarePackageMod.LOCSTRINGS.OPTIONS.removeStarterRestriction_ToolTip")]
        public bool removeStarterRestriction { get; set; } = true;

        [PeterHan.PLib.Option("CarePackageMod.LOCSTRINGS.OPTIONS.allowReshuffle_Title", "CarePackageMod.LOCSTRINGS.OPTIONS.allowReshuffle_ToolTip")]
        public bool allowReshuffle { get; set; } = false;

        [PeterHan.PLib.Option("CarePackageMod.LOCSTRINGS.OPTIONS.rosterIsOrdered_Title", "CarePackageMod.LOCSTRINGS.OPTIONS.rosterIsOrdered_ToolTip")]
        public bool rosterIsOrdered { get; set; } = false;

        [PeterHan.PLib.Option("CarePackageMod.LOCSTRINGS.OPTIONS.multiplier_Title", "CarePackageMod.LOCSTRINGS.OPTIONS.multiplier_ToolTip")]
        public float multiplier { get; set; } = 1f;


        [PeterHan.PLib.Option("CarePackageMod.LOCSTRINGS.OPTIONS.labelPackages_Title", "CarePackageMod.LOCSTRINGS.OPTIONS.labelPackages_ToolTip")]
        public LocString labelPackages { get; set; }

        public CarePackageContainer[] CarePackages { get; set; } = CarePackageList.GetPackages();
        #endregion

        public static Config.Manager<CarePackageState> StateManager = new Config.Manager<CarePackageState>(Config.PathHelper.CreatePath("Care Package Manager"), true, UpdateFunction);

        public static bool UpdateFunction(CarePackageState state)
        {
            return true;
        }
    }

    public class CarePackageContainer
    {
        public string ID;
        public float amount;
        public int? onlyAfterCycle;
        public int? onlyUntilCycle;
        [NonSerialized]
        public float multiplier = 1f;

        public CarePackageContainer(string ID, float amount, int? onlyAfterCycle = null, int? onlyUntilCycle = null)
        {
            this.ID = ID;
            this.amount = amount;
            this.onlyAfterCycle = onlyAfterCycle;
            this.onlyUntilCycle = onlyUntilCycle;
        }

        public CarePackageInfo ToInfo()
        {
            float amount = (float)Math.Max(Math.Round(this.amount * this.multiplier, 0), 1.0);
            return new CarePackageInfo(this.ID, amount, (() => CycleCondition(onlyAfterCycle, onlyUntilCycle)));
        }

        public static implicit operator CarePackageInfo(CarePackageContainer container)
        {
            return container.ToInfo();
        }

        public static implicit operator CarePackageContainer(CarePackageInfo info)
        {
            return new CarePackageContainer(info.id, info.quantity, 0);
        }

        public static bool CycleCondition(int? afterCycle = null, int? untilCycle = null)
        {
            if (afterCycle == null) afterCycle = 0;
            if (untilCycle == null) untilCycle = int.MaxValue;
            return GameClock.Instance.GetCycle() > afterCycle && GameClock.Instance.GetCycle() <= untilCycle;
        }
    }

    public class CarePackageList
    {
        public static CarePackageContainer[] GetPackages()
        {
            if (!DlcManager.IsExpansion1Installed())
            {
                List<CarePackageContainer> result = new List<CarePackageContainer>
                {
                    new CarePackageContainer("Niobium", 5f),
                    new CarePackageContainer("Isoresin", 35f),
                    new CarePackageContainer("Fullerene", 1f),
                    new CarePackageContainer("Katairite", 1000f),
                    new CarePackageContainer("Diamond", 500f),
                    new CarePackageContainer(GeneShufflerRechargeConfig.ID, 1f),

                    new CarePackageContainer("SandStone", 1000f),
                    new CarePackageContainer("Dirt", 500f),
                    new CarePackageContainer("Algae", 500f),
                    new CarePackageContainer("OxyRock", 100f),
                    new CarePackageContainer("Water", 2000f),
                    new CarePackageContainer("Sand", 3000f),
                    new CarePackageContainer("Carbon", 3000f),
                    new CarePackageContainer("Fertilizer", 3000f),
                    new CarePackageContainer("Ice", 4000f),
                    new CarePackageContainer("Brine", 2000f),
                    new CarePackageContainer("SaltWater", 2000f),
                    new CarePackageContainer("Rust", 1000f),
                    new CarePackageContainer("Cuprite", 2000f),
                    new CarePackageContainer("GoldAmalgam", 2000f),
                    new CarePackageContainer("Copper", 400f),
                    new CarePackageContainer("Iron", 400f),
                    new CarePackageContainer("Lime", 150f),
                    new CarePackageContainer("Polypropylene", 500f),
                    new CarePackageContainer("Glass", 200f),
                    new CarePackageContainer("Steel", 100f),
                    new CarePackageContainer("Ethanol", 100f),
                    new CarePackageContainer("AluminumOre", 100f),
                    new CarePackageContainer("PrickleGrassSeed", 3f),
                    new CarePackageContainer("LeafyPlantSeed", 3f),
                    new CarePackageContainer("CactusPlantSeed", 3f),
                    new CarePackageContainer("MushroomSeed", 1f),
                    new CarePackageContainer("PrickleFlowerSeed", 2f),
                    new CarePackageContainer("OxyfernSeed", 1f),
                    new CarePackageContainer("ForestTreeSeed", 1f),
                    new CarePackageContainer(BasicFabricMaterialPlantConfig.SEED_ID, 3f),
                    new CarePackageContainer("SwampLilySeed", 1f),
                    new CarePackageContainer("ColdBreatherSeed", 1f),
                    new CarePackageContainer("SpiceVineSeed", 1f),
                    new CarePackageContainer("FieldRation", 5f),
                    new CarePackageContainer("BasicForagePlant", 6f),
                    new CarePackageContainer("CookedEgg", 3f),
                    new CarePackageContainer(PrickleFruitConfig.ID, 3f),
                    new CarePackageContainer("FriedMushroom", 3f),
                    new CarePackageContainer("CookedMeat", 3f),
                    new CarePackageContainer("SpicyTofu", 3f),
                    new CarePackageContainer("LightBugBaby", 1f),
                    new CarePackageContainer("HatchBaby", 1f),
                    new CarePackageContainer("PuftBaby", 1f),
                    new CarePackageContainer("SquirrelBaby", 1f),
                    new CarePackageContainer("CrabBaby", 1f),
                    new CarePackageContainer("DreckoBaby", 1f),
                    new CarePackageContainer("Pacu", 8f),
                    new CarePackageContainer("MoleBaby", 1f),
                    new CarePackageContainer("OilfloaterBaby", 1f),
                    new CarePackageContainer("LightBugEgg", 3f),
                    new CarePackageContainer("HatchEgg", 3f),
                    new CarePackageContainer("PuftEgg", 3f),
                    new CarePackageContainer("OilfloaterEgg", 3f),
                    new CarePackageContainer("MoleEgg", 3f),
                    new CarePackageContainer("DreckoEgg", 3f),
                    new CarePackageContainer("SquirrelEgg", 2f),
                    new CarePackageContainer("BasicCure", 3f),
                    new CarePackageContainer("Funky_Vest", 1f),
                };

                return result.ToArray();
            }
            else
            {
                List<CarePackageContainer> result = new List<CarePackageContainer>
                {
                    new CarePackageContainer("Niobium", 5f),
                    new CarePackageContainer("Isoresin", 35f),
                    new CarePackageContainer("Fullerene", 1f),
                    new CarePackageContainer("Katairite", 1000f),
                    new CarePackageContainer("Diamond", 500f),
                    new CarePackageContainer(GeneShufflerRechargeConfig.ID, 1f),

                    new CarePackageContainer("SandStone", 1000f),
                    new CarePackageContainer("Dirt", 500f),
                    new CarePackageContainer("Algae", 500f),
                    new CarePackageContainer("OxyRock", 100f),
                    new CarePackageContainer("Water", 2000f),
                    new CarePackageContainer("Sand", 3000f),
                    new CarePackageContainer("Carbon", 3000f),
                    new CarePackageContainer("Fertilizer", 3000f),
                    new CarePackageContainer("Ice", 4000f),
                    new CarePackageContainer("Brine", 2000f),
                    new CarePackageContainer("SaltWater", 2000f),
                    new CarePackageContainer("Rust", 1000f),
                    new CarePackageContainer("Cuprite", 2000f),
                    new CarePackageContainer("GoldAmalgam", 2000f),
                    new CarePackageContainer("Copper", 400f),
                    new CarePackageContainer("Iron", 400f),
                    new CarePackageContainer("Lime", 150f),
                    new CarePackageContainer("Polypropylene", 500f),
                    new CarePackageContainer("Glass", 200f),
                    new CarePackageContainer("Steel", 100f),
                    new CarePackageContainer("Ethanol", 100f),
                    new CarePackageContainer("AluminumOre", 100f),
                    new CarePackageContainer("PrickleGrassSeed", 3f),
                    new CarePackageContainer("LeafyPlantSeed", 3f),
                    new CarePackageContainer("CactusPlantSeed", 3f),
                    new CarePackageContainer("MushroomSeed", 3f),
                    new CarePackageContainer("PrickleFlowerSeed", 3f),
                    new CarePackageContainer("OxyfernSeed", 1f),
                    new CarePackageContainer("ForestTreeSeed", 1f),
                    new CarePackageContainer(BasicFabricMaterialPlantConfig.SEED_ID, 3f),
                    new CarePackageContainer("SwampLilySeed", 1f),
                    new CarePackageContainer("ColdBreatherSeed", 1f),
                    new CarePackageContainer("SpiceVineSeed", 1f),
                    new CarePackageContainer("FieldRation", 5f),
                    new CarePackageContainer("BasicForagePlant", 6f),
                    new CarePackageContainer("ForestForagePlant", 2f),
                    new CarePackageContainer("SwampForagePlant", 2f),
                    new CarePackageContainer("CookedEgg", 3f),
                    new CarePackageContainer(PrickleFruitConfig.ID, 3f),
                    new CarePackageContainer("FriedMushroom", 3f),
                    new CarePackageContainer("CookedMeat", 3f),
                    new CarePackageContainer("SpicyTofu", 3f),
                    new CarePackageContainer("WormSuperFood", 2f),
                    new CarePackageContainer("LightBugBaby", 1f),
                    new CarePackageContainer("HatchBaby", 1f),
                    new CarePackageContainer("PuftBaby", 1f),
                    new CarePackageContainer("SquirrelBaby", 1f),
                    new CarePackageContainer("CrabBaby", 1f),
                    new CarePackageContainer("DreckoBaby", 1f),
                    new CarePackageContainer("Pacu", 8f),
                    new CarePackageContainer("MoleBaby", 1f),
                    new CarePackageContainer("OilfloaterBaby", 1f),
                    new CarePackageContainer("DivergentBeetleBaby", 1f),
                    new CarePackageContainer("StaterpillarBaby", 1f),
                    new CarePackageContainer("LightBugEgg", 3f),
                    new CarePackageContainer("HatchEgg", 3f),
                    new CarePackageContainer("PuftEgg", 3f),
                    new CarePackageContainer("OilfloaterEgg", 3f),
                    new CarePackageContainer("MoleEgg", 3f),
                    new CarePackageContainer("DreckoEgg", 3f),
                    new CarePackageContainer("SquirrelEgg", 2f),
                    new CarePackageContainer("DivergentBeetleEgg", 2f),
                    new CarePackageContainer("StaterpillarEgg", 2f),
                    new CarePackageContainer("BasicCure", 3f),
                    new CarePackageContainer("Funky_Vest", 1f),
                };

                return result.ToArray();
            }
        }
    }
}