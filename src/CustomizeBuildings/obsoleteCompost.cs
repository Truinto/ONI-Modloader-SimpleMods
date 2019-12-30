using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace EasierBuildings
{
    /*
    [HarmonyPatch(typeof(CompostConfig), "CreateBuildingDef")]
    internal class CompostConfig_CreateBuildingDef
    {
        private static void Postfix(Game __instance, ref BuildingDef __result)
        {
            __result.UtilityInputOffset = new CellOffset(0, 0);
            __result.UtilityOutputOffset = new CellOffset(1, 0);
            __result.InputConduitType = ConduitType.Solid;
            __result.OutputConduitType = ConduitType.Solid;
        }
    }

    [HarmonyPatch(typeof(CompostConfig), "ConfigureBuildingTemplate")]
    internal class CompostConfig_ConfigureBuildingTemplate
    {
        private static bool Prefix(Game __instance, GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = 2000f;
            go.AddOrGet<Compost>().simulatedInternalTemperature = 348.15f;
            CompostWorkable compostWorkable = go.AddOrGet<CompostWorkable>();
            compostWorkable.workTime = 20f;
            compostWorkable.overrideAnims = new KAnimFile[1]
            {
                Assets.GetAnim((HashedString) "anim_interacts_compost_kanim")
            };
            ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
            elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
            {
                new ElementConverter.ConsumedElement(CompostConfig.COMPOST_TAG, 0.1f)
            };
            elementConverter.outputElements = new ElementConverter.OutputElement[1]
            {
                new ElementConverter.OutputElement(0.1f, SimHashes.Dirt, 348.15f, false, true, 0.0f, 0.5f, 1f, byte.MaxValue, 0)
            };
            ElementDropper elementDropper = go.AddComponent<ElementDropper>();
            elementDropper.emitMass = 1000f;
            elementDropper.emitTag = SimHashes.Dirt.CreateTag();
            elementDropper.emitOffset = new Vector3(0.5f, 1f, 0.0f);
            ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
            manualDeliveryKg.SetStorage(storage);
            manualDeliveryKg.requestedItemTag = CompostConfig.COMPOST_TAG;
            manualDeliveryKg.capacity = 300f;
            manualDeliveryKg.refillMass = 0.1f;
            manualDeliveryKg.minimumMass = 100f;
            manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FarmFetch.IdHash;

            go.AddOrGet<SolidConduitConsumer>();
            SolidConduitDispenser conduitDispenser = go.AddOrGet<SolidConduitDispenser>();
            conduitDispenser.alwaysDispense = true;
            conduitDispenser.invertElementFilter = false;
            conduitDispenser.elementFilter = new SimHashes[]
            {
                SimHashes.Dirt
            };

            Prioritizable.AddRef(go);
            go.AddOrGet<BuildingComplete>().isManuallyOperated = true;

            return false;
        }
    }

    [HarmonyPatch(typeof(SolidConduitDispenser), "FindSuitableItem")]
    internal class SolidConduitDispenser_FindSuitableItem
    {
        private static bool Prefix(ref Storage ___storage, ref int ___round_robin_index, ref SimHashes[] ___elementFilter, ref bool ___invertElementFilter, ref Pickupable __result)
        {
            List<GameObject> items = ___storage.items;
            if (items.Count < 1) { __result = (Pickupable)null; return false; }

            for (int i = 0; i < items.Count; i++)
            {
                ___round_robin_index %= items.Count;
                GameObject item = items[___round_robin_index];
                ++___round_robin_index;

                PrimaryElement component = item.GetComponent<PrimaryElement>();

                if ((UnityEngine.Object)component != (UnityEngine.Object)null &&
                    (double)component.Mass > 0.0 &&
                    (___elementFilter == null || ___elementFilter.Length == 0 || !___invertElementFilter &&
                    IsFilteredElement(component.ElementID, ___elementFilter) || ___invertElementFilter &&
                    !IsFilteredElement(component.ElementID, ___elementFilter)))
                {
                    __result = !(bool)((UnityEngine.Object)item) ? (Pickupable)null : item.GetComponent<Pickupable>();
                    return false;
                }
            }

            __result = (Pickupable)null; return false;
        }

        private static bool IsFilteredElement(SimHashes element, SimHashes[] elementFilter)
        {
            for (int index = 0; index != elementFilter.Length; ++index)
            {
                if (elementFilter[index] == element)
                    return true;
            }
            return false;
        }
    }
    */

}