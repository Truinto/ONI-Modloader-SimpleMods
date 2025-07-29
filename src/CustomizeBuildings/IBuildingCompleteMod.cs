using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomizeBuildings
{
    public interface IBuildingCompleteMod
    {
        public abstract bool Enabled(string id);

        public abstract void EditDef(BuildingDef def);

        public abstract void EditGO(BuildingDef def);
    }

    public static class Mods
    {
        private static List<IBuildingCompleteMod>? mods;

        public static List<IBuildingCompleteMod> Get => mods ??= Assembly.GetExecutingAssembly().GetTypes().Where(w => typeof(IBuildingCompleteMod).IsAssignableFrom(w) && w.IsClass).Select(s => (IBuildingCompleteMod)Activator.CreateInstance(s)).ToList();
    }
}
