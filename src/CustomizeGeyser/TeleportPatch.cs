using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeGeyser
{
    [HarmonyPatch(typeof(DebugHandler), nameof(DebugHandler.OnKeyDown))]
    public class TeleportPatch
    {
        public static bool Prepare()
        {
            return CustomizeGeyserState.StateManager.State.GeyserTeleportEnabled;
        }

        public static void Postfix(KButtonEvent e)
        {
            if (!e.Consumed && e.IsAction(Action.DebugTeleport))
            {
                if (SelectTool.Instance?.selected?.gameObject.GetComponent<GeyserConfigurator>() == null)
                    return;

                int mouseCell = DebugHandler.GetMouseCell();
                if (!Grid.IsValidBuildingCell(mouseCell))
                    return;

                SelectTool.Instance.selected.transform.SetPosition(Grid.CellToPosCBC(mouseCell, Grid.SceneLayer.Move));

                e.Consumed = true;
                //PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, STRINGS.UI.DEBUG_TOOLS.INVALID_LOCATION, null, DebugHandler.GetMousePos(), 1.5f, false, true);
            }
        }
    }
}
