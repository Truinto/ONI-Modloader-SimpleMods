using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System;
using Common;

namespace CustomizeRecipe
{
    //[HarmonyPatch(typeof(ComplexFabricator), "StartWorkingOrder")]
    public class BugSearch
    {
        public static Dictionary<ComplexFabricator, int> Handles = new Dictionary<ComplexFabricator, int>();

        public static void Prefix(int index, ComplexFabricator __instance, int ___workingOrderIdx, List<int> ___openOrderCounts)
        {
            if (!Handles.ContainsKey(__instance))
                Handles[__instance] = __instance.Subscribe(1721324763, new Action<object>(Update));
            Helpers.Print($"___openOrderCounts = {___openOrderCounts[index]}");
        }

        public static void Update(object nix)
        {
            Helpers.Print($"Update {nix}");
        }
    }

    [HarmonyPatch(typeof(Assets), "CreatePrefabs")]
    [HarmonyPriority(Priority.LowerThanNormal)]
    public class RecipePatch
    {
        public static ComplexRecipe.RecipeElement[] EmptyIngredient = Array.Empty<ComplexRecipe.RecipeElement>();

        [HarmonyPostfix]
        public static void Load()
        {
#if DEBUG
            foreach (var recipe in ComplexRecipeManager.Get().recipes)
                Helpers.Print($"id: {recipe.id}, description: {recipe.description}, building: {recipe.fabricators.FirstOrDefault()}");
#endif
            foreach (var setting in CustomizeRecipeState.StateManager.State.RecipeSettings)
                Process(setting);

            if (CustomizeRecipeState.StateManager.State.CheatFast)
                ComplexRecipeManager.Get().recipes.ForEach(a => a.time = 5f);

            if (CustomizeRecipeState.StateManager.State.CheatFree)
                foreach (var recipe in ComplexRecipeManager.Get().recipes)
                    for (int i = 0; i < recipe.ingredients.Length; i++)
                        recipe.ingredients[i] = new ComplexRecipe.RecipeElement(recipe.ingredients[i].material, 0f, recipe.ingredients[i].temperatureOperation, recipe.ingredients[i].storeElement);
        }

        public static void Process(RecipeData setting)
        {
            // try to fill out ID
            if (setting.Id == null && setting.Building != null && setting.Inputs != null && setting.Outputs != null)
                setting.Id = ComplexRecipeManager.MakeRecipeID(setting.Building, setting.InputsList, setting.OutputsList);

            if (setting.Id == null)
            {
                Config.PostBootDialog.ErrorList.Add("Missing recipe ID, skipping entry.");
                return;
            }

            // check ID formatting
            if (Assets.TryGetPrefab(setting.Id.TrySubstring('_')) == null)
            {
                Config.PostBootDialog.ErrorList.Add($"Recipe ID is invalid. Must start with building and underscore, like this \"{RockCrusherConfig.ID}_\"");
                return;
            }

            // try to find recipe, if non found try to generate a new one
            var recipe = ComplexRecipeManager.Get().recipes.Find(f => f.id == setting.Id);
            if (recipe == null)
            {
                if (setting.Building == null || setting.Inputs == null || setting.Outputs == null)
                {
                    Config.PostBootDialog.ErrorList.Add("Trying to generate new recipe, but could not parse building, inputs, or outputs: " + recipe.id);
                    return;
                }

                recipe = new ComplexRecipe(setting.Id, EmptyIngredient, EmptyIngredient)
                {
                    time = 40f,
                    fabricators = new List<Tag>() { setting.Building.ToTag() },
                    nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult
                };
            }

            // apply modifications
            if (setting.Description != null)
                recipe.description = setting.Description;
            if (setting.Inputs != null)
                recipe.ingredients = setting.InputsArray;
            if (setting.Outputs != null)
                recipe.results = setting.OutputsArray;
            if (setting.Time != null)
                recipe.time = setting.Time.Value;
            if (setting.HEP != null)
                recipe.consumedHEP = setting.HEP.Value;
        }
    
        public static void Print()
        {
            CustomizeRecipeState.StateManager.State.RecipeSettings.Clear();
            foreach (var recipe in ComplexRecipeManager.Get().recipes)
            {
                RecipeData item = new RecipeData(recipe.id, recipe.fabricators.FirstOrDefault().ToString(), recipe.time, recipe.consumedHEP, recipe.description, recipe.ingredients, recipe.results);
                CustomizeRecipeState.StateManager.State.RecipeSettings.Add(item);
            }
        }
    }

    [HarmonyPatch(typeof(PrimaryElement), "SetTemperature")]
    public class TemperatureFix
    {
        public static bool Prepare()
        {
            return CustomizeRecipeState.StateManager.State.CheatFree || CustomizeRecipeState.StateManager.State.AllowZeroInput;
        }

        public static bool Prefix(PrimaryElement __instance, float temperature)
        {
            if (float.IsNaN(temperature) || float.IsInfinity(temperature) || temperature <= 0f)
                temperature = __instance.Element.defaultValues.temperature;
            __instance.setTemperatureCallback(__instance, temperature);
            return false;
        }
    }

    [HarmonyPatch(typeof(ComplexFabricator), "StartWorkingOrder")]
    public class FetchFix
    {
        public static bool Prepare()
        {
            return CustomizeRecipeState.StateManager.State.CheatFree || CustomizeRecipeState.StateManager.State.AllowZeroInput;
        }

        public static void Prefix(int index, ComplexFabricator __instance, List<int> ___openOrderCounts)
        {
            if (___openOrderCounts[index] <= 0)
            ___openOrderCounts[index] = 1;
        }
    }
}
