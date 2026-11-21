using System;
using HarmonyLib;
using UnityEngine;
using Common;

#pragma warning disable IDE1006 // ignore naming styles for compatibility reasons

namespace CustomizePlants
{
    // prints out list of all objects that run ExtendEntityToBasicPlant
    [HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.ExtendEntityToBasicPlant),
        typeof(GameObject), typeof(Crop.CropVal), typeof(float), typeof(float), typeof(float), typeof(float),
        typeof(SimHashes[]), typeof(bool), typeof(float), typeof(float), typeof(bool), typeof(bool), typeof(bool),
        typeof(bool), typeof(bool), typeof(float), typeof(float), typeof(float), typeof(string), typeof(string))]

    public static class PlantLookupPatch
    {
        public static System.Text.RegularExpressions.Regex FindBetweenLink = new(@">(.*)<", System.Text.RegularExpressions.RegexOptions.Compiled);

        public static bool AutomaticallyAdd = false;

        public static void Prefix(GameObject template)
        {
            if (CustomizePlantsState.Instance.AutomaticallyAddModPlants == true)
            {
                AutomaticallyAdd = true;
                CustomizePlantsState.Instance.AutomaticallyAddModPlants = false;
            }

            Type? classType = null;
            var frames = new System.Diagnostics.StackTrace().GetFrames();
            for (int i = 2; i < frames.Length; i++)
            {
                classType = frames[i].GetMethod().ReflectedType;
                var prefab = classType.GetMethod("CreatePrefab");
                if (prefab != null)
                    break;
            }

            var match = FindBetweenLink.Match(template.GetProperName() ?? "null");
            string displayName = match.Success ? match.Groups[1].Value : "";
            string className = classType?.AssemblyQualifiedName ?? "";
            string plantName = template.name;

            Helpers.Print($"[CustomizePlants] '{displayName}' is {plantName} defined in '{className}'");

            int index_c = className.IndexOf(", Version=");
            if (index_c > 0)
                className.Substring(0, index_c);

            if (AutomaticallyAdd && !className.Contains(", Assembly-CSharp") && !CustomizePlantsState.Instance.ModPlants.Contains(className))
            {
                Helpers.Print("Found new mod plant and adding it to the list: " + className);
                CustomizePlantsState.Instance.IgnoreList.Add(plantName);
                CustomizePlantsState.Instance.ModPlants.Add(className);
                CustomizePlantsState.Instance.PlantSettings.Add(new PlantData(plantName));
                CustomizePlantsState.Instance.TrySave();
            }
        }
    }
}
