using HarmonyLib;
using UnityEngine;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using Common;
using Config;
using System.Reflection;
using Shared;
using System.CodeDom;

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(BuildingConfigManager), nameof(BuildingConfigManager.RegisterBuilding))]
    public class BuildingConfigManager_RegisterBuilding
    {
        public static BuildingDef BuildingDefOverride(BuildingDef buildingDef)
        {
            //Debug.Log(buildingDef.BuildLocationRule.GetType().FullName);
            //Debug.Log(buildingDef.BuildingComplete.GetType().AssemblyQualifiedName);

            foreach (var mod in Mods.Get)
            {
                if (mod.Enabled(buildingDef.PrefabID))
                {
                    try
                    {
                        mod.EditDef(buildingDef);
                    } catch (Exception e) { Helpers.Print(e.ToString()); }
                }
            }

            if (!CustomizeBuildingsState.StateManager.State.BuildingBaseSettingGlobalFlag)
                return buildingDef;

            CustomizeBuildingsState.StateManager.State.BuildingBaseSettings.TryGetValue(buildingDef.PrefabID, out var entry);
            if (entry == null)
                CustomizeBuildingsState.StateManager.State.BuildingBaseSettings.TryGetValue(buildingDef.Name.StripLinks(), out entry);

            if (entry == null)
                return buildingDef;

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

            if (entry.ThermalConductivity != null)
            {
                buildingDef.ThermalConductivity = (float)entry.ThermalConductivity;
            }

            if (entry.MaterialCategory != null)
            {
                buildingDef.MaterialCategory = entry.MaterialCategory.Split(' ');
            }

            if (entry.ConstructionMass != null)
            {
                buildingDef.Mass = entry.ConstructionMass;
            }

            if (buildingDef.MaterialCategory.Length != buildingDef.Mass.Length)
            {
                float[] newmass = new float[buildingDef.MaterialCategory.Length];
                for (int i = 0; i < newmass.Length; i++)
                {
                    if (i < buildingDef.Mass.Length)
                        newmass[i] = buildingDef.Mass[i];
                    else
                        newmass[i] = 1f;
                }
                buildingDef.Mass = newmass;
            }

            if (entry.ObjectLayer != null)
            {
                buildingDef.ObjectLayer = entry.ObjectLayer.Value;
            }

            if (entry.TileLayer != null)
            {
                buildingDef.TileLayer = entry.TileLayer.Value;
            }

            if (entry.ReplacementLayer != null)
            {
                buildingDef.ReplacementLayer = entry.ReplacementLayer.Value;
            }

            if (entry.SceneLayer != null)
            {
                buildingDef.SceneLayer = entry.SceneLayer.Value;
            }

            // public bool Cancellable = true;
            // public bool OnePerWorld = false;

            #endregion

            return buildingDef;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> code, ILGenerator generator, MethodBase original)
        {
            var tool = new TranspilerTool(code, generator, original);

            tool.Seek(typeof(IBuildingConfig), nameof(IBuildingConfig.CreateBuildingDef));
            tool.InsertAfter(BuildingDefOverride);

            return tool;
        }
    }

    [HarmonyPatch(typeof(Assets), nameof(Assets.AddBuildingDef))]
    public class Assets_AddBuildingDef
    {
        public static void Prefix(BuildingDef def)
        {
            Helpers.Print($"Loading {def.PrefabID}, {def.Name.StripLinks()}");

            foreach (var mod in Mods.Get)
            {
                if (mod.Enabled(def.PrefabID))
                {
                    try
                    {
                        mod.EditGO(def); // TODO: update all capacity patches to new IBuildingCompleteMod
                    } catch (Exception e) { Helpers.Print(e.ToString()); }
                }
            }

            if (def.PrefabID == RefrigeratorConfig.ID
                && CustomizeBuildingsState.StateManager.State.BuildingBaseSettings.TryGetValue(RefrigeratorConfig.ID, out var baseSetting))
            {
                if (baseSetting.PowerConsumption.HasValue)
                    def.BuildingComplete.GetDef<RefrigeratorController.Def>().powerSaverEnergyUsage = baseSetting.PowerConsumption.Value / 6f;
            }

            if (!CustomizeBuildingsState.StateManager.State.BuildingAdvancedGlobalFlag)
                return;

            MachineMuliplier(def);
            OutputTemp(def);
            SuperAdvanced(def);
        }

        public static void MachineMuliplier(BuildingDef def)
        {
            float multiplier;
            bool flag = CustomizeBuildingsState.StateManager.State.BuildingAdvancedMachineMultiplier.TryGetValue(def.PrefabID, out multiplier);
            if (!flag)
                flag = CustomizeBuildingsState.StateManager.State.BuildingAdvancedMachineMultiplier.TryGetValue(def.Name.StripLinks(), out multiplier);

            if (flag)
            {
                ElementConverter[] converters = def.BuildingComplete.GetComponents<ElementConverter>();
                for (int j = 0; j < converters.Length; j++)
                {
                    for (int i = 0; i < converters[j].consumedElements.Length; i++)
                        converters[j].consumedElements[i].MassConsumptionRate *= multiplier;
                    for (int i = 0; i < converters[j].outputElements.Length; i++)
                        converters[j].outputElements[i].massGenerationRate *= multiplier;
                    Debug.Log("Multiplier: " + def.PrefabID + " x" + multiplier);
                }

                Storage[] storages = def.BuildingComplete.GetComponents<Storage>();
                for (int i = 0; i < storages.Length; i++)
                {
                    storages[i].capacityKg *= multiplier;
                }

                ManualDeliveryKG[] manualDeliveryKGs = def.BuildingComplete.GetComponents<ManualDeliveryKG>();
                for (int i = 0; i < manualDeliveryKGs.Length; i++)
                {
                    manualDeliveryKGs[i].capacity *= multiplier;
                }

                ConduitConsumer[] conduitConsumer = def.BuildingComplete.GetComponents<ConduitConsumer>();
                for (int i = 0; i < conduitConsumer.Length; i++)
                {
                    conduitConsumer[i].capacityKG *= multiplier;
                    conduitConsumer[i].consumptionRate *= multiplier;
                }

                PassiveElementConsumer[] elementConsumer = def.BuildingComplete.GetComponents<PassiveElementConsumer>();
                for (int i = 0; i < elementConsumer.Length; i++)
                {
                    elementConsumer[i].consumptionRate *= multiplier;
                    elementConsumer[i].capacityKG *= multiplier;
                }

                BuildingElementEmitter[] buildingElementEmitter = def.BuildingComplete.GetComponents<BuildingElementEmitter>();
                for (int i = 0; i < buildingElementEmitter.Length; i++)
                {
                    buildingElementEmitter[i].emitRate *= multiplier;
                }

                AlgaeDistillery[] algaeDistillery = def.BuildingComplete.GetComponents<AlgaeDistillery>();
                for (int i = 0; i < algaeDistillery.Length; i++)
                {
                    algaeDistillery[i].emitMass *= multiplier;
                }

                ElementDropper[] elementDropper = def.BuildingComplete.GetComponents<ElementDropper>();
                for (int i = 0; i < elementDropper.Length; i++)
                {
                    elementDropper[i].emitMass *= multiplier;
                }

            }
        }

        public static void OutputTemp(BuildingDef def)
        {
            CustomizeBuildingsState.StateManager.State.BuildingAdvancedOutputTemp.TryGetValue(def.PrefabID, out var setting);
            if (setting == null)
                CustomizeBuildingsState.StateManager.State.BuildingAdvancedOutputTemp.TryGetValue(def.Name.StripLinks(), out setting);

            if (setting == null)
                return;

            foreach (var converter in def.BuildingComplete.GetComponents<ElementConverter>())
            {
                setting.Set(converter);
            }

            foreach (var converter in def.BuildingComplete.GetComponents<EnergyGenerator>())
            {
                setting.Set(converter);
            }
        }

        public static void SuperAdvanced(BuildingDef def)
        {
            if (CustomizeBuildingsState.StateManager.State.AdvancedSettings == null) return;

            CustomizeBuildingsState.StateManager.State.AdvancedSettings.TryGetValue(def.PrefabID, out var buildingEntry);
            if (buildingEntry == null)
                CustomizeBuildingsState.StateManager.State.AdvancedSettings.TryGetValue(def.Name.StripLinks(), out buildingEntry);

            if (buildingEntry == null)
                return;

            Debug.Log("Applying settings for: " + def.PrefabID);
            foreach (var componentEntry in buildingEntry)
            {
                if (componentEntry.Key.Length < 4) continue;

                Type componentType = null;
                object component = null;

                if (componentEntry.Key == "BASE") //edit BuildingDef instead
                {
                    componentType = def.GetType();
                    component = def;
                }
                else if (componentEntry.Key.StartsWith("ADD:")) //addicomponent
                {
                    componentType = Type.GetType(componentEntry.Key.Substring(4) + ", Assembly-CSharp", false);
                    if (componentType != null)
                        component = def.BuildingComplete.AddComponent(componentType);
                }
                else if (componentEntry.Key.StartsWith("DEL:")) //delete component
                {
                    componentType = Type.GetType(componentEntry.Key.Substring(4) + ", Assembly-CSharp", false);
                    if (componentType != null)
                        component = def.BuildingComplete.GetComponent(componentType);
                    UnityEngine.Object.DestroyImmediate(component as UnityEngine.Object);
                    continue;
                }
                else if (Regex.IsMatch(componentEntry.Key, @"AT\d:")) //edit component at index
                {
                    componentType = Type.GetType(componentEntry.Key.Substring(4) + ", Assembly-CSharp", false);
                    if (componentType != null)
                        component = def.BuildingComplete.GetComponents(componentType)[int.Parse(componentEntry.Key.Substring(2, 1))];
                }
                else if (componentEntry.Key.EndsWith(".Def"))
                {
                    componentType = Type.GetType(componentEntry.Key.Replace('.', '+') + ", Assembly-CSharp", false);
                    component = def.BuildingComplete.GetComponent<StateMachineController>()?.cmpdef.defs.FirstOrDefault(f => f.GetType() == componentType);
                }
                else //edit component
                {
                    componentType = Type.GetType(componentEntry.Key + ", Assembly-CSharp", false);
                    if (componentType != null)
                        component = def.BuildingComplete.GetComponent(componentType);
                }

                // null checks
                if (componentType == null)
                {
                    PostBootDialog.ErrorList.Add(def.PrefabID + ": component type does not exist: " + componentEntry.Key);
                    continue;
                }
                if (component == null)
                {
                    PostBootDialog.ErrorList.Add(def.PrefabID + ": does not have component: " + componentEntry.Key);
                    continue;
                }

                foreach (KeyValuePair<string, object> fieldEntry in componentEntry.Value)
                {
                    FieldInfo fieldInfo;
                    object setValue = fieldEntry.Value;
                    Debug.Log($"PROCESSING: value={setValue} type={setValue.GetType()}");

                    try
                    {
                        fieldInfo = componentType.GetField(fieldEntry.Key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        if (fieldInfo.FieldType == typeof(float))
                        {
                            setValue = Convert.ToSingle(setValue);
                        }
                        else if (fieldInfo.FieldType.IsEnum)
                        {
                            string strValue = setValue.ToString();
                            int index = strValue.LastIndexOf(".");
                            //string enumName = strValue.Substring(0, index);
                            string enumConst = strValue.Substring(index + 1);
                            setValue = Enum.Parse(fieldInfo.FieldType, enumConst);
                        }

                        fieldInfo.SetValue(component, setValue);
                    } catch (Exception e)
                    {
                        PostBootDialog.ErrorList.Add($"{def.PrefabID}, {componentEntry.Key}, {fieldEntry.Key} encountered an error: '{e.Message}' when trying to write: {fieldEntry.Value}");
                    }
                }

            }
            CustomizeBuildingsState.StateManager.State.AdvancedSettings.Remove(def.PrefabID);
        }
    }
}
