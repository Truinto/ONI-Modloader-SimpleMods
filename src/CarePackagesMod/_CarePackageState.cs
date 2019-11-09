using System.Collections.Generic;
using UnityEngine;
using System;

namespace CarePackageMod
{
    public class CarePackageState
    {
        public int version { get; set; } = 6;

        public bool enabled { get; set; } = true;

        public bool biggerRoster { get; set; } = true;

        public float multiplier { get; set; } = 1f;

        public bool rosterIsOrdered { get; set; } = false;

        public CarePackageContainer[] CarePackages { get; set; } = CarePackageList.GetPackages();


        //public static BaseStateManager<CarePackageState> StateManager = new BaseStateManager<CarePackageState>(new ModFolderPathHelper("CarePackageMod", 1833878154L).path, ONI_Common.Paths.GetLogsPath() + ModFolderPathHelper.sep + "CarePackageModLog.txt");
        public static Config.Manager<CarePackageState> StateManager = new Config.Manager<CarePackageState>(Config.Helper.CreatePath("Care Package Manager"), true);
    }

    public class CarePackageContainer
    {
        public string ID;
        public float amount;
        public int? onlyAfterCycle;
        public int? onlyUntilCycle;

        public CarePackageContainer(string ID, float amount, int? onlyAfterCycle = null, int? onlyUntilCycle = null)
        {
            this.ID = ID;
            this.amount = amount;
            this.onlyAfterCycle = onlyAfterCycle;
            this.onlyUntilCycle = onlyUntilCycle;
        }

        public CarePackageInfo ToInfo()
        {
            return new CarePackageInfo(this.ID, this.amount, (() => CycleCondition(onlyAfterCycle, onlyUntilCycle)));
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

    internal class CarePackageList
    {
        internal static CarePackageContainer[] GetPackages()
        {
            List<CarePackageContainer> result = new List<CarePackageContainer>();
            
            result.Add(new CarePackageContainer("Algae", 1000f, 0, 4));

            result.Add(new CarePackageContainer("Niobium", 5f));
            result.Add(new CarePackageContainer("Isoresin", 35f));
            result.Add(new CarePackageContainer("Fullerene", 1f));
            result.Add(new CarePackageContainer("Katairite", 1000f));
            result.Add(new CarePackageContainer("Diamond", 500f));
            result.Add(new CarePackageContainer(GeneShufflerRechargeConfig.ID, 1f));


            result.Add(new CarePackageContainer("SandStone", 1000f));
            result.Add(new CarePackageContainer("Dirt", 500f));
            result.Add(new CarePackageContainer("Algae", 500f));
            result.Add(new CarePackageContainer("OxyRock", 100f));
            result.Add(new CarePackageContainer("Water", 2000f));
            result.Add(new CarePackageContainer("Sand", 3000f));
            result.Add(new CarePackageContainer("Carbon", 3000f));
            result.Add(new CarePackageContainer("Fertilizer", 3000f));
            result.Add(new CarePackageContainer("Ice", 4000f, 12));
            result.Add(new CarePackageContainer("Brine", 2000f));
            result.Add(new CarePackageContainer("SaltWater", 2000f));
            result.Add(new CarePackageContainer("Rust", 1000f));
            result.Add(new CarePackageContainer("Cuprite", 2000f, 12));
            result.Add(new CarePackageContainer("GoldAmalgam", 2000f, 12));
            result.Add(new CarePackageContainer("Copper", 400f, 24));
            result.Add(new CarePackageContainer("Iron", 400f, 24));
            result.Add(new CarePackageContainer("Lime", 150f, 48));
            result.Add(new CarePackageContainer("Polypropylene", 500f, 48));
            result.Add(new CarePackageContainer("Glass", 200f, 48));
            result.Add(new CarePackageContainer("Steel", 100f, 48));
            result.Add(new CarePackageContainer("Ethanol", 100f, 48));
            result.Add(new CarePackageContainer("AluminumOre", 100f, 48));
            result.Add(new CarePackageContainer("PrickleGrassSeed", 3f));
            result.Add(new CarePackageContainer("LeafyPlantSeed", 3f));
            result.Add(new CarePackageContainer("CactusPlantSeed", 3f));
            result.Add(new CarePackageContainer("MushroomSeed", 1f));
            result.Add(new CarePackageContainer("PrickleFlowerSeed", 2f));
            result.Add(new CarePackageContainer("OxyfernSeed", 1f));
            result.Add(new CarePackageContainer("ForestTreeSeed", 1f));
            result.Add(new CarePackageContainer("BasicFabricMaterialPlantSeed", 3f, 24));
            result.Add(new CarePackageContainer("SwampLilySeed", 1f, 24));
            result.Add(new CarePackageContainer("ColdBreatherSeed", 1f, 24));
            result.Add(new CarePackageContainer("SpiceVineSeed", 1f, 24));
            result.Add(new CarePackageContainer("FieldRation", 5f));
            result.Add(new CarePackageContainer("BasicForagePlant", 6f));
            result.Add(new CarePackageContainer("CookedEgg", 3f, 6));
            result.Add(new CarePackageContainer("PrickleFruit", 3f, 12));
            result.Add(new CarePackageContainer("FriedMushroom", 3f, 24));
            result.Add(new CarePackageContainer("CookedMeat", 3f, 48));
            result.Add(new CarePackageContainer("SpicyTofu", 3f, 48));
            result.Add(new CarePackageContainer("LightBugBaby", 1f));
            result.Add(new CarePackageContainer("HatchBaby", 1f));
            result.Add(new CarePackageContainer("PuftBaby", 1f));
            result.Add(new CarePackageContainer("SquirrelBaby", 1f));
            result.Add(new CarePackageContainer("CrabBaby", 1f));
            result.Add(new CarePackageContainer("DreckoBaby", 1f, 24));
            result.Add(new CarePackageContainer("Pacu", 8f, 24));
            result.Add(new CarePackageContainer("MoleBaby", 1f, 48));
            result.Add(new CarePackageContainer("OilfloaterBaby", 1f, 48));
            result.Add(new CarePackageContainer("LightBugEgg", 3f));
            result.Add(new CarePackageContainer("HatchEgg", 3f));
            result.Add(new CarePackageContainer("PuftEgg", 3f));
            result.Add(new CarePackageContainer("OilfloaterEgg", 3f, 12));
            result.Add(new CarePackageContainer("MoleEgg", 3f, 24));
            result.Add(new CarePackageContainer("DreckoEgg", 3f, 24));
            result.Add(new CarePackageContainer("SquirrelEgg", 2f));
            result.Add(new CarePackageContainer("BasicCure", 3f));
            result.Add(new CarePackageContainer("Funky_Vest", 1f));
            

            return result.ToArray();
        }

        //private static bool CycleCondition(int i) { return true; }
        //private static bool DiscoveredCondition(Tag tag) { return true; }

    //    #region Klei's defaults
    //    private static CarePackageInfo[] list = new CarePackageInfo[58]
    //    {
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SandStone).tag.ToString(), 1000f, (Func<bool>) null),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Dirt).tag.ToString(), 500f, (Func<bool>) null),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Algae).tag.ToString(), 500f, (Func<bool>) null),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag.ToString(), 100f, (Func<bool>) null),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Water).tag.ToString(), 2000f, (Func<bool>) null),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Sand).tag.ToString(), 3000f, (Func<bool>) null),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Carbon).tag.ToString(), 3000f, (Func<bool>) null),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Fertilizer).tag.ToString(), 3000f, (Func<bool>) null),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ice).tag.ToString(), 4000f, (Func<bool>) (() => CycleCondition(12))),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Brine).tag.ToString(), 2000f, (Func<bool>) null),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SaltWater).tag.ToString(), 2000f, (Func<bool>) null),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Rust).tag.ToString(), 1000f, (Func<bool>) null),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag.ToString(), 2000f, (Func<bool>) (() =>
    //      {
    //        if (CycleCondition(12))
    //          return DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag);
    //        return false;
    //      })),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag.ToString(), 2000f, (Func<bool>) (() =>
    //      {
    //        if (CycleCondition(12))
    //          return DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag);
    //        return false;
    //    })),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Copper).tag.ToString(), 400f, (Func<bool>) (() =>
    //      {
    //        if (CycleCondition(24))
    //          return DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Copper).tag);
    //        return false;
    //    })),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Iron).tag.ToString(), 400f, (Func<bool>) (() =>
    //      {
    //        if (CycleCondition(24))
    //          return DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Iron).tag);
    //        return false;
    //    })),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Lime).tag.ToString(), 150f, (Func<bool>) (() =>
    //      {
    //        if (CycleCondition(48))
    //          return DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Lime).tag);
    //        return false;
    //    })),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag.ToString(), 500f, (Func<bool>) (() =>
    //      {
    //        if (CycleCondition(48))
    //          return DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag);
    //        return false;
    //    })),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Glass).tag.ToString(), 200f, (Func<bool>) (() =>
    //      {
    //        if (CycleCondition(48))
    //          return DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Glass).tag);
    //        return false;
    //    })),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Steel).tag.ToString(), 100f, (Func<bool>) (() =>
    //      {
    //        if (CycleCondition(48))
    //          return DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Steel).tag);
    //        return false;
    //    })),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag.ToString(), 100f, (Func<bool>) (() =>
    //      {
    //        if (CycleCondition(48))
    //          return DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag);
    //        return false;
    //    })),
    //      new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag.ToString(), 100f, (Func<bool>) (() =>
    //      {
    //        if (CycleCondition(48))
    //          return DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag);
    //        return false;
    //    })),
    //      new CarePackageInfo("PrickleGrassSeed", 3f, (Func<bool>) null),
    //      new CarePackageInfo("LeafyPlantSeed", 3f, (Func<bool>) null),
    //      new CarePackageInfo("CactusPlantSeed", 3f, (Func<bool>) null),
    //      new CarePackageInfo("MushroomSeed", 1f, (Func<bool>) null),
    //      new CarePackageInfo("PrickleFlowerSeed", 2f, (Func<bool>) null),
    //      new CarePackageInfo("OxyfernSeed", 1f, (Func<bool>) null),
    //      new CarePackageInfo("ForestTreeSeed", 1f, (Func<bool>) null),
    //      new CarePackageInfo(BasicFabricMaterialPlantConfig.SEED_ID, 3f, (Func<bool>) (() => CycleCondition(24))),
    //      new CarePackageInfo("SwampLilySeed", 1f, (Func<bool>) (() => CycleCondition(24))),
    //      new CarePackageInfo("ColdBreatherSeed", 1f, (Func<bool>) (() => CycleCondition(24))),
    //      new CarePackageInfo("SpiceVineSeed", 1f, (Func<bool>) (() => CycleCondition(24))),
    //      new CarePackageInfo("FieldRation", 5f, (Func<bool>) null),
    //      new CarePackageInfo("BasicForagePlant", 6f, (Func<bool>) null),
    //      new CarePackageInfo("CookedEgg", 3f, (Func<bool>) (() => CycleCondition(6))),
    //      new CarePackageInfo(PrickleFruitConfig.ID, 3f, (Func<bool>) (() => CycleCondition(12))),
    //      new CarePackageInfo("FriedMushroom", 3f, (Func<bool>) (() => CycleCondition(24))),
    //      new CarePackageInfo("CookedMeat", 3f, (Func<bool>) (() => CycleCondition(48))),
    //      new CarePackageInfo("SpicyTofu", 3f, (Func<bool>) (() => CycleCondition(48))),
    //      new CarePackageInfo("LightBugBaby", 1f, (Func<bool>) null),
    //      new CarePackageInfo("HatchBaby", 1f, (Func<bool>) null),
    //      new CarePackageInfo("PuftBaby", 1f, (Func<bool>) null),
    //      new CarePackageInfo("SquirrelBaby", 1f, (Func<bool>) null),
    //      new CarePackageInfo("CrabBaby", 1f, (Func<bool>) null),
    //      new CarePackageInfo("DreckoBaby", 1f, (Func<bool>) (() => CycleCondition(24))),
    //      new CarePackageInfo("Pacu", 8f, (Func<bool>) (() => CycleCondition(24))),
    //      new CarePackageInfo("MoleBaby", 1f, (Func<bool>) (() => CycleCondition(48))),
    //      new CarePackageInfo("OilfloaterBaby", 1f, (Func<bool>) (() => CycleCondition(48))),
    //      new CarePackageInfo("LightBugEgg", 3f, (Func<bool>) null),
    //      new CarePackageInfo("HatchEgg", 3f, (Func<bool>) null),
    //      new CarePackageInfo("PuftEgg", 3f, (Func<bool>) null),
    //      new CarePackageInfo("OilfloaterEgg", 3f, (Func<bool>) (() => CycleCondition(12))),
    //      new CarePackageInfo("MoleEgg", 3f, (Func<bool>) (() => CycleCondition(24))),
    //      new CarePackageInfo("DreckoEgg", 3f, (Func<bool>) (() => CycleCondition(24))),
    //      new CarePackageInfo("SquirrelEgg", 2f, (Func<bool>) null),
    //      new CarePackageInfo("BasicCure", 3f, (Func<bool>) null),
    //      new CarePackageInfo("Funky_Vest", 1f, (Func<bool>) null)
    //};
    //    #endregion
    }
}