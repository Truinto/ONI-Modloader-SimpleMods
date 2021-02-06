using Harmony;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System;
using KSerialization;

namespace CustomizeBuildings
{
    //[HarmonyPatch(typeof(Filterable), nameof(Filterable.GetTagOptions))]
    public class Filterable_Patch
    {
        public static bool Prefix(Dictionary<Tag, HashSet<Tag>> __result, Filterable __instance)
        {
            if (__instance.filterElementState == Filterable.ElementState.None)
            {
                
            }
            return true;
        }
    }



    //[HarmonyPatch(typeof(ResetSkillsStation), "OnCompleteWork")]
    public class ResetSkillsStation_Patch
    {
        public static bool Prefix(Worker worker, ResetSkillsStation __instance)
        {
            __instance.assignable.Unassign();
            var resume = worker.GetComponent<MinionResume>();
            var select = __instance.GetComponent<SkillStationSelection>();

            return false;
        }
    }

    public class SkillStationSelection : KMonoBehaviour
    {
        public string Selection;
    }

    public class SkillStationSideScreen : SideScreenContent
    {
        private GameObject station;

        public List<FilterSideScreenRow> Options = new List<FilterSideScreenRow>();

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
        }

        public override string GetTitle()
        {
            return "SkillStationSideScreen";
        }

        public override bool IsValidForTarget(GameObject target)
        {
            return target.GetComponent<ResetSkillsStation>();
        }

        public override void SetTarget(GameObject target)
        {
            this.station = target;
            //target.GetComponent<Ownable>();
            //target.GetComponent<ResetSkillsStation>();
        }

        public override void ClearTarget()
        {
            this.station = null;
        }
    }
}