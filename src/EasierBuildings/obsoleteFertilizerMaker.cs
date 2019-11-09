using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace EasierBuildings
{
    [HarmonyPatch(typeof(FertilizerMakerConfig), "CreateBuildingDef")]
    internal class FertilizerMakerConfig_CreateBuildingDef
    {
        private static void Postfix(Game __instance, ref BuildingDef __result)
        {
            if (!(bool)EasierBuildingsState.StateManager.State.ExtraPipeOutputs)
                return;
            __result.OutputConduitType = ConduitType.Gas;
        }
    }

    [HarmonyPatch(typeof(FertilizerMakerConfig), "ConfigureBuildingTemplate")]
	internal class FertilizerMakerConfig_ConfigureBuildingTemplate
    {
		private static bool Prefix(Game __instance, GameObject go)
        {
            if (!(bool)EasierBuildingsState.StateManager.State.ExtraPipeOutputs)
                return true;
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
            Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go, false);
            defaultStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
            go.AddOrGet<WaterPurifier>();
            //ManualDeliveryKG manualDeliveryKg1 = go.AddComponent<ManualDeliveryKG>();
            //manualDeliveryKg1.SetStorage(defaultStorage);
            //manualDeliveryKg1.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
            //manualDeliveryKg1.requestedItemTag = new Tag("Dirt");
            //manualDeliveryKg1.capacity = 136.5f;
            //manualDeliveryKg1.refillMass = 19.5f;
            ManualDeliveryKG manualDeliveryKg2 = go.AddComponent<ManualDeliveryKG>();
            manualDeliveryKg2.SetStorage(defaultStorage);
            manualDeliveryKg2.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
            manualDeliveryKg2.requestedItemTag = new Tag("Phosphorite");
            manualDeliveryKg2.capacity = 6f*54.6f;
            manualDeliveryKg2.refillMass = 6f*7.8f;
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.consumptionRate = 10f;
            conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
            conduitConsumer.capacityKG = 3.12f;
            conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
            conduitConsumer.forceAlwaysSatisfied = true;
            ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
            elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
            {
                new ElementConverter.ConsumedElement(new Tag("DirtyWater"), 6f*(0.039f+0.065f)),
                //new ElementConverter.ConsumedElement(new Tag("Dirt"), 0.065f),
                new ElementConverter.ConsumedElement(new Tag("Phosphorite"), 6f*0.026f)
            };
            elementConverter.outputElements = new ElementConverter.OutputElement[]
            {
                new ElementConverter.OutputElement(6f*0.12f, SimHashes.Fertilizer, 323.15f, false, true, 0.0f, 0.5f, 1f, byte.MaxValue, 0),
                new ElementConverter.OutputElement(6f*0.01f, SimHashes.Methane, 323.15f, false, true, 0.0f, 0.5f, 1f, byte.MaxValue, 0)
            };
            //BuildingElementEmitter buildingElementEmitter = go.AddOrGet<BuildingElementEmitter>();
            //buildingElementEmitter.emitRate = 0.01f;
            //buildingElementEmitter.temperature = 349.15f;
            //buildingElementEmitter.element = SimHashes.Methane;
            //buildingElementEmitter.modifierOffset = new Vector2(2f, 2f);

            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Gas;
            conduitDispenser.invertElementFilter = true;
            conduitDispenser.elementFilter = new SimHashes[]
            {
                SimHashes.Dirt,
                SimHashes.Phosphorite,
                SimHashes.DirtyWater
            };

            ElementDropper elementDropper = go.AddComponent<ElementDropper>();
            elementDropper.emitMass = 6f*10f;
            elementDropper.emitTag = new Tag("Fertilizer");
            elementDropper.emitOffset = new Vector3(0.0f, 1f, 0.0f);
            Prioritizable.AddRef(go);

            return false;
        }
    }
    
}
