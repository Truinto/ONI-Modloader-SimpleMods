using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Harmony;
using System.Diagnostics;
using UnityEngine;
using System.Reflection;
using Config;

namespace Common
{
    public static class Helpers
    {
        #region Log
        public static string ModName;

        /// <summary>Prints text to the log.</summary>
        public static void Print(string text)
        {
            Console.Write(System.DateTime.UtcNow.ToString("[HH:mm:ss.fff] [") + Thread.CurrentThread.ManagedThreadId + "] [INFO] [" + ModName + "] ");
            Console.WriteLine(text);
        }

        /// <summary>Prints text only if run in DEBUG.</summary>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void PrintDebug(string text)
        {
            Console.Write(System.DateTime.UtcNow.ToString("[HH:mm:ss.fff] [") + Thread.CurrentThread.ManagedThreadId + "] [DEBUG] [" + ModName + "] ");
            Console.WriteLine(text);
        }

        /// <summary>Prints text to the log and also prompts a message box after the main menu loaded.</summary>
        public static void PrintDialog(string text)
        {
            Console.Write(System.DateTime.UtcNow.ToString("[HH:mm:ss.fff] [") + Thread.CurrentThread.ManagedThreadId + "] [DIALOG] [" + ModName + "] ");
            Console.WriteLine(text);
            PostBootDialog.ErrorList.Add(text);
        }
        #endregion

        #region Math
        /// <summary>Clamps the value between Min and Max.</summary>
        /// <param name="Min">the minimum the value must be</param>
        /// <param name="Value">value to be clamped</param>
        /// <param name="Max">the maximum the value must be</param>
        /// <returns></returns>
        public static int MinMax(int Min, int Value, int Max)
        {
            return Math.Max(Math.Min(Max, Value), Min);
        }

        /// <summary>Clamps the value between Min and Max.</summary>
        /// <param name="Min">the minimum the value must be</param>
        /// <param name="Value">value to be clamped</param>
        /// <param name="Max">the maximum the value must be</param>
        /// <returns></returns>
        public static float MinMax(float Min, float Value, float Max)
        {
            return Math.Max(Math.Min(Max, Value), Min);
        }
        #endregion

        #region Conversion
        public static SimHashes ToSimHash(this string str, SimHashes fallback = SimHashes.Vacuum)
        {
            SimHashes result = (SimHashes)Hash.SDBMLower(str);

            if (ElementLoader.GetElementIndex(result) < 0)
                return fallback;

            return result;
        }

        public static string ToDiseaseId(this byte diseaseIdx)
        {
            if (diseaseIdx < Db.Get().Diseases.Count)
                return Db.Get().Diseases[diseaseIdx].Id;
            else
                return null;
        }

        public static byte ToDiseaseIdx(this string diseaseId)
        {
            return Db.Get().Diseases.GetIndex(diseaseId);
        }
        #endregion

        public static void AddButton(GameObject go, string title, string tooltip, System.Action onPress, float order)
        {
            Game.Instance.userMenu.AddButton(go, new KIconButtonMenu.ButtonInfo(text: title,
                                                                                on_click: new System.Action(onPress),
                                                                                shortcutKey: (Action)Enum.Parse(typeof(Action), "NumActions"),  // DLC and vanilla have different enums
                                                                                tooltipText: tooltip),
                                                                                sort_order: order);
        }

        public static bool IsModActive(string title)
        {
            return Global.Instance.modManager.mods.FirstOrDefault(s => s.title == title)?.IsEnabledForActiveDlc() ?? false;
        }

        public static HashSet<Tag> RemoveRange(this HashSet<Tag> set, IEnumerable<Tag> itemsToRemove)
        {
            var result = new HashSet<Tag>();

            foreach (Tag item in set)
                if (!itemsToRemove.Contains(item))
                    result.Add(item);

            return result;
        }

        #region Strings
        public static LocString GetTemperatureUnit()
        {
            switch (GameUtil.temperatureUnit)
            {
                case GameUtil.TemperatureUnit.Celsius:
                    return STRINGS.UI.UNITSUFFIXES.TEMPERATURE.CELSIUS;
                case GameUtil.TemperatureUnit.Fahrenheit:
                    return STRINGS.UI.UNITSUFFIXES.TEMPERATURE.FAHRENHEIT;
                default:
                    return STRINGS.UI.UNITSUFFIXES.TEMPERATURE.KELVIN;
            }
        }
        #endregion

        #region Components
        public static void PrintAllPatches(Type type, string method)
        {
            try
            {
                var harmony = HarmonyInstance.Create("com.company.project.product");

                var original = type.GetMethod(method);
                var info = harmony.GetPatchInfo(original);

                if (info == null) return;

                foreach (var patch in info.Prefixes)
                {
                    Debug.Log("Prefix index: " + patch.index);
                    Debug.Log("Prefix owner: " + patch.owner);
                    Debug.Log("Prefix patch: " + patch.patch.Name);
                    Debug.Log("Prefix priority: " + patch.priority);
                    Debug.Log("Prefix before: " + String.Join(" ", patch.before.ToArray()));
                    Debug.Log("Prefix after: " + String.Join(" ", patch.after.ToArray()));
                }
                foreach (var patch in info.Postfixes)
                {
                    Debug.Log("Postfix index: " + patch.index);
                    Debug.Log("Postfix owner: " + patch.owner);
                    Debug.Log("Postfix patch: " + patch.patch.Name);
                    Debug.Log("Postfix priority: " + patch.priority);
                    Debug.Log("Postfix before: " + String.Join(" ", patch.before.ToArray()));
                    Debug.Log("Postfix after: " + String.Join(" ", patch.after.ToArray()));
                }
                foreach (var patch in info.Transpilers)
                {
                    Debug.Log("Transpiler index: " + patch.index);
                    Debug.Log("Transpiler owner: " + patch.owner);
                    Debug.Log("Transpiler patch: " + patch.patch.Name);
                    Debug.Log("Transpiler priority: " + patch.priority);
                    Debug.Log("Transpiler before: " + String.Join(" ", patch.before.ToArray()));
                    Debug.Log("Transpiler after: " + String.Join(" ", patch.after.ToArray()));
                }
                // all owners shortcut
                Debug.Log("All owners: " + String.Join(" ", info.Owners.ToArray()));
            }
            catch (Exception) { }
        }

        public static void RemoveComponent(GameObject go)
        {
            UnityEngine.Object.DestroyImmediate(go);
        }

        public static void RemoveComponent<UComponent>(this GameObject go) where UComponent : UnityEngine.Object
        {
            UComponent comp = go.GetComponent<UComponent>();
            if (comp != null)
                UnityEngine.Object.DestroyImmediate(comp);
        }

        public static void RemoveComponents<UComponent>(this GameObject go) where UComponent : UnityEngine.Object
        {
            UComponent[] comps = go.GetComponents<UComponent>();

            foreach (var comp in comps)
                UnityEngine.Object.DestroyImmediate(comp);
        }

        public static void RemoveDef(GameObject go, StateMachine.BaseDef def)
        {
            StateMachineController controller = go.GetComponent<StateMachineController>();
            if (controller != null)
            {
                if (def != null)
                    controller.cmpdef.defs.Remove(def);
            }
        }

        public static void RemoveDef<Def>(this GameObject go) where Def : StateMachine.BaseDef
        {
            StateMachineController controller = go.GetComponent<StateMachineController>();
            if (controller != null)
            {
                var def = go.GetDef<Def>();
                if (def != null)
                    controller.cmpdef.defs.Remove(def);
            }
        }
        #endregion

        #region Special
        public static void ProcessTags(this KPrefabID prefab, string[] tags, bool serialize = false)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                if (tags[i] == "C:")
                    prefab.Tags.Clear();
                else if (tags[i].StartsWith("R:", StringComparison.Ordinal))
                    prefab.RemoveTag(tags[i].Substring(2));
                else
                    prefab.AddTag(tags[i], serialize);
            }
        }

        public static FertilityMonitor.BreedingChance BreedingChance(string tag, float weight)
        {
            return new FertilityMonitor.BreedingChance()
            {
                egg = tag.ToTag(),
                weight = weight
            };
        }
        #endregion

        #region FastAccess
        public static FastGetter CreateGetter(this Type type, string name)
        {
            return new FastGetter(FastAccess.CreateGetterHandler(AccessTools.Field(type, name)));
        }
        public static FastSetter CreateSetter(this Type type, string name)
        {
            return new FastSetter(FastAccess.CreateSetterHandler(AccessTools.Field(type, name)));
        }
        public static FastGetter CreateGetterProperty(this Type type, string name)
        {
            return new FastGetter(FastAccess.CreateGetterHandler(AccessTools.Property(type, name)));
        }
        public static FastSetter CreateSetterProperty(this Type type, string name)
        {
            return new FastSetter(FastAccess.CreateSetterHandler(AccessTools.Property(type, name)));
        }
        public static FastInvoke CreateInvoker(this Type type, string methodName, Type[] args = null, Type[] typeArgs = null)
        {
            if (args == null && typeArgs == null)
                return new FastInvoke(MethodInvoker.GetHandler(AccessTools.Method(type, methodName)));
            return new FastInvoke(MethodInvoker.GetHandler(AccessTools.Method(type, methodName, args, typeArgs)));
        }
        #endregion
    }

    public delegate void FastSetter(object source, object value);
    public delegate object FastGetter(object source);
    public delegate object FastInvoke(object target, params object[] paramters);
}