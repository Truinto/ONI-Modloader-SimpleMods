using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Common;
using UnityEngine.UI;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(MaterialSelector), nameof(MaterialSelector.ConfigureScreen))]
    public class MaterialSelector_ConfigureScreen_Patch
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.BuildingAdvancedGlobalFlag;
        }

        public static IEnumerable<CodeInstruction> No_____Transpiler(IEnumerable<CodeInstruction> instr)
        {
            List<CodeInstruction> code = instr.ToList();

            int index = 0;
            //while (code[index++].opcode != OpCodes.Stloc_0) ;
            while (code[index++].opcode != OpCodes.Endfinally) ;

            Debug.Log("MaterialSelector_ConfigureScreen_Patch patched at index: " + index);

            code.Insert(index++, new CodeInstruction(OpCodes.Ldarg_1));
            code.Insert(index++, new CodeInstruction(OpCodes.Ldarg_2));
            code.Insert(index++, new CodeInstruction(OpCodes.Ldloc_0));
            code.Insert(index++, CodeInstruction.Call(typeof(MaterialSelector_ConfigureScreen_Patch), nameof(MaterialSelector_ConfigureScreen_Patch.Change)));
            //code.Insert(index++, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MaterialSelector_ConfigureScreen_Patch), nameof(MaterialSelector_ConfigureScreen_Patch.Change))));

            return code;
        }

        public static void Change(Recipe.Ingredient ingredient, Recipe recipe, List<Tag> list)
        {
            int num = recipe.Ingredients.IndexOf(ingredient);
            Helpers.Print($"MaterialOverride: {recipe.Name} index: {num}");

            var setting = CustomizeBuildingsState.StateManager.State.BuildingAdvancedMaterial.FirstOrDefault(f => f.Id == recipe.Name && (f.Index == null || f.Index == num));
            if (setting != null)
            {
                foreach (Tag mat in setting.MaterialOverride.Split(' '))
                    if (!list.Contains(mat))
                        list.Add(mat);
            }

            Helpers.Print("MaterialSelector_ConfigureScreen_Patch: " + list.Join());
        }

        public static void Postfix(Recipe.Ingredient ingredient, Recipe recipe, MaterialSelector __instance, ToggleGroup ___toggleGroup)
        {
            int num = recipe.Ingredients.IndexOf(ingredient);
            var setting = CustomizeBuildingsState.StateManager.State.BuildingAdvancedMaterial?.FirstOrDefault(f => f.Id == recipe.Result && (f.Index == null || f.Index == num));
            if (setting == null)
                return;

            Helpers.Print("Applying Material Override for " + recipe.Result);
            var oldToggles = __instance.ElementToggles.Keys.ToList();
            __instance.ClearMaterialToggles();

            if (setting.MaterialAppend)
                foreach (Tag elem in oldToggles)
                    addToggle(elem);

            foreach (Tag elem in setting.MaterialOverride.Split(' '))
                addToggle(elem);

            __instance.RefreshToggleContents();

            void addToggle(Tag elem)
            {
                if (!__instance.ElementToggles.ContainsKey(elem))
                {
                    GameObject newToggleGO = Util.KInstantiate(__instance.TogglePrefab, __instance.LayoutContainer, "MaterialSelection_" + elem.ProperName());
                    newToggleGO.transform.localScale = Vector3.one;
                    newToggleGO.SetActive(true);
                    KToggle newToggle = newToggleGO.GetComponent<KToggle>();
                    __instance.ElementToggles.Add(elem, newToggle);
                    newToggle.group = ___toggleGroup;
                    ToolTip tool_tip = newToggleGO.gameObject.GetComponent<ToolTip>();
                    tool_tip.toolTip = elem.ProperName();
                }
            }

        }
    }

    //[HarmonyPatch(typeof(MaterialSelector), nameof(MaterialSelector.RefreshToggleContents))]
    public class MaterialSelector_RefreshToggleContents_Patch
    {

    }
}