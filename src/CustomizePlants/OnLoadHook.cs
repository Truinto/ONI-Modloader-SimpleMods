using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using static Config.PostBootDialog;
using System.Reflection;
using System.IO;
using Common;
using static ResearchTypes;

namespace CustomizePlants
{
    public static class PLANTS
    {
        public static readonly string[] NAMES = [
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
            OxyfernConfig.ID,
            SuperWormPlantConfig.ID,
            WormPlantConfig.ID,
            SwampHarvestPlantConfig.ID,
            ToePlantConfig.ID,
            WineCupsConfig.ID,
            CritterTrapPlantConfig.ID,
            CylindricaConfig.ID,
            FilterPlantConfig.ID,
            FlyTrapPlantConfig.ID,
            FilamentPlantConfig.ID,
            DinofernConfig.ID,
            DewPalmConfig.ID,
            ClamConfig.ID,
            DewDripperPlantConfig.ID,
            IceFlowerConfig.ID,
            HardSkinBerryPlantConfig.ID,
            GardenFoodPlantConfig.ID,
            GardenDecorPlantConfig.ID,
            CarrotPlantConfig.ID,
            ButterflyPlantConfig.ID,
            BulbloomConfig.ID,
            KelpPlantConfig.ID,
            SpaceTreeConfig.ID,
            SpaceTreeBranchConfig.ID,
            OxyCoralConfig.ID,
            SeaTreeRootConfig.ID,
            SeaTreeBranchConfig.ID,
            PlanktonCoralConfig.ID,
            SaltySticksPlantConfig.ID,
            WaterCupsPlantConfig.ID,
            VineMotherConfig.ID,
            VineBranchConfig.ID,
            UrchinPlantConfig.ID,
            TubeWormConfig.ID,
        ];

        public static readonly Type[] CLASSES = [
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
            typeof(OxyfernConfig),
            typeof(SuperWormPlantConfig),
            typeof(WormPlantConfig),
            typeof(SwampHarvestPlantConfig),
            typeof(ToePlantConfig),
            typeof(WineCupsConfig),
            typeof(CritterTrapPlantConfig),
            typeof(CylindricaConfig),
            typeof(FilterPlantConfig),
            typeof(FlyTrapPlantConfig),
            typeof(FilamentPlantConfig),
            typeof(DinofernConfig),
            typeof(DewPalmConfig),
            typeof(ClamConfig),
            typeof(DewDripperPlantConfig),
            typeof(IceFlowerConfig),
            typeof(HardSkinBerryPlantConfig),
            typeof(GardenFoodPlantConfig),
            typeof(GardenDecorPlantConfig),
            typeof(CarrotPlantConfig),
            typeof(ButterflyPlantConfig),
            typeof(BulbloomConfig),
            typeof(KelpPlantConfig),
            typeof(SpaceTreeConfig),
            typeof(SpaceTreeBranchConfig),
            typeof(OxyCoralConfig),
            typeof(SeaTreeRootConfig),
            typeof(SeaTreeBranchConfig),
            typeof(PlanktonCoralConfig),
            typeof(SaltySticksPlantConfig),
            typeof(WaterCupsPlantConfig),
            typeof(VineMotherConfig),
            typeof(VineBranchConfig),
            typeof(UrchinPlantConfig),
            typeof(TubeWormConfig),
        ];

        public static readonly Type[] CLASSES_NOPREVIEW = [
            typeof(ForestTreeBranchConfig),
            typeof(SuperWormPlantConfig),
            typeof(SpaceTreeBranchConfig),
            typeof(SeaTreeBranchConfig),
            typeof(VineBranchConfig),
        ];
    }

    [HarmonyPatch(typeof(Assets), nameof(Assets.AddPrefab))]
    public static class Assets_AddPrefabPatch
    {
        public static void Prefix(KPrefabID prefab)
        {
            if (prefab != null && !CustomizePlantsState.Instance.IgnoreList.Contains(prefab.gameObject.name))
                PlantHelper.ProcessPlant(prefab.gameObject);
        }
    }

    public static class OnLoadPatch
    {
        public static bool IsPatched = false;

        public static void OnLoad()
        {
            if (IsPatched) return;
            IsPatched = true;

            var harmony = FumiKMod.Instance;
            var postfix = new HarmonyMethod(typeof(OnLoadPatch).GetMethod(nameof(OnLoadPatch.PlantPostfix)));

            foreach (Type type in PLANTS.CLASSES_NOPREVIEW)
            {
                var original = type.GetMethod("CreatePrefab");
                harmony.Patch(original, prefix: null, postfix: postfix);
            }

            if (CustomizePlantsState.Instance.ModPlants != null)
            {
                foreach (string config in CustomizePlantsState.Instance.ModPlants)
                {
                    try
                    {
                        int cStart = config.IndexOf(' ') + 1;
                        int cLength = config.IndexOf(',', cStart) - cStart;
                        string nameDll = config.Substring(cStart, cLength) + ".dll";
                        if (nameDll != "Assembly-CSharp.dll")
                        {
                            nameDll = nameDll.Replace("-merged.dll", "*.dll");
                            string[] dlls = Directory.GetFiles(Config.PathHelper.ModsDirectory, nameDll, SearchOption.AllDirectories);
                            if (dlls.Length == 0) throw new FileNotFoundException("ModPlants: could not find mod: " + nameDll);

                            foreach (string dll in dlls)
                            {
                                Helpers.Print("ModPlants: loading external dll: " + dll);
                                Assembly.LoadFile(dll);
                            }
                        }

                        Type type = Type.GetType(config, true);
                        MethodInfo original = type.GetMethod("CreatePrefab") ?? throw new NullReferenceException("ModPlants: CreatePrefab is NULL");
                        harmony.Patch(original, prefix: null, postfix: postfix);
                    }
                    catch (Exception e)
                    {
                        Helpers.Print(e.Message);
                        Helpers.Print(ToDialog("ModPlants: " + config + " is not a valid qualifier."));
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
