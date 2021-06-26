using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using Klei.AI;

namespace CustomizeModifier
{
    [HarmonyPatch(typeof(RanchStationConfig), "DoPostConfigureComplete")]
    public class RanchEarlyGroomPatch
    {
        public static void Postfix(GameObject go)
        {
            RanchStation.Def def = go.AddOrGetDef<RanchStation.Def>();
            def.isCreatureEligibleToBeRanchedCb = isCreatureEligibleToBeRanched;
            def.onRanchCompleteCb = onRanchComplete;
        }

        public static bool isCreatureEligibleToBeRanched(GameObject creature_go, RanchStation.Instance ranch_station_smi)
        {
            return 120f >= (creature_go.GetComponent<Effects>()?.Get("Ranched")?.timeRemaining ?? 0f);
        }

        public static void onRanchComplete(GameObject creature_go)
        {
            RanchStation.Instance targetRanchStation = creature_go.GetSMI<RanchableMonitor.Instance>().targetRanchStation;
            RancherChore.RancherChoreStates.Instance smi = targetRanchStation.GetSMI<RancherChore.RancherChoreStates.Instance>();
            float num = (float)(1.2 + (double)targetRanchStation.GetSMI<RancherChore.RancherChoreStates.Instance>().sm.rancher.Get(smi).GetAttributes().Get(Db.Get().Attributes.Ranching.Id).GetTotalValue() * 0.100000001490116);
            creature_go.GetComponent<Effects>().Add("Ranched", true).timeRemaining *= num;
        }
        // Notes: EffectInstance duration is defined in ModifierSet.LoadEffects(), the function above increases this time by a percentage depending on the dupes ranching skill
    }
}