using HarmonyLib;
using UnityEngine;
using System;
using Klei.AI;

namespace CustomizeBuildings
{
    public class NoDupeRanch : IBuildingCompleteMod
    {
        public bool Enabled(string id)
        {
            return id == RanchStationConfig.ID 
                && CustomizeBuildingsState.Instance.NoDupeGlobal 
                && CustomizeBuildingsState.Instance.NoDupeRanchStation;
        }

        public void EditDef(BuildingDef def)
        {
        }

        public void EditGO(BuildingDef def)
        {
            Db.Get().effects.Get("Ranched").duration = 60000f;
        }
    }
}
