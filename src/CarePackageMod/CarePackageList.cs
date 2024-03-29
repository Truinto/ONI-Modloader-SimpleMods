﻿//#define LOCALE
using System.Collections.Generic;

namespace CarePackageMod
{
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