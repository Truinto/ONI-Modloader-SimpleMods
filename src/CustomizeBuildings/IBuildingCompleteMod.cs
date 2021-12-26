using UnityEngine;

namespace CustomizeBuildings
{
    public interface IBuildingCompleteMod
    {
        public abstract bool Enabled(string id);

        public abstract void Edit(GameObject go);

        public abstract void Undo(GameObject go);
    }
}
