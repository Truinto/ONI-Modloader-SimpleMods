using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System;
using Common;
using Shared.CollectionNS;

namespace CustomizeRecipe
{
    //[HarmonyPatch(typeof(ComplexFabricator), nameof(ComplexFabricator.StartWorkingOrder))]
    public class BugSearch
    {
        public static Dictionary<ComplexFabricator, int> Handles = new();

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

    //[HarmonyPatch(typeof(ComplexFabricator), nameof(ComplexFabricator.ValidateNextOrder))]
    public class BugSearch2
    {
        public static void Prefix(ComplexFabricator __instance)
        {
            Helpers.Print($"name={__instance.name} go={__instance.gameObject.name} len={__instance.recipe_list.Length} order={__instance.nextOrderIdx}");
        }
    }

    [HarmonyPatch(typeof(ComplexRecipeManager), nameof(ComplexRecipeManager.PostProcess))]
    [HarmonyPriority(Priority.LowerThanNormal)]
    public class RecipePatch
    {
        [HarmonyPrefix]
        public static void Load()
        {
#if DEBUG
            foreach (var recipe in ComplexRecipeManager.Get().preProcessRecipes)
                Helpers.Print($"id: {recipe.id}, description: {recipe.description}, building: {recipe.fabricators.FirstOrDefault()}");
#endif
            UnlockStorage();

            foreach (var setting in CustomizeRecipeState.StateManager.State.RecipeSettings)
                Process(setting);

            if (CustomizeRecipeState.StateManager.State.CheatFast)
            {
                foreach (var recipe in ComplexRecipeManager.Get().preProcessRecipes)
                {
                    recipe.time = 5f;
                }
            }

            if (CustomizeRecipeState.StateManager.State.CheatFree)
            {
                foreach (var recipe in ComplexRecipeManager.Get().preProcessRecipes)
                {
                    for (int i = 0; i < recipe.ingredients.Length; i++)
                    {
                        recipe.ingredients[i].amount = 0f;
                        if (recipe.ingredients[i].possibleMaterialAmounts != null)
                            for (int j = 0; j < recipe.ingredients[i].possibleMaterialAmounts.Length; j++)
                                recipe.ingredients[i].possibleMaterialAmounts[j] = 0f;
                    }
                }
            }
        }

        /// <summary>
        /// Fix some buildings always store produce and overwrite recipe instead.
        /// </summary>
        public static void UnlockStorage()
        {
            var recipes = ComplexRecipeManager.Get().preProcessRecipes;

            foreach (var building in Assets.BuildingDefs)
            {
                if (building is null)
                    continue;
                var fabricator = building.BuildingComplete?.GetComponent<ComplexFabricator>();
                if (fabricator == null || fabricator.storeProduced != true)
                    continue;

                foreach (var recipe in recipes)
                {
                    if (recipe.fabricators.Contains(building.Tag))
                    {
                        foreach (var output in recipe.results)
                        {
                            var element = output.material.ToElement();
                            if (element != null && element.IsLiquid)
                            {
                                output.storeElement = true;
                                fabricator.storeProduced = false;
                            }
                        }
                    }
                }
            }
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

            // try to find recipe by ID
            ComplexRecipe? recipe = null;
            foreach (var preProcessedRecipe in ComplexRecipeManager.Get().preProcessRecipes)
            {
                if (preProcessedRecipe.id == setting.Id)
                {
                    recipe = preProcessedRecipe;
                    break;
                }
            }

            // generate new recipe, since ID wasn't found
            if (recipe == null)
            {
                if (setting.Building == null || setting.Inputs == null || setting.Outputs == null)
                {
                    Config.PostBootDialog.ErrorList.Add("Trying to generate new recipe, but could not parse building, inputs, or outputs: " + recipe.id);
                    return;
                }
                Helpers.Print($"Making new Recipe: {setting.Id}");
                recipe = new ComplexRecipe(setting.Id, setting.InputsArray, setting.OutputsArray)
                {
                    time = 40f,
                    fabricators = new List<Tag>() { setting.Building.ToTag() },
                    nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult,
                    description = "",
                };
            }

            // apply modifications
            if (setting.NameDisplay != null)
                recipe.nameDisplay = setting.NameDisplay.Value;
            if (setting.CustomSpritePrefabID != null)
                recipe.customSpritePrefabID = setting.CustomSpritePrefabID;
            if (setting.CustomName != null)
                recipe.customName = setting.CustomName;
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
            if (setting.HEPout != null)
                recipe.producedHEP = setting.HEPout.Value;
            recipe.description ??= "";
        }

        public static void Print()
        {
            CustomizeRecipeState.StateManager.State.RecipeSettings.Clear();
            foreach (var recipe in ComplexRecipeManager.Get().preProcessRecipes)
            {
                var item = new RecipeData()
                {
                    Id = recipe.id,
                    Building = recipe.fabricators.FirstOrDefault().ToString(),
                    Time = recipe.time,
                    HEP = recipe.consumedHEP > 0 ? recipe.consumedHEP : null,
                    HEPout = recipe.producedHEP > 0 ? recipe.producedHEP : null,
                    NameDisplay = recipe.nameDisplay,
                    CustomName = recipe.customName,
                    CustomSpritePrefabID = recipe.customSpritePrefabID,
                    Description = recipe.description,
                    Inputs = { recipe.ingredients.Select(s => (RecipeData.RecipeElement)s) },
                    Outputs = { recipe.results.Select(s => (RecipeData.RecipeElement)s) },
                };
                CustomizeRecipeState.StateManager.State.RecipeSettings.Add(item);
            }
        }
    }

    [HarmonyPatch(typeof(PrimaryElement), nameof(PrimaryElement.SetTemperature))]
    public class TemperatureFix
    {
        public static bool Prefix(PrimaryElement __instance, float temperature)
        {
            if (float.IsNaN(temperature) || float.IsInfinity(temperature) || temperature <= 0f)
                temperature = __instance.Element.defaultValues.temperature;
            __instance.setTemperatureCallback(__instance, temperature);
            return false;
        }
    }

    [HarmonyPatch(typeof(ComplexFabricator), nameof(ComplexFabricator.StartWorkingOrder))]
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
