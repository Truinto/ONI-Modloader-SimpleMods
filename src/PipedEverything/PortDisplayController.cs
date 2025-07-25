﻿using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Common;

namespace PipedEverything
{
    [SkipSaveFileSerialization]
    public class PortDisplayController : KMonoBehaviour
    {
        [SerializeField]
        private HashedString lastMode = OverlayModes.None.ID;

        [SerializeField]
        public List<PortDisplay2> outputPorts = new();

        [SerializeField]
        public List<PortDisplay2> gasOverlay = new();

        [SerializeField]
        public List<PortDisplay2> liquidOverlay = new();

        [SerializeField]
        public List<PortDisplay2> solidOverlay = new();

        [MyCmpGet]
        private Operational? operational;

        public override void OnSpawn()
        {
            base.OnSpawn();

            if (this.operational != null && outputPorts.Count > 0)
            {
                base.Subscribe((int)GameHashes.OnStorageChange, o =>
                {
                    this.operational.SetFlag(ConduitDispenser.outputConduitFlag, !AnyBlockedOutput());
                });
            }
        }

        public void AddPort(GameObject go, PortDisplayInfo port)
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
            if (go.GetComponent<Building>() != null)
                go.AddOrGet<BuildingCellVisualizer>();
            else
                go.AddOrGet<EntityCellVisualizer>();
        }

        public bool Draw(EntityCellVisualizer __instance, HashedString mode)
        {
            bool isNewMode = mode != this.lastMode;

            if (isNewMode)
            {
                ClearPorts();
                this.lastMode = mode;
            }

            foreach (PortDisplay2 port in GetPorts(mode))
            {
                port.Draw(__instance, isNewMode);
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
            if (element == null)
                return false;
            var hash = element.id;
            foreach (var port in element.IsGas ? gasOverlay : element.IsLiquid ? liquidOverlay : solidOverlay)
            {
                if (port.input && (port.filter.Contains(SimHashes.Void) && element.GetConduitType() == port.type || port.filter.Contains(hash)))
                    return port.IsConnected();
            }
            return false;
        }

        public bool IsOutputConnected(Element element)
        {
            if (element == null)
                return false;
            var hash = element.id;
            foreach (var port in element.IsGas ? gasOverlay : element.IsLiquid ? liquidOverlay : solidOverlay)
            {
                if (!port.input && (port.filter.Contains(SimHashes.Void) && element.GetConduitType() == port.type || port.filter.Contains(hash)))
                    return port.IsConnected();
            }
            return false;
        }

        public bool CanStore(Element element)
        {
            if (element == null)
                return false;
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

        public PortDisplay2? GetPort(bool input, ConduitType conduitType, SimHashes hash)
        {
            foreach (var port in conduitType == ConduitType.Gas ? gasOverlay : conduitType == ConduitType.Liquid ? liquidOverlay : solidOverlay)
            {
                if (port.input == input && (port.filter.Contains(SimHashes.Void) || port.filter.Contains(hash) || hash == SimHashes.Void))
                    return port;
            }
            return null;
        }
    }
}
