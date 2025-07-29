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
        public static void SetAutomatic(GameObject go)
        {
            var bc = go.GetComponent<BuildingComplete>();
            var fabricator = go.GetComponent<ComplexFabricator>();

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

            bc.isManuallyOperated = false;
            fabricator.duplicantOperated = false;
        }
    }

    #region ComplexFabricator

    public class NoDupeMods : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal;
        }

        public void EditDef(BuildingDef def)
        {
        }

        public void EditGO(BuildingDef def)
        {
            switch (def.PrefabID)
            {
                case ApothecaryConfig.ID:
                    if (CustomizeBuildingsState.StateManager.State.NoDupeApothecary)
                        NoDupeHelper.SetAutomatic(def.BuildingComplete);
                    break;
                case ClothingFabricatorConfig.ID:
                    if (CustomizeBuildingsState.StateManager.State.NoDupeClothingFabricator)
                        NoDupeHelper.SetAutomatic(def.BuildingComplete);
                    break;
                case CookingStationConfig.ID:
                    if (CustomizeBuildingsState.StateManager.State.NoDupeCookingStation)
                        NoDupeHelper.SetAutomatic(def.BuildingComplete);
                    break;
                case EggCrackerConfig.ID:
                    if (CustomizeBuildingsState.StateManager.State.NoDupeEggCracker)
                        NoDupeHelper.SetAutomatic(def.BuildingComplete);
                    break;
                case GlassForgeConfig.ID:
                    if (CustomizeBuildingsState.StateManager.State.NoDupeGlassForge)
                        NoDupeHelper.SetAutomatic(def.BuildingComplete);
                    break;
                case GourmetCookingStationConfig.ID:
                    if (CustomizeBuildingsState.StateManager.State.NoDupeGourmetCookingStation)
                        NoDupeHelper.SetAutomatic(def.BuildingComplete);
                    break;
                case MetalRefineryConfig.ID:
                    if (CustomizeBuildingsState.StateManager.State.NoDupeMetalRefinery)
                        NoDupeHelper.SetAutomatic(def.BuildingComplete);
                    break;
                case MicrobeMusherConfig.ID:
                    if (CustomizeBuildingsState.StateManager.State.NoDupeMicrobeMusher)
                        NoDupeHelper.SetAutomatic(def.BuildingComplete);
                    break;
                case RockCrusherConfig.ID:
                    if (CustomizeBuildingsState.StateManager.State.NoDupeRockCrusher)
                        NoDupeHelper.SetAutomatic(def.BuildingComplete);
                    break;
                case DiamondPressConfig.ID:
                    if (CustomizeBuildingsState.StateManager.State.NoDupeDiamondPress)
                        NoDupeHelper.SetAutomatic(def.BuildingComplete);
                    break;
                case SuitFabricatorConfig.ID:
                    if (CustomizeBuildingsState.StateManager.State.NoDupeSuitFabricator)
                        NoDupeHelper.SetAutomatic(def.BuildingComplete);
                    break;
                case SupermaterialRefineryConfig.ID:
                    if (CustomizeBuildingsState.StateManager.State.NoDupeSupermaterialRefinery)
                        NoDupeHelper.SetAutomatic(def.BuildingComplete);
                    break;
                case SludgePressConfig.ID:
                    if (CustomizeBuildingsState.StateManager.State.NoDupeSludgePress)
                        NoDupeHelper.SetAutomatic(def.BuildingComplete);
                    break;
                case MilkPressConfig.ID:
                    if (CustomizeBuildingsState.StateManager.State.NoDupeMilkPress)
                        NoDupeHelper.SetAutomatic(def.BuildingComplete);
                    break;
                case ChemicalRefineryConfig.ID:
                    if (CustomizeBuildingsState.StateManager.State.NoDupeChemicalRefinery)
                        NoDupeHelper.SetAutomatic(def.BuildingComplete);
                    break;
                case MissileFabricatorConfig.ID:
                    if (CustomizeBuildingsState.StateManager.State.NoDupeMissileFabricator)
                        NoDupeHelper.SetAutomatic(def.BuildingComplete);
                    break;
                case DeepfryerConfig.ID:
                    if (CustomizeBuildingsState.StateManager.State.NoDupeDeepfryer)
                        NoDupeHelper.SetAutomatic(def.BuildingComplete);
                    break;
            }
        }
    }

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

            if (Helpers.IsModActive("PipedOutput"))
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

    [HarmonyPatch(typeof(RailGunPayloadOpener), MethodType.StaticConstructor)]
    public class RailGunPayloadOpener_Static
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NoDupeGlobal && CustomizeBuildingsState.StateManager.State.NoDupePayloadOpener;
        }

        public static void Postfix()
        {
            RailGunPayloadOpener.delivery_time = 0f;
        }
    }

    [HarmonyPatch(typeof(AlgaeHabitat.SMInstance), nameof(AlgaeHabitat.SMInstance.CreateEmptyChore))]
    public class AlgaeHabitatSMInstance_CreateEmptyChore
    {
        public static bool Prepare()
        {
            return false; // TODO: finish NoDupeAlgae
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
