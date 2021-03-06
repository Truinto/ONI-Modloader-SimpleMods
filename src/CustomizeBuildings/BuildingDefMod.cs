﻿using Harmony;
using UnityEngine;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using Common;
using Config;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(BuildingConfigManager), "RegisterBuilding")]
    public class BuildingConfigManager_RegisterBuilding
    {
        public static Regex FindBetweenLink = new Regex(@">(.*)<", RegexOptions.Compiled);

        public static void CreateBuildingDefOverride(BuildingDef buildingDef)
        {
            Helpers.PrintDebug(buildingDef.PrefabID + "\t\tName: " + FindBetweenLink.Match(buildingDef.Name).Groups[1].Value);
            //Debug.Log(buildingDef.BuildLocationRule.GetType().FullName);
            //Debug.Log(buildingDef.BuildingComplete.GetType().AssemblyQualifiedName);
            CustomizeBuildingsState.BuildingStruct entry;

            bool flag = CustomizeBuildingsState.StateManager.State.BuildingBaseSettings.TryGetValue(buildingDef.PrefabID, out entry);
            if (flag == false)
                flag = CustomizeBuildingsState.StateManager.State.BuildingBaseSettings.TryGetValue(FindBetweenLink.Match(buildingDef.Name).Groups[1].Value, out entry);

            if (flag)
            {
                #region checks


                if (entry.PowerConsumption != null)
                {
                    buildingDef.EnergyConsumptionWhenActive = (float)entry.PowerConsumption;
                    buildingDef.RequiresPowerInput = entry.PowerConsumption != 0f;
                }

                if (entry.OverheatTemperature != null)
                {
                    buildingDef.OverheatTemperature = (float)entry.OverheatTemperature;
                }

                if (entry.ExhaustKilowattsWhenActive != null)
                {
                    buildingDef.ExhaustKilowattsWhenActive = (float)entry.ExhaustKilowattsWhenActive;
                }

                if (entry.SelfHeatKilowattsWhenActive != null)
                {
                    buildingDef.SelfHeatKilowattsWhenActive = (float)entry.SelfHeatKilowattsWhenActive;
                }

                if (entry.GeneratorWattageRating != null)
                {
                    buildingDef.GeneratorWattageRating = (float)entry.GeneratorWattageRating;
                }

                if (entry.GeneratorBaseCapacity != null)
                {
                    buildingDef.GeneratorBaseCapacity = (float)entry.GeneratorBaseCapacity;
                }

                if (entry.BaseDecor != null)
                {
                    buildingDef.BaseDecor = (float)entry.BaseDecor;
                }

                if (entry.BaseDecorRadius != null)
                {
                    buildingDef.BaseDecorRadius = (float)entry.BaseDecorRadius;
                }

                if (entry.LocationRule != null)
                {
                    buildingDef.BuildLocationRule = (BuildLocationRule)entry.LocationRule;
                    if (entry.LocationRule == BuildLocationRule.Anywhere)
                        buildingDef.ContinuouslyCheckFoundation = false;
                }

                if (entry.Rotations != null)
                {
                    buildingDef.PermittedRotations = (PermittedRotations)entry.Rotations;
                }

                if (entry.Floodable != null)
                {
                    buildingDef.Floodable = (bool)entry.Floodable;
                }

                if (entry.IsFoundation != null)
                {
                    buildingDef.IsFoundation = (bool)entry.IsFoundation;
                }

                if (entry.MaterialCategory != null)
                {
                    buildingDef.MaterialCategory = entry.MaterialCategory.Split(' ');
                }

                if (entry.ConstructionMass != null)
                {
                    buildingDef.Mass = entry.ConstructionMass;
                }

                if (entry.ThermalConductivity != null)
                {
                    buildingDef.ThermalConductivity = (float)entry.ThermalConductivity;
                }

                #endregion

                // public bool Cancellable = true;
                // public bool OnePerWorld = false;

            }
        }

        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.BuildingBaseSettingGlobalFlag;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            List<CodeInstruction> code = instr.ToList();

            int index = 0;
            while (code[index++].opcode != OpCodes.Stloc_0) ;

            Debug.Log("BuildingConfigManager_RegisterBuilding patched at index: " + index);

            code.Insert(index++, new CodeInstruction(OpCodes.Ldloc_0));
            code.Insert(index++, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BuildingConfigManager_RegisterBuilding), "CreateBuildingDefOverride", new Type[] { typeof(BuildingDef) })));

            //for (int i = 0; i < code.Count; i++)
            //{
            //    CodeInstruction codeInstruction = code[i];
            //    Debug.Log(codeInstruction.ToString());
            //    //Debug.Log("opcode: " + codeInstruction.opcode
            //    //    + "\t\toperand: " + codeInstruction.operand);
            //}

            return (IEnumerable<CodeInstruction>)code;
        }
    }

    [HarmonyPatch(typeof(Assets), "AddBuildingDef")]
    public class Assets_AddBuildingDef
    {
        private static void Prefix(BuildingDef def)
        {
            MuliplierHelper.ProcessPost(def);
            AdvancedHelper.Process(def);

            //if (def.PrefabID == CreatureDeliveryPointConfig.ID)
            //    if (!TUNING.STORAGEFILTERS.BAGABLE_CREATURES.Contains(GameTags.Egg))
            //        TUNING.STORAGEFILTERS.BAGABLE_CREATURES.Add(GameTags.Egg);

        }
    }

    public class MuliplierHelper
    {
        public static void ProcessPost(BuildingDef buildingDef)
        {
            float multiplier;
            bool flag = CustomizeBuildingsState.StateManager.State.MachineMultiplier.TryGetValue(buildingDef.PrefabID, out multiplier);
            if (!flag)
            {
                flag = CustomizeBuildingsState.StateManager.State.MachineMultiplier.TryGetValue(BuildingConfigManager_RegisterBuilding.FindBetweenLink.Match(buildingDef.Name).Groups[1].Value, out multiplier);
            }
            if (flag)
            {
                ElementConverter[] converters = buildingDef.BuildingComplete.GetComponents<ElementConverter>();
                for (int j = 0; j < converters.Count(); j++)
                {
                    for (int i = 0; i < converters[j].consumedElements.Count(); i++)
                        converters[j].consumedElements[i].massConsumptionRate *= multiplier;
                    for (int i = 0; i < converters[j].outputElements.Count(); i++)
                        converters[j].outputElements[i].massGenerationRate *= multiplier;
                    Debug.Log("Multiplier: " + buildingDef.PrefabID + " x" + multiplier);
                }

                Storage[] storages = buildingDef.BuildingComplete.GetComponents<Storage>();
                for (int i = 0; i < storages.Count(); i++)
                {
                    storages[i].capacityKg *= multiplier;
                }

                ManualDeliveryKG[] manualDeliveryKGs = buildingDef.BuildingComplete.GetComponents<ManualDeliveryKG>();
                for (int i = 0; i < manualDeliveryKGs.Count(); i++)
                {
                    manualDeliveryKGs[i].capacity *= multiplier;
                }

                ConduitConsumer[] conduitConsumer = buildingDef.BuildingComplete.GetComponents<ConduitConsumer>();
                for (int i = 0; i < conduitConsumer.Count(); i++)
                {
                    conduitConsumer[i].capacityKG *= multiplier;
                    conduitConsumer[i].consumptionRate *= multiplier;
                }

                PassiveElementConsumer[] elementConsumer = buildingDef.BuildingComplete.GetComponents<PassiveElementConsumer>();
                for (int i = 0; i < elementConsumer.Count(); i++)
                {
                    elementConsumer[i].consumptionRate *= multiplier;
                    elementConsumer[i].capacityKG *= multiplier;
                }

                BuildingElementEmitter[] buildingElementEmitter = buildingDef.BuildingComplete.GetComponents<BuildingElementEmitter>();
                for (int i = 0; i < elementConsumer.Count(); i++)
                {
                    buildingElementEmitter[i].emitRate *= multiplier;
                }

                AlgaeDistillery[] algaeDistillery = buildingDef.BuildingComplete.GetComponents<AlgaeDistillery>();
                for (int i = 0; i < elementConsumer.Count(); i++)
                {
                    algaeDistillery[i].emitMass *= multiplier;
                }

                ElementDropper[] elementDropper = buildingDef.BuildingComplete.GetComponents<ElementDropper>();
                for (int i = 0; i < elementConsumer.Count(); i++)
                {
                    elementDropper[i].emitMass *= multiplier;
                }

            }
        }
    }

    public class AdvancedHelper
    {
        public static void Process(BuildingDef def)
        {
            if (CustomizeBuildingsState.StateManager.State.AdvancedSettings == null) return;

            Dictionary<string, Dictionary<string, object>> buildingEntry;
            bool foundBuilding = CustomizeBuildingsState.StateManager.State.AdvancedSettings.TryGetValue(def.PrefabID, out buildingEntry);

            if (foundBuilding)
            {
                Debug.Log("Applying settings for: " + def.PrefabID);
                foreach (KeyValuePair<string, Dictionary<string, object>> componentEntry in buildingEntry)
                {
                    if (componentEntry.Key.Length < 4) continue;

                    Type componentType;
                    UnityEngine.Object component;

                    if (componentEntry.Key == "BASE")
                    { //edit BuildingDef instead
                        componentType = def.GetType();
                        component = def;
                    }
                    else if (componentEntry.Key.StartsWith("ADD:"))
                    { //addicomponent
                        componentType = Type.GetType(componentEntry.Key.Substring(4) + ", Assembly-CSharp", false);
                        if (componentType == null)
                        {
                            PostBootDialog.ErrorList.Add(def.PrefabID + ": component type does not exist: " + componentEntry.Key);
                            continue;
                        }

                        component = def.BuildingComplete.AddComponent(componentType);
                        if (component == null)
                        {
                            PostBootDialog.ErrorList.Add(def.PrefabID + ": could not add component: " + componentEntry.Key);
                            continue;
                        }
                    }
                    else if (componentEntry.Key.StartsWith("DEL:"))
                    { //delete component
                        componentType = Type.GetType(componentEntry.Key.Substring(4) + ", Assembly-CSharp", false);
                        if (componentType == null)
                        {
                            PostBootDialog.ErrorList.Add(def.PrefabID + ": component type does not exist: " + componentEntry.Key);
                            continue;
                        }

                        component = def.BuildingComplete.GetComponent(componentType);
                        if (component == null)
                        {
                            PostBootDialog.ErrorList.Add(def.PrefabID + ": could not get component for deletion: " + componentEntry.Key);
                            continue;
                        }
                        UnityEngine.Object.DestroyImmediate(component);
                        continue;
                    }
                    else if (Regex.IsMatch(componentEntry.Key, "AT.:"))
                    { //edit component at index
                        int index;
                        if (!Int32.TryParse(componentEntry.Key.Substring(2, 2), out index))
                        {
                            PostBootDialog.ErrorList.Add(def.PrefabID + ": index could not be parsed: " + componentEntry.Key);
                            continue;
                        }

                        componentType = Type.GetType(componentEntry.Key + ", Assembly-CSharp", false);
                        if (componentType == null)
                        {
                            PostBootDialog.ErrorList.Add(def.PrefabID + ": component type does not exist: " + componentEntry.Key);
                            continue;
                        }

                        component = def.BuildingComplete.GetComponent(componentType);
                        if (component == null)
                        {
                            PostBootDialog.ErrorList.Add(def.PrefabID + ": does not have component: " + componentEntry.Key);
                            continue;
                        }
                    }
                    else
                    { //edit component
                        componentType = Type.GetType(componentEntry.Key + ", Assembly-CSharp", false);
                        if (componentType == null)
                        {
                            PostBootDialog.ErrorList.Add(def.PrefabID + ": component type does not exist: " + componentEntry.Key);
                            continue;
                        }

                        component = def.BuildingComplete.GetComponent(componentType);
                        if (component == null)
                        {
                            PostBootDialog.ErrorList.Add(def.PrefabID + ": does not have component: " + componentEntry.Key);
                            continue;
                        }
                    }

                    foreach (KeyValuePair<string, object> fieldEntry in componentEntry.Value)
                    {
                        object setValue = fieldEntry.Value;
                        Debug.Log("PROCESSING: value=" + setValue.ToString() + " type=" + setValue.GetType().ToString());


                        if (setValue.GetType() == typeof(double))
                        {
                            setValue = Convert.ToSingle((double)setValue);
                        }
                        else if (setValue.GetType() == typeof(string))
                        {
                            Debug.Log("It's a string.");
                            string strValue = (string)setValue; //convert string in enum if possible

                            int index = strValue.LastIndexOf(".");
                            if (index > 0 && strValue.Length > 2)
                            {
                                string enumName = strValue.Substring(0, index);
                                string enumConst = strValue.Substring(index + 1);
                                Type enumType = Type.GetType(enumName + ", Assembly-CSharp");
                                if (enumType != null)
                                {
                                    Debug.Log("It's an enum.");
                                    try
                                    {
                                        setValue = Enum.Parse(enumType, enumConst);
                                    }
                                    catch (Exception)
                                    {
                                        PostBootDialog.ErrorList.Add("Enum does not exist: " + enumConst);
                                        continue;
                                    }
                                }
                            }
                        }

                        try
                        {
                            //Type valueType = fieldEntry.Value.GetType();
                            AccessTools.Field(componentType, fieldEntry.Key).SetValue(component, setValue);
                        }
                        catch (Exception e)
                        {
                            PostBootDialog.ErrorList.Add(def.PrefabID + ", " + componentEntry.Key + ", " + fieldEntry.Key + " encountered an error: '" + e.Message + "' when trying to write: " + fieldEntry.Value.ToString());
                        }
                    }

                }
                CustomizeBuildingsState.StateManager.State.AdvancedSettings.Remove(def.PrefabID);
            }
        }
    }
}