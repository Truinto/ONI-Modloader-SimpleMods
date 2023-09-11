using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Common;

namespace PipedEverything
{
    [SkipSaveFileSerialization]
    internal class PortDisplayController : KMonoBehaviour
    {
        [SerializeField]
        private HashedString lastMode = OverlayModes.None.ID;

        [SerializeField]
        private List<PortDisplay2> outputPorts = new();

        [SerializeField]
        private List<PortDisplay2> gasOverlay = new();

        [SerializeField]
        private List<PortDisplay2> liquidOverlay = new();

        [SerializeField]
        private List<PortDisplay2> solidOverlay = new();

        [MyCmpReq]
        private Operational operational;

        public override void OnSpawn()
        {
            base.OnSpawn();

            if (outputPorts.Count > 0)
            {
                base.Subscribe((int)GameHashes.OnStorageChange, o =>
                {
                    this.operational.SetFlag(ConduitDispenser.outputConduitFlag, !AnyBlockedOutput());
                });
            }
        }

        public void AssignPort(GameObject go, PortDisplayInfo port)
        {
            PortDisplay2 portDisplay = go.AddComponent<PortDisplay2>();
            portDisplay.AssignPort(port);

            switch (port.type)
            {
                case ConduitType.Gas:
                    this.gasOverlay.Add(portDisplay);
                    break;
                case ConduitType.Liquid:
                    this.liquidOverlay.Add(portDisplay);
                    break;
                case ConduitType.Solid:
                    this.solidOverlay.Add(portDisplay);
                    break;
            }

            if (port.input == false)
                outputPorts.Add(portDisplay);

            // criteria for drawing port icons on buildings
            // vanilla will only attempt to draw icons on buildings with BuildingCellVisualizer
            go.AddOrGet<BuildingCellVisualizer>();

            // when vanilla tries to draw, call this controller if the building is in the DrawPorts list
            string ID = go.GetComponent<KPrefabID>().PrefabTag.Name;
            Patches.DrawBuildings.Add(ID);
        }

        public bool Draw(BuildingCellVisualizer __instance, HashedString mode, GameObject go)
        {
            bool isNewMode = mode != this.lastMode;

            if (isNewMode)
            {
                ClearPorts();
                this.lastMode = mode;
            }

            foreach (PortDisplay2 port in GetPorts(mode))
            {
                port.Draw(go, __instance, isNewMode);
            }

            return true;
        }

        private void ClearPorts()
        {
            foreach (PortDisplay2 port in GetPorts(this.lastMode))
            {
                port.DisableIcons();
            }
        }

        private List<PortDisplay2> GetPorts(HashedString mode)
        {
            if (mode == OverlayModes.GasConduits.ID) return this.gasOverlay;
            if (mode == OverlayModes.LiquidConduits.ID) return this.liquidOverlay;
            if (mode == OverlayModes.SolidConveyor.ID) return this.solidOverlay;

            return new List<PortDisplay2>();
        }

        public bool IsInputConnected(Element element)
        {
            var hash = element.id;
            foreach (var port in element.IsGas ? gasOverlay : element.IsLiquid ? liquidOverlay : solidOverlay)
            {
                if (port.input && port.filter.Contains(hash))
                    return port.IsConnected();
            }
            return false;
        }

        public bool IsOutputConnected(Element element)
        {
            var hash = element.id;
            foreach (var port in element.IsGas ? gasOverlay : element.IsLiquid ? liquidOverlay : solidOverlay)
            {
                if (!port.input && port.filter.Contains(hash))
                    return port.IsConnected();
            }
            return false;
        }

        public bool CanStore(Element element)
        {
            var port = GetPort(false, element.GetConduitType(), element.id);
            return port != null && port.IsConnected() && port.GetCapacity(element.id) > 0f;
        }

        public bool AnyBlockedOutput()
        {
            foreach (var port in outputPorts)
            {
                if (port.IsBlocked())
                    return true;
            }
            return false;
        }

        public PortDisplay2 GetPort(bool input, ConduitType conduitType, SimHashes hash)
        {
            foreach (var port in conduitType == ConduitType.Gas ? gasOverlay : conduitType == ConduitType.Liquid ? liquidOverlay : solidOverlay)
            {
                if (port.input == input && port.filter.Contains(hash))
                    return port;
            }
            return null;
        }
    }
}
