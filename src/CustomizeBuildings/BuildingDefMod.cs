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

namespace CustomizeBuildings
{
    [HarmonyPatch(typeof(BuildingConfigManager), "RegisterBuilding")]
    public class BuildingConfigManager_RegisterBuilding
    {
        public static void CreateBuildingDefOverride(BuildingDef buildingDef)
        {
            //Debug.Log(buildingDef.BuildLocationRule.GetType().FullName);
            //Debug.Log(buildingDef.BuildingComplete.GetType().AssemblyQualifiedName);

            CustomizeBuildingsState.StateManager.State.BuildingBaseSettings.TryGetValue(buildingDef.PrefabID, out var entry);
            if (entry == null)
                CustomizeBuildingsState.StateManager.State.BuildingBaseSettings.TryGetValue(buildingDef.Name.StripLinks(), out entry);

            if (entry == null)
                return;

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

            // public bool Cancellable = true;
            // public bool OnePerWorld = false;

            #endregion
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
            code.Insert(index++, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BuildingConfigManager_RegisterBuilding), nameof(CreateBuildingDefOverride))));

            return code;
        }
    }

    [HarmonyPatch(typeof(Assets), nameof(Assets.AddBuildingDef))]
    public class Assets_AddBuildingDef
    {
        public static bool Prepare()
        {
            if (mods == null)
                mods = Assembly.GetExecutingAssembly().GetTypes().Where(w => typeof(IBuildingCompleteMod).IsAssignableFrom(w) && w.IsClass).Select(s => Activator.CreateInstance(s) as IBuildingCompleteMod).ToList();

            Helpers.Print($"mods count={mods.Count}");
            return true;
        }

        private static List<IBuildingCompleteMod> mods;

        public static void Prefix(BuildingDef def)
        {
            Helpers.Print($"Loading {def.PrefabID}, {def.Name.StripLinks()}");

            foreach (var mod in mods)
            {
                if (mod.Enabled(def.PrefabID))
                    try
                    {
                        mod.Edit(def); // TODO: update all capacity patches to new IBuildingCompleteMod
                    }
                    catch (Exception e) { Helpers.Print(e.ToString()); }
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


        public static void NewAdvanced(string setting)
        {
            var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            //var AllTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes());

            // patch GeneratedBuildings.LoadGeneratedBuildings, very low priority
            // split with ':'
            // first value is always PrefabID
            // next values are considered fields
            // last value is split with ;
            // left side of '=' is the field
            // right right of '=' will be parsed and set
            // valid values are int, long, float, double, enum, string, and arrays/Lists of these values (split multi-values with ',')
            // example string: "IceCooledFan: Storage#1: capacityKg = 1000; Storage#2: capacityKg = 1000;storageFilters = Ice, DirtyIce, Dirt, Sand;"
            setting = Regex.Replace(setting, @"\s+", "");

            BuildingDef def = null;
            object target = null;
            int j = 0;                               // pointer back
            int numcom;                              // index of component to edit
            for (int i = 0; i < setting.Length - 1; i++) // pointer front
            {
                if (setting[i] == ':')
                {
                    if (def == null) // first read is always prefabID
                    {
                        string prefabID = read();
                        def = Assets.BuildingDefs.FirstOrDefault(f => f.PrefabID == prefabID); if (def == null)
                            throw new Exception("invalid PrefabID " + prefabID);
                        continue;
                    }

                    string component = read();
                    numcom = 0;
                    var cindex = component.IndexOf('#');
                    if (cindex >= 0)
                    {
                        string sub = component.Substring(cindex + 1);
                        int.TryParse(sub, out numcom);
                        component = component.Substring(0, cindex);
                    }

                    var targets = def.BuildingComplete.GetComponents<Component>().Where(w => w.GetType().Name == component).ToArray();
                    if (targets.Length <= numcom)
                        throw new Exception($"{def.PrefabID} has {targets.Length} of '{component}', but tried to access {numcom}");
                    target = targets[numcom];

                    continue;
                }

                if (setting[i] == ';')
                {
                    if (def == null)
                        throw new Exception("missing PrefabID");
                    if (target == null)
                        throw new Exception("missing target component");

                    var instruction = read();
                    int findex = instruction.IndexOf('=');
                    if (findex < 0)
                        throw new Exception("invalid format, did not find '='");
                    string newvalue = instruction.Substring(findex + 1);
                    string field = instruction.Substring(0, findex);

                    var fi = target.GetType().GetField(field, flags);
                    if (fi == null)
                        throw new Exception("invalid field name: " + field);
                    var value = fi.GetValue(target);
                    switch (value)
                    {
                        case Enum:
                            if (!Helpers.TryParseEnum(fi.FieldType, newvalue, out var venum))
                                throw new Exception("could not parse enum: " + newvalue);
                            fi.SetValue(target, venum);
                            Helpers.PrintDebug($"set enum {venum} to {def.PrefabID}:{fi.DeclaringType.Name}:{field}");
                            break;

                        case int:
                            if (!int.TryParse(newvalue, out int vint))
                                throw new Exception("could not parse int: " + newvalue);
                            fi.SetValue(target, vint);
                            Helpers.PrintDebug($"set int {vint} to {def.PrefabID}:{fi.DeclaringType.Name}:{field}");
                            break;

                        case float:
                            if (!int.TryParse(newvalue, out int vfloat))
                                throw new Exception("could not parse int: " + newvalue);
                            fi.SetValue(target, vfloat);
                            Helpers.PrintDebug($"set float {vfloat} to {def.PrefabID}:{fi.DeclaringType.Name}:{field}");
                            break;

                        case null:
                            throw new Exception("target has field, but could not access");
                        default:
                            throw new Exception("target field is not supported");
                    }


                    continue;
                }


                continue;
                string read() // get string between front and back pointer; advance pointer to ignore next character
                {
                    string result = setting.Substring(j, i - j);
                    j = i + 1;
                    return result;
                }
            }




            // -- old
            //var lines = setting.Split(':');
            //if (lines.Length < 2) throw new Exception("Missing separator ':'");

            //var def = Assets.BuildingDefs.FirstOrDefault(f => f.PrefabID == lines[0]);
            //if (def == null) throw new Exception("PrefabID doesn't exist");

            //object target = lines.Length == 2 ? def : def.BuildingComplete;
            //for (int i = 1; i < lines.Length - 1; i++)
            //{
            //    target.GetType().GetNestedTypes(flags); // todo
            //}

            //var values = lines[lines.Length - 1].Split(';').Where(w => w.Contains('='));
            //foreach (string x in values)
            //{

            //}
        }
    }
}