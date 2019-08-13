using System.Collections.Generic;
using ONI_Common.Json;
using UnityEngine;
using System;
using ModHelper;

namespace CarePackageMod
{
    public class CarePackageState
    {
        public bool enabled { get; set; } = true;

        //public CarePackageInfo[] CarePackages { get; set; } = new CarePackageInfo[] {
        //    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag.ToString(), 100f, (Func<bool>) null),
        //    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag.ToString(), 500f, (Func<bool>) null)
        //};

        public Dictionary<string, float> CarePackages { get; set; } = CarePackageList.GetPackages();

        public static BaseStateManager<CarePackageState> StateManager
            = new BaseStateManager<CarePackageState>(new ModFolderPathHelper("CarePackageMod", 1833878154L).path,
                ONI_Common.Paths.GetLogsPath() + ModFolderPathHelper.sep + "CarePackageModLog.txt");
        //public static BaseStateManager<LessIrrigation> StateManager = new BaseStateManager<CarePackageState>("CarePackageMod");
    }

    internal class CarePackageList
    {
        internal static Dictionary<string, float> GetPackages()
        {
            Dictionary<string, float> result = new Dictionary<string, float>();

            foreach (CarePackageInfo info in list)
            {
                result.Add(info.id, info.quantity);
            }

            result.Add("Niobium", 5f);
            result.Add("Isoresin", 35f);
            result.Add("Fullerene", 1f);
            result.Add("Katairite", 1000f);
            result.Add("Diamond", 500f);
            result.Add(GeneShufflerRechargeConfig.ID, 1f);
            

            return result;
        }

        private static bool CycleCondition(int i) { return true; }
        private static bool DiscoveredCondition(Tag tag) { return true; }

        internal static CarePackageInfo[] list = new CarePackageInfo[58]
        {
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SandStone).tag.ToString(), 1000f, (Func<bool>) null),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Dirt).tag.ToString(), 500f, (Func<bool>) null),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Algae).tag.ToString(), 500f, (Func<bool>) null),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag.ToString(), 100f, (Func<bool>) null),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Water).tag.ToString(), 2000f, (Func<bool>) null),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Sand).tag.ToString(), 3000f, (Func<bool>) null),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Carbon).tag.ToString(), 3000f, (Func<bool>) null),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Fertilizer).tag.ToString(), 3000f, (Func<bool>) null),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ice).tag.ToString(), 4000f, (Func<bool>) (() => CycleCondition(12))),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Brine).tag.ToString(), 2000f, (Func<bool>) null),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SaltWater).tag.ToString(), 2000f, (Func<bool>) null),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Rust).tag.ToString(), 1000f, (Func<bool>) null),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag.ToString(), 2000f, (Func<bool>) (() =>
          {
            if (CycleCondition(12))
              return DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag);
            return false;
          })),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag.ToString(), 2000f, (Func<bool>) (() =>
          {
            if (CycleCondition(12))
              return DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag);
            return false;
        })),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Copper).tag.ToString(), 400f, (Func<bool>) (() =>
          {
            if (CycleCondition(24))
              return DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Copper).tag);
            return false;
        })),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Iron).tag.ToString(), 400f, (Func<bool>) (() =>
          {
            if (CycleCondition(24))
              return DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Iron).tag);
            return false;
        })),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Lime).tag.ToString(), 150f, (Func<bool>) (() =>
          {
            if (CycleCondition(48))
              return DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Lime).tag);
            return false;
        })),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag.ToString(), 500f, (Func<bool>) (() =>
          {
            if (CycleCondition(48))
              return DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag);
            return false;
        })),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Glass).tag.ToString(), 200f, (Func<bool>) (() =>
          {
            if (CycleCondition(48))
              return DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Glass).tag);
            return false;
        })),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Steel).tag.ToString(), 100f, (Func<bool>) (() =>
          {
            if (CycleCondition(48))
              return DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Steel).tag);
            return false;
        })),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag.ToString(), 100f, (Func<bool>) (() =>
          {
            if (CycleCondition(48))
              return DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag);
            return false;
        })),
          new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag.ToString(), 100f, (Func<bool>) (() =>
          {
            if (CycleCondition(48))
              return DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag);
            return false;
        })),
          new CarePackageInfo("PrickleGrassSeed", 3f, (Func<bool>) null),
          new CarePackageInfo("LeafyPlantSeed", 3f, (Func<bool>) null),
          new CarePackageInfo("CactusPlantSeed", 3f, (Func<bool>) null),
          new CarePackageInfo("MushroomSeed", 1f, (Func<bool>) null),
          new CarePackageInfo("PrickleFlowerSeed", 2f, (Func<bool>) null),
          new CarePackageInfo("OxyfernSeed", 1f, (Func<bool>) null),
          new CarePackageInfo("ForestTreeSeed", 1f, (Func<bool>) null),
          new CarePackageInfo(BasicFabricMaterialPlantConfig.SEED_ID, 3f, (Func<bool>) (() => CycleCondition(24))),
          new CarePackageInfo("SwampLilySeed", 1f, (Func<bool>) (() => CycleCondition(24))),
          new CarePackageInfo("ColdBreatherSeed", 1f, (Func<bool>) (() => CycleCondition(24))),
          new CarePackageInfo("SpiceVineSeed", 1f, (Func<bool>) (() => CycleCondition(24))),
          new CarePackageInfo("FieldRation", 5f, (Func<bool>) null),
          new CarePackageInfo("BasicForagePlant", 6f, (Func<bool>) null),
          new CarePackageInfo("CookedEgg", 3f, (Func<bool>) (() => CycleCondition(6))),
          new CarePackageInfo(PrickleFruitConfig.ID, 3f, (Func<bool>) (() => CycleCondition(12))),
          new CarePackageInfo("FriedMushroom", 3f, (Func<bool>) (() => CycleCondition(24))),
          new CarePackageInfo("CookedMeat", 3f, (Func<bool>) (() => CycleCondition(48))),
          new CarePackageInfo("SpicyTofu", 3f, (Func<bool>) (() => CycleCondition(48))),
          new CarePackageInfo("LightBugBaby", 1f, (Func<bool>) null),
          new CarePackageInfo("HatchBaby", 1f, (Func<bool>) null),
          new CarePackageInfo("PuftBaby", 1f, (Func<bool>) null),
          new CarePackageInfo("SquirrelBaby", 1f, (Func<bool>) null),
          new CarePackageInfo("CrabBaby", 1f, (Func<bool>) null),
          new CarePackageInfo("DreckoBaby", 1f, (Func<bool>) (() => CycleCondition(24))),
          new CarePackageInfo("Pacu", 8f, (Func<bool>) (() => CycleCondition(24))),
          new CarePackageInfo("MoleBaby", 1f, (Func<bool>) (() => CycleCondition(48))),
          new CarePackageInfo("OilfloaterBaby", 1f, (Func<bool>) (() => CycleCondition(48))),
          new CarePackageInfo("LightBugEgg", 3f, (Func<bool>) null),
          new CarePackageInfo("HatchEgg", 3f, (Func<bool>) null),
          new CarePackageInfo("PuftEgg", 3f, (Func<bool>) null),
          new CarePackageInfo("OilfloaterEgg", 3f, (Func<bool>) (() => CycleCondition(12))),
          new CarePackageInfo("MoleEgg", 3f, (Func<bool>) (() => CycleCondition(24))),
          new CarePackageInfo("DreckoEgg", 3f, (Func<bool>) (() => CycleCondition(24))),
          new CarePackageInfo("SquirrelEgg", 2f, (Func<bool>) null),
          new CarePackageInfo("BasicCure", 3f, (Func<bool>) null),
          new CarePackageInfo("Funky_Vest", 1f, (Func<bool>) null)
    };
    }
}