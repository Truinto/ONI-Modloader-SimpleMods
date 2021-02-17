using Harmony;
using System;
using UnityEngine;
using System.Collections.Generic;


namespace CustomizeBuildings
{
    public class NoDupeHelper
    {
        public static void SetAutomatic(GameObject go, object fabricatorObject)
        {
            BuildingComplete bc = go.GetComponent<BuildingComplete>();
            ComplexFabricator fabricator = (ComplexFabricator)fabricatorObject;

            if (bc == null)
            {
                Debug.LogWarning("SetAutomatic bc was null");
                return;
            }
            if (fabricator == null)
            {
                Debug.LogWarning("SetAutomatic fabricator was null");
                return;
            }

            go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
            fabricator.duplicantOperated = false;
        }

        public static bool CheckConfig(Dictionary<string, bool> state, string key)
        {
            try
            {
                bool result = state[key];
                Debug.Log("CheckConfig " + key + " resulted in " + result.ToString());
                return result;
            }
            catch (Exception) { Debug.LogWarning("CheckSetting could not resolve: " + key); }
            return false;
        }

        public static bool CheckConfig(string key)
        {
            if (!CustomizeBuildingsState.StateManager.State.NoDupeBuildingsGlobal) return false;

            try
            {
                bool result = CustomizeBuildingsState.StateManager.State.NoDupeBuildings[key];
                Debug.Log("CheckConfig " + key + " resulted in " + result.ToString());
                return result;
            }
            catch (Exception) { Debug.LogWarning("CheckConfig could not resolve: " + key); }
            return false;
        }
    }

    #region Postfixes

    [HarmonyPatch(typeof(ApothecaryConfig), "ConfigureBuildingTemplate")]
    public class ApothecaryConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return NoDupeHelper.CheckConfig(CustomizeBuildingsState.IDApothecary);
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<ComplexFabricator>());
        }
    }

    [HarmonyPatch(typeof(ClothingFabricatorConfig), "ConfigureBuildingTemplate")]
    internal class ClothingFabricatorConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return NoDupeHelper.CheckConfig(CustomizeBuildingsState.IDClothingFabricator);
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<ComplexFabricator>());
        }
    }

    [HarmonyPatch(typeof(CookingStationConfig), "ConfigureBuildingTemplate")]
    internal class CookingStationConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return NoDupeHelper.CheckConfig(CustomizeBuildingsState.IDCookingStation);
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<CookingStation>());
        }
    }

    [HarmonyPatch(typeof(EggCrackerConfig), "ConfigureBuildingTemplate")]
    internal class EggCrackerConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return NoDupeHelper.CheckConfig(CustomizeBuildingsState.IDEggCracker);
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<ComplexFabricator>());
        }
    }

    [HarmonyPatch(typeof(GlassForgeConfig), "ConfigureBuildingTemplate")]
    internal class GlassForgeConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return NoDupeHelper.CheckConfig(CustomizeBuildingsState.IDGlassForge);
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<GlassForge>());
        }
    }

    [HarmonyPatch(typeof(GourmetCookingStationConfig), "ConfigureBuildingTemplate")]
    internal class GourmetCookingStationConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return NoDupeHelper.CheckConfig(CustomizeBuildingsState.IDGourmetCookingStation);
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<GourmetCookingStation>());
        }
    }

    [HarmonyPatch(typeof(MetalRefineryConfig), "ConfigureBuildingTemplate")]
    internal class MetalRefineryConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return NoDupeHelper.CheckConfig(CustomizeBuildingsState.IDMetalRefinery);
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<LiquidCooledRefinery>());
        }
    }

    [HarmonyPatch(typeof(MicrobeMusherConfig), "ConfigureBuildingTemplate")]
    internal class MicrobeMusherConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return NoDupeHelper.CheckConfig(CustomizeBuildingsState.IDMicrobeMusher);
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<MicrobeMusher>());
        }
    }

    [HarmonyPatch(typeof(RockCrusherConfig), "ConfigureBuildingTemplate")]
    internal class RockCrusherConfig_ConfigureBuildingTemplate2
    {
        public static bool Prepare()
        {
            return NoDupeHelper.CheckConfig(CustomizeBuildingsState.IDRockCrusher);
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<ComplexFabricator>());
        }
    }

    [HarmonyPatch(typeof(SuitFabricatorConfig), "ConfigureBuildingTemplate")]
    internal class SuitFabricatorConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return NoDupeHelper.CheckConfig(CustomizeBuildingsState.IDSuitFabricator);
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<ComplexFabricator>());
        }
    }

    [HarmonyPatch(typeof(SupermaterialRefineryConfig), "ConfigureBuildingTemplate")]
    internal class SupermaterialRefineryConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return NoDupeHelper.CheckConfig(CustomizeBuildingsState.IDSupermaterialRefinery);
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<ComplexFabricator>());
        }
    }

    [HarmonyPatch(typeof(OilRefineryConfig), "ConfigureBuildingTemplate")]
    internal class OilRefineryConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return NoDupeHelper.CheckConfig(CustomizeBuildingsState.IDOilRefinery);
        }

        public static void Postfix(GameObject go)
        {
            OilRefinery oilRefinery = go.GetComponent<OilRefinery>();            
            if (oilRefinery == null)
            {
                Debug.LogWarning("oilRefinery was null");
            }

            go.GetComponent<BuildingComplete>().isManuallyOperated = false;
            //go.GetComponent<OilRefinery>().enabled = false;
            UnityEngine.Object.DestroyImmediate(oilRefinery);

            go.AddOrGet<WaterPurifier>();
        }
    }

    [HarmonyPatch(typeof(OilWellCapConfig), nameof(OilWellCapConfig.ConfigureBuildingTemplate))]
    public class OilWellCapConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return NoDupeHelper.CheckConfig(CustomizeBuildingsState.IDOilWellCap);
        }

        [HarmonyPriority(Priority.LowerThanNormal)]
        public static void Postfix(GameObject go)
        {
            //foreach (var mod in Global.Instance.modManager.mods) Debug.Log($"id={mod.label.id} title={mod.label.title} enabled={mod.IsEnabledForActiveDlc()}");

            if (Helper.IsModActive("Piped output"))
                return;

            OilWellCap oilWellCap = go.GetComponent<OilWellCap>();
            if (oilWellCap == null)
            {
                Debug.LogWarning("oilWellCap was null");
                return;
            }

            BuildingElementEmitter buildingElementEmitter = go.AddComponent<BuildingElementEmitter>();
            buildingElementEmitter.emitRate = oilWellCap.addGasRate; // = 0.03333334f;
            buildingElementEmitter.temperature = oilWellCap.gasTemperature; // = 573.15f;
            buildingElementEmitter.element = oilWellCap.gasElement; // = SimHashes.Methane;
            buildingElementEmitter.modifierOffset = new Vector2(2f, 2f);

            UnityEngine.Object.DestroyImmediate(oilWellCap);
            go.AddOrGet<WaterPurifier>();
        }
    }

    #endregion
}