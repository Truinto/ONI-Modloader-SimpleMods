using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ConduitMod
{
    [HarmonyPatch(typeof(Valve), "OnSpawn")]
    internal class ConduitMod
    {
        private static void Prefix(ValveBase ___valveBase)
        {
            int inputCell = (int) Traverse.Create(___valveBase).Field("inputCell").GetValue();
            int outputCell = (int) Traverse.Create(___valveBase).Field("outputCell").GetValue();
            ConduitType conduitType = (ConduitType) Traverse.Create(___valveBase).Field("conduitType").GetValue();

            Debug.Log("inputCell: " + inputCell);
            Debug.Log("outputCell: " + outputCell);

            if (true)//(conduitType == ConduitType.Gas)
            {
                Game.Instance.gasConduitSystem.RemoveLink(inputCell, outputCell);
                Debug.Log("OnSpawn RemoveLink Gas");
            }

            if (true)//(conduitType == ConduitType.Liquid)
            {
                Game.Instance.liquidConduitSystem.RemoveLink(inputCell, outputCell);
                Debug.Log("OnSpawn RemoveLink Liquid");
            }
        }
    }
}

/*namespace LogicMod
{
    [HarmonyPatch(typeof(LogicGate), "OnSpawn")]
    internal class LogicMod
    {
        private static void Prefix(LogicGateBase.Op ___op, int ___InputCellOne, int ___OutputCell)
        {
            Debug.Log("OnSpawn LogicGate");
            if (___op == LogicGateBase.Op.Not)
            {
                Game.Instance.logicCircuitSystem.RemoveLink(___InputCellOne, ___OutputCell);
                Debug.Log("OnSpawn RemoveLink Logic");
            }
        }
    }
}*/
