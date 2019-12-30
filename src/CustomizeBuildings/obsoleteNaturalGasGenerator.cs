using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace EasierBuildings
{
    [HarmonyPatch(typeof(MethaneGeneratorConfig), "DoPostConfigureComplete")]
    internal class MethaneGeneratorConfig_DoPostConfigureComplete
    {

        private static void Postfix(GameObject go)
        {
            if (!(bool)EasierBuildingsState.StateManager.State.ExtraPipeOutputs)
                return;
            ConduitPortInfo secondaryOutPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(2, 0));

            EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
            energyGenerator.formula.outputs = new EnergyGenerator.OutputItem[]
            {
                new EnergyGenerator.OutputItem(SimHashes.DirtyWater, 0.0675f, true, new CellOffset(1, 1), 313.15f),
                new EnergyGenerator.OutputItem(SimHashes.CarbonDioxide, 0.0225f, true, new CellOffset(0, 2), 383.15f)
            };

            ConduitDispenser2 conduitDispenser2 = go.AddOrGet<ConduitDispenser2>();
            conduitDispenser2.secondaryOutput = secondaryOutPort;
            conduitDispenser2.conduitType = secondaryOutPort.conduitType;
            conduitDispenser2.invertElementFilter = true;
            conduitDispenser2.elementFilter = new SimHashes[]
            {
                SimHashes.CarbonDioxide,
                SimHashes.Methane
            };
        }
    }
}