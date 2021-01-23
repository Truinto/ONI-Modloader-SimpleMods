using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomizeCritter
{
    public static class Helper
    {
        public static void ProcessTags(this KPrefabID prefab, string[] tags, bool serialize = false)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                if (tags[i] == "C:")
                    prefab.Tags.Clear();
                else if (tags[i].StartsWith("R:", StringComparison.Ordinal))
                    prefab.RemoveTag(tags[i].Substring(2));
                else
                    prefab.AddTag(tags[i], serialize);
            }
        }

        public static string ToDiseaseId(this byte diseaseIdx)
        {
            if (diseaseIdx < Db.Get().Diseases.Count)
                return Db.Get().Diseases[diseaseIdx].Id;
            else
                return null;
        }

        public static byte ToDiseaseIdx(this string diseaseId)
        {
            return Db.Get().Diseases.GetIndex(diseaseId);
        }

        public static FertilityMonitor.BreedingChance BreedingChance(string tag, float weight)
        {
            return new FertilityMonitor.BreedingChance()
            {
                egg = tag.ToTag(),
                weight = weight
            };
        }
    }
}
