using Common;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomizeBuildings
{
    public class RadBattery : IBuildingCompleteMod
    {
        bool IBuildingCompleteMod.Enabled(string id)
        {
            return id == HEPBatteryConfig.ID && CustomizeBuildingsState.StateManager.State.RadBattery != 1000f;
        }

        public void EditDef(BuildingDef def)
        {
        }

        public void EditGO(BuildingDef def)
        {
            var HEP = def.BuildingComplete.GetComponent<HighEnergyParticleStorage>();
            HEP.capacity = CustomizeBuildingsState.StateManager.State.RadBattery;
        }
    }
}
