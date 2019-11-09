using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace EasierBuildings
{
    [HarmonyPatch(typeof(PetroleumGeneratorConfig), "DoPostConfigureComplete")]
    internal class PetroleumGeneratorConfig_DoPostConfigureComplete
    {

        private static void Postfix(GameObject go)
        {
            if (!(bool)EasierBuildingsState.StateManager.State.ExtraPipeOutputs)
                return;
            ConduitPortInfo secondaryOutPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(1, 0));

            go.AddOrGet<Storage>().capacityKg = 100f;

            EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
            energyGenerator.formula.outputs = new EnergyGenerator.OutputItem[]
            {
                new EnergyGenerator.OutputItem(SimHashes.CarbonDioxide, 0.5f, false, new CellOffset(0, 3), 383.15f),
                new EnergyGenerator.OutputItem(SimHashes.DirtyWater, 0.75f, true, new CellOffset(1, 1), 313.15f)
            };

            ConduitDispenser2 conduitDispenser2 = go.AddOrGet<ConduitDispenser2>();
            conduitDispenser2.secondaryOutput = secondaryOutPort;
            conduitDispenser2.conduitType = secondaryOutPort.conduitType;
            conduitDispenser2.invertElementFilter = true;
            conduitDispenser2.elementFilter = new SimHashes[]
            {
                SimHashes.Petroleum,
                SimHashes.Ethanol,
                SimHashes.CarbonDioxide
            };
        }
    }
}