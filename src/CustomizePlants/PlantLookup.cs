using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using static Config.PostBootDialog;
using System.Reflection;

namespace CustomizePlants
{
    // prints out list of all objects that run ExtendEntityToBasicPlant
    [HarmonyPatch(typeof(EntityTemplates), "ExtendEntityToBasicPlant")]
    public static class PlantLookupPatch
    {
        public static System.Text.RegularExpressions.Regex FindBetweenLink = new System.Text.RegularExpressions.Regex(@">(.*)<", System.Text.RegularExpressions.RegexOptions.Compiled);

        public static bool AutomaticallyAdd = false;

        public static void Prefix(GameObject template)
        {
            if (CustomizePlantsState.StateManager.State.AutomaticallyAddModPlants == true)
            {
                AutomaticallyAdd = true;
                CustomizePlantsState.StateManager.State.AutomaticallyAddModPlants = false;
            }

            Type classType = null;
            var frames = new System.Diagnostics.StackTrace().GetFrames();
            for (int i = 2; i < frames.Length; i++)
            {
                classType = frames[i].GetMethod().ReflectedType;
                var prefab = classType.GetMethod("CreatePrefab");
                if (prefab != null)
                    break;
            }

            string displayName = FindBetweenLink.Match(template.GetComponent<DecorProvider>()?.overrideName ?? "null")?.Groups[1]?.Value ?? "null";
            string className = classType.AssemblyQualifiedName;
            string plantName = template.name;

            Debug.Log($"[CustomizePlants] '{displayName}' is {plantName} defined in '{className}'");

            int index_c = className.IndexOf(", Version=");
            if (index_c > 0)
                className.Substring(0, index_c);

            if (AutomaticallyAdd && !className.Contains(", Assembly-CSharp") && !CustomizePlantsState.StateManager.State.ModPlants.Contains(className))
            {
                Debug.Log("Found new mod plant and adding it to the list: " + className);
                CustomizePlantsState.StateManager.State.IgnoreList.Add(plantName);
                CustomizePlantsState.StateManager.State.ModPlants.Add(className);
                CustomizePlantsState.StateManager.State.PlantSettings.Add(new PlantData(plantName));
                CustomizePlantsState.StateManager.TrySaveConfigurationState();
            }
        }
    }

    public class PlantInfo
    {
        string displayName;
        string className;
        string plantName;

        public PlantInfo() { }
        public PlantInfo(string displayName, string className, string plantName)
        {
            this.displayName = displayName;
            this.className = className;
            this.plantName = plantName;
        }
    }

}