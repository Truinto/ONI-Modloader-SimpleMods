using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;
using static BootDialog.PostBootDialog;

namespace CustomizePlants
{
    public static class PLANTS
    {
        public static readonly string[] NAMES = {
            BasicSingleHarvestPlantConfig.ID,
            SeaLettuceConfig.ID,
            BasicFabricMaterialPlantConfig.ID,
            BeanPlantConfig.ID,
            BulbPlantConfig.ID,
            CactusPlantConfig.ID,
            ColdWheatConfig.ID,
            EvilFlowerConfig.ID,
            ForestTreeBranchConfig.ID,
            ForestTreeConfig.ID,
            GasGrassConfig.ID,
            LeafyPlantConfig.ID,
            MushroomPlantConfig.ID,
            PrickleFlowerConfig.ID,
            PrickleGrassConfig.ID,
            SaltPlantConfig.ID,
            SpiceVineConfig.ID,
            SwampLilyConfig.ID,
            ColdBreatherConfig.ID,
            OxyfernConfig.ID
        };

        public static readonly Type[] METHODS = {
            typeof(BasicSingleHarvestPlantConfig),
            typeof(SeaLettuceConfig),
            typeof(BasicFabricMaterialPlantConfig),
            typeof(BeanPlantConfig),
            typeof(BulbPlantConfig),
            typeof(CactusPlantConfig),
            typeof(ColdWheatConfig),
            typeof(EvilFlowerConfig),
            typeof(ForestTreeConfig),
            typeof(ForestTreeBranchConfig),
            typeof(GasGrassConfig),
            typeof(LeafyPlantConfig),
            typeof(MushroomPlantConfig),
            typeof(PrickleFlowerConfig),
            typeof(PrickleGrassConfig),
            typeof(SaltPlantConfig),
            typeof(SpiceVineConfig),
            typeof(SwampLilyConfig),
            typeof(ColdBreatherConfig),
            typeof(OxyfernConfig)
        };
    }
    
    [HarmonyPatch(typeof(KMod.Mod), "Load")]
    public static class OnLoadPatch
	{
        public static bool IsPatched = false;

        public static void Prefix()
        {
            if (IsPatched) return;
            IsPatched = true;

            var harmony = HarmonyInstance.Create("com.fumihiko.oni.customizeplants");
            var postfix = typeof(OnLoadPatch).GetMethod("PlantPostfix");

            foreach (Type type in PLANTS.METHODS)
            {
                var original = type.GetMethod("CreatePrefab");
                harmony.Patch(original, prefix: null, postfix: new HarmonyMethod(postfix));

                //FumLib.FumTools.PrintAllPatches(type, "CreatePrefab");
            }

            if (CustomizePlantsState.StateManager.State.ModPlants != null)
            { 
                foreach (string config in CustomizePlantsState.StateManager.State.ModPlants)
                {
                    try
                    {
                        var original = Type.GetType(config).GetMethod("CreatePrefab");
                        harmony.Patch(original, prefix: null, postfix: new HarmonyMethod(postfix));
                    }
                    catch (Exception)
                    {
                        Debug.LogWarning(ToDialog("ModPlants: " + config + " is not a valid class."));
                    }
                }
            }
        }
        
        public static void PlantPostfix(GameObject __result)
        {
            PlantHelper.ProcessPlant(__result);
        }

    }
    
}
