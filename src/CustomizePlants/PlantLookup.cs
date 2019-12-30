using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;
using static BootDialog.PostBootDialog;
using System.Reflection;

namespace CustomizePlants
{
    // prints out list of all objects that run ExtendEntityToBasicPlant
    [HarmonyPatch(typeof(EntityTemplates), "ExtendEntityToBasicPlant")]
    public static class PlantLookupPatch
    {
        public static System.Text.RegularExpressions.Regex FindBetweenLink = new System.Text.RegularExpressions.Regex(@">(.*)<", System.Text.RegularExpressions.RegexOptions.Compiled);
        
        public static void Prefix(GameObject template)
        {
            MethodBase classBase = new System.Diagnostics.StackTrace().GetFrame(2).GetMethod();

            string displayName = FindBetweenLink.Match(template.GetComponent<DecorProvider>()?.overrideName ?? "null")?.Groups[1]?.Value ?? "null";
            string className = classBase.ReflectedType.Name;
            string plantName = template.name;

            Debug.Log("Debug CustomizePlants: '" + displayName + "' is " + plantName + " defined in " + className);

            if (CustomizePlantsState.StateManager.State.AutomaticallyAddModPlants && !PLANTS.METHODS.Contains(classBase.GetType()))
            {
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