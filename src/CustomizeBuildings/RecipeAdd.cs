using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using HarmonyLib;
using UnityEngine;

namespace CustomizeBuildings
{

    [HarmonyPatch(typeof(RockCrusherConfig), "ConfigureBuildingTemplate")]
    public class RockCrusherConfig_ConfigureBuildingTemplate    // TODO: remove
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.NewRecipeRockCrusher;
        }

        public static void Postfix()
        {
            if (CustomizeBuildingsState.StateManager.State.NewRecipeRockCrusher)
            {
                string workstation = "RockCrusher"; // "Kiln"

                Tag ingredient1 = SimHashes.Regolith.CreateTag();
                float amountIn1 = 100f;
                Tag result1 = SimHashes.Sand.CreateTag();
                float amountOut1 = 99f;

                ComplexRecipe.RecipeElement[] ingredients1 = new ComplexRecipe.RecipeElement[] { new ComplexRecipe.RecipeElement(ingredient1, amountIn1) };
                ComplexRecipe.RecipeElement[] results1 = new ComplexRecipe.RecipeElement[] { new ComplexRecipe.RecipeElement(result1, amountOut1) };

                string obsolete_id1 = ComplexRecipeManager.MakeObsoleteRecipeID(workstation, result1);
                string str1 = ComplexRecipeManager.MakeRecipeID(workstation, (IList<ComplexRecipe.RecipeElement>)ingredients1, (IList<ComplexRecipe.RecipeElement>)results1);
                ComplexRecipe complexRecipe1 = new ComplexRecipe(str1, ingredients1, results1)
                {
                    time = 40f,
                    description = "Turns Regolith into Sand and Dirt.",
                    fabricators = new List<Tag>() { TagManager.Create(workstation) },
                    nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult
                };
                ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id1, str1);
            }
        }
    }
}