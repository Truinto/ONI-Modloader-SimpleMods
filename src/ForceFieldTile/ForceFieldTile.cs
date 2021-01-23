public class ForceFieldTile : TileTemperature
{
    protected override void OnPrefabInit()
    {
        base.OnPrefabInit();
        /*if (this.isPole)
            Grid.HasPole[Grid.PosToCell((KMonoBehaviour) this)] = true;
        else
            Grid.HasLadder[Grid.PosToCell((KMonoBehaviour) this)] = true;
        this.GetComponent<KPrefabID>().AddTag(GameTags.Ladders);
        Components.Ladders.Add(this);*/
    }

    protected override void OnSpawn()
    {
        base.OnSpawn();
        int placementCell = Grid.PosToCell((KMonoBehaviour)this);
        Grid.FakeFloor[placementCell] = true;
        Pathfinding.Instance.AddDirtyNavGridCell(placementCell);
        Grid.HasDoor[placementCell] = true;
        Grid.RenderedByWorld[placementCell] = false;
        SimMessages.SetCellProperties(placementCell, (byte)8);
        Game.Instance.SetForceField(placementCell, true, Grid.Solid[placementCell]);
        //this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal, (object) null);
    }

    protected override void OnCleanUp()
    {
        int placementCell = Grid.PosToCell((KMonoBehaviour)this);
        SimMessages.ClearCellProperties(placementCell, (byte)12);
        Grid.RenderedByWorld[placementCell] = true; // Grid.Element[placementCell].substance.renderedByWorld;
        Grid.FakeFloor[placementCell] = false;
        Pathfinding.Instance.AddDirtyNavGridCell(placementCell);
        Grid.HasDoor[placementCell] = false;
        Game.Instance.SetForceField(placementCell, false, Grid.Solid[placementCell]);
        base.OnCleanUp();
        /*if (this.isPole)
            Grid.HasPole[Grid.PosToCell((KMonoBehaviour) this)] = false;
        else
            Grid.HasLadder[Grid.PosToCell((KMonoBehaviour) this)] = false;
        Components.Ladders.Remove(this);*/
    }
}

//interesting: SimMessages.ReplaceAndDisplaceElement
// SetSimState SimMessages.ReplaceAndDisplaceElement(cell, component.ElementID, CellEventLogger.Instance.DoorClose, mass, temperature, byte.MaxValue, 0, handle1.index);
// OnCleanUp SimMessages.ReplaceAndDisplaceElement(placementCell, SimHashes.Vacuum, CellEventLogger.Instance.DoorOpen, 0.0f, -1f, byte.MaxValue, 0, -1);