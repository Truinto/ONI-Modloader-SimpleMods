using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace EasierBuildings
{
    public class SecondaryPortConfig //add this code to the config file
    {
        private ConduitPortInfo secondaryOutPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(2, 0));
        private ConduitPortInfo secondaryInPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(2, 1));

        public void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            // replace the usual class to make manual operated to automated
            go.AddOrGet<WaterPurifier>();

            Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go, false);
            defaultStorage.showInUI = true;
            defaultStorage.capacityKg = 30000f;
            defaultStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);

            //OUTPUT:
            ConduitDispenser2 conduitDispenser2 = go.AddOrGet<ConduitDispenser2>();
            conduitDispenser2.secondaryOutput = this.secondaryOutPort;
            conduitDispenser2.conduitType = this.secondaryOutPort.conduitType;
            conduitDispenser2.invertElementFilter = true;
            conduitDispenser2.elementFilter = new SimHashes[]
            {
                SimHashes.Petroleum,
                SimHashes.CrudeOil
            };

            //INPUT:
            ConduitConsumer2 conduitConsumer2 = go.AddOrGet<ConduitConsumer2>();
            conduitConsumer2.secondaryInput = this.secondaryInPort;
            conduitConsumer2.conduitType = secondaryInPort.conduitType;
            conduitConsumer2.consumptionRate = 10f;
            conduitConsumer2.capacityTag = SimHashes.CrudeOil.CreateTag();
            conduitConsumer2.wrongElementResult = ConduitConsumer2.WrongElementResult.Dump;
            conduitConsumer2.capacityKG = 100f;
            conduitConsumer2.forceAlwaysSatisfied = true;

        }

        public void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            //base.DoPostConfigurePreview(def, go);
            this.AttachPort(go);
        }

        public void DoPostConfigureUnderConstruction(GameObject go)
        {
            //base.DoPostConfigureUnderConstruction(go);
            this.AttachPort(go);
        }

        private void AttachPort(GameObject go)
        {
            go.AddComponent<ConduitSecondaryOutput>().portInfo = this.secondaryOutPort;
            go.AddComponent<ConduitSecondaryInput>().portInfo = this.secondaryInPort;
        }

    }
}