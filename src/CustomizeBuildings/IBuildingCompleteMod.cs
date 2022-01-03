using UnityEngine;

namespace CustomizeBuildings
{
    public interface IBuildingCompleteMod
    {
        public abstract bool Enabled(string id);

        public abstract void Edit(BuildingDef def);

        public abstract void Undo(BuildingDef def);
    }
}
