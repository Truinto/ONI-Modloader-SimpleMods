using HarmonyLib;
using System;
using UnityEngine;
using System.Collections.Generic;
using Common;
using System.Linq;

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
    }

    #region ComplexFabricator

    [HarmonyPatch(typeof(ApothecaryConfig), "ConfigureBuildingTemplate")]
    public class ApothecaryConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeApothecary;
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<ComplexFabricator>());
        }
    }

    [HarmonyPatch(typeof(ClothingFabricatorConfig), "ConfigureBuildingTemplate")]
    public class ClothingFabricatorConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeClothingFabricator;
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<ComplexFabricator>());
        }
    }

    [HarmonyPatch(typeof(CookingStationConfig), "ConfigureBuildingTemplate")]
    public class CookingStationConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeCookingStation;
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<CookingStation>());
        }
    }

    [HarmonyPatch(typeof(EggCrackerConfig), "ConfigureBuildingTemplate")]
    public class EggCrackerConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeEggCracker;
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<ComplexFabricator>());
        }
    }

    [HarmonyPatch(typeof(GlassForgeConfig), "ConfigureBuildingTemplate")]
    public class GlassForgeConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeGlassForge;
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<GlassForge>());
        }
    }

    [HarmonyPatch(typeof(GourmetCookingStationConfig), "ConfigureBuildingTemplate")]
    public class GourmetCookingStationConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeGourmetCookingStation;
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<GourmetCookingStation>());
        }
    }

    [HarmonyPatch(typeof(MetalRefineryConfig), "ConfigureBuildingTemplate")]
    public class MetalRefineryConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeMetalRefinery;
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<LiquidCooledRefinery>());
        }
    }

    [HarmonyPatch(typeof(MicrobeMusherConfig), "ConfigureBuildingTemplate")]
    public class MicrobeMusherConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeMicrobeMusher;
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<MicrobeMusher>());
        }
    }

    [HarmonyPatch(typeof(RockCrusherConfig), "ConfigureBuildingTemplate")]
    public class RockCrusherConfig_ConfigureBuildingTemplate2
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeRockCrusher;
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<ComplexFabricator>());
        }
    }

    [HarmonyPatch(typeof(SuitFabricatorConfig), "ConfigureBuildingTemplate")]
    public class SuitFabricatorConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeSuitFabricator;
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<ComplexFabricator>());
        }
    }

    [HarmonyPatch(typeof(SupermaterialRefineryConfig), "ConfigureBuildingTemplate")]
    public class SupermaterialRefineryConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeSupermaterialRefinery;
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<ComplexFabricator>());
        }
    }

#if DLC1
    [HarmonyPatch(typeof(SludgePressConfig), "ConfigureBuildingTemplate")]
    public class SludgePressConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeSludgePress;
        }

        public static void Postfix(GameObject go)
        {
            NoDupeHelper.SetAutomatic(go, go.GetComponent<ComplexFabricator>());
        }
    }
#endif
#endregion

    [HarmonyPatch(typeof(OilRefineryConfig), "ConfigureBuildingTemplate")]
    public class OilRefineryConfig_ConfigureBuildingTemplate
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeOilRefinery;
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
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeOilWellCap;
        }

        [HarmonyPriority(Priority.LowerThanNormal)]
        public static void Postfix(GameObject go)
        {
            //foreach (var mod in Global.Instance.modManager.mods) Debug.Log($"id={mod.label.id} title={mod.label.title} enabled={mod.IsEnabledForActiveDlc()}");

            if (Helpers.IsModActive("Piped output"))
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

    [HarmonyPatch(typeof(AlgaeHabitat.SMInstance), nameof(AlgaeHabitat.SMInstance.CreateEmptyChore))]
    public class AlgaeHabitatSMInstance_CreateEmptyChore
    {
        public static bool Prepare()
        {
            return false;
        }

        public static bool Prefix(AlgaeHabitat.SMInstance __instance)
        {
            __instance.master.GetComponents<Storage>().FirstOrDefault(f => f.storageFilters != null && f.storageFilters.Count > 0).DropAll();
            return false;
        }
    }

    #region StateMachine Patches
    [HarmonyPatch(typeof(Compost.States), nameof(Compost.States.InitializeStates))]
    public class Compost_States_Patch
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeCompost;
        }

        public static void Postfix(Compost.States __instance)
        {
            // remove ToggleChore
            __instance.inert.enterActions.RemoveAll(f => f.name == "ToggleChoreEnter()");
            __instance.inert.exitActions.RemoveAll(f => f.name == "ToggleChoreExit()");

            // remove ToggleStatusItem
            __instance.inert.enterActions.RemoveAll(f => f.name.StartsWith("AddStatusItem(", StringComparison.Ordinal));
            __instance.inert.exitActions.RemoveAll(f => f.name.StartsWith("RemoveStatusItem(", StringComparison.Ordinal));

            __instance.inert.GoTo(__instance.composting);

            __instance.composting.enterActions.RemoveAll(f => f.name.StartsWith("ScheduleGoTo(", StringComparison.Ordinal));
        }
    }

    [HarmonyPatch(typeof(Desalinator.States), nameof(Desalinator.States.InitializeStates))]
    public class Desalinator_Patch
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupeDesalinator;
        }

        public static void Postfix(Desalinator.States __instance)
        {
            __instance.full.enterActions.Clear();   // remove PlayAnims
            __instance.full.transitions.Clear();    // remove OnAnimQueueComplete

            __instance.full.Enter(smi =>            // OnEmptyComplete
            {
                smi.emptyChore = null;
                Tag tag = GameTagExtensions.Create(SimHashes.Salt);
                ListPool<GameObject, Desalinator>.PooledList salt = ListPool<GameObject, Desalinator>.Allocate();
                Storage storage = smi.master.GetComponent<Storage>();
                storage.Find(tag, salt);
                foreach (GameObject go in salt)
                {
                    storage.Drop(go, true);
                }
                salt.Recycle();
            });
            __instance.full.GoTo(__instance.empty);    // go back to normal operation
        }
    }
    #endregion
}