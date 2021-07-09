using HarmonyLib;
using System;
using UnityEngine;
using System.Collections.Generic;
using Common;
using System.Linq;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(AlgaeHabitatConfig), nameof(AlgaeHabitatConfig.ConfigureBuildingTemplate))]
    public class AlgaeGrower
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.AlgaeTerrariumPatch && CustomizeBuildingsState.StateManager.State.AlgaeTerrarium != null;
        }

        public static void Postfix(GameObject go)
        {
            go.RemoveComponents<ElementConverter>();
            var setting = CustomizeBuildingsState.StateManager.State.AlgaeTerrarium;
            var ec = go.AddComponent<ElementConverter>();
            setting.Set(ec);

            go.RemoveComponents<ManualDeliveryKG>();
            Storage storage = go.AddOrGet<Storage>();
            var passiveConsumer = go.GetComponent<PassiveElementConsumer>();
            foreach (var element in ec.consumedElements)
            {
                ManualDeliveryKG manualDeliveryKG = go.AddComponent<ManualDeliveryKG>();
                manualDeliveryKG.SetStorage(storage);
                manualDeliveryKG.requestedItemTag = element.tag;
                manualDeliveryKG.capacity = element.massConsumptionRate * 3000f;
                manualDeliveryKG.refillMass = element.massConsumptionRate * 600f;
                manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

                if (passiveConsumer != null && element.tag.ToElement().IsLiquid)
                    passiveConsumer.elementToConsume = element.tag.ToSimHash();
            }

            foreach (var element in ec.outputElements.Where(w => w.storeOutput))
            {
                ElementDropper elementDropper = go.AddComponent<ElementDropper>();
                elementDropper.emitMass = element.massGenerationRate * 1200f;
                elementDropper.emitTag = element.elementHash.ToTag();
                elementDropper.emitOffset = new Vector3(0f, 1f, 0f);
            }
        }
    }

    [HarmonyPatch(typeof(AlgaeHabitat.SMInstance), nameof(AlgaeHabitat.SMInstance.HasEnoughMass))]
    public class AlgaeGrowerFix1
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.AlgaeTerrariumPatch;
        }

        public static bool Prefix(Tag tag, AlgaeHabitat.SMInstance __instance, ref bool __result)
        {
            if (tag.GetHash() == (int)SimHashes.Algae)
                __result = true;
            else
                __result = __instance.converter.HasEnoughMassToStartConverting();
            return false;
        }
    }
}
