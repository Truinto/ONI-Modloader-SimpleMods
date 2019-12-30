// Decompiled with JetBrains decompiler
// Type: ElectrolyzerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3EAEEF34-517A-43A2-A9FE-A8421AAC144C
// Assembly location: D:\Programme\Steam\SteamApps\common\OxygenNotIncluded\OxygenNotIncluded_Data\Managed\Assembly-CSharp.dll

using Harmony;
using TUNING;
using UnityEngine;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(ElectrolyzerConfig), "ConfigureBuildingTemplate")]
    internal class ElectrolyzerConfig_ConfigureBuildingTemplate
    {

        private static void Postfix(GameObject go)
        {
            Electrolyzer electrolyzer = go.GetComponent<Electrolyzer>();
            if (electrolyzer == null) return;
            electrolyzer.maxMass = CustomizeBuildingsState.StateManager.State.ElectrolizerMaxPressure;
        }
    }
}