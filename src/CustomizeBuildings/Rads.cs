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

        public void Edit(GameObject go)
        {
            var HEP = go.GetComponent<HighEnergyParticleStorage>();
            HEP.capacity = CustomizeBuildingsState.StateManager.State.RadBattery;
        }

        public void Undo(GameObject go)
        {
            throw new NotImplementedException();
        }
    }
}
