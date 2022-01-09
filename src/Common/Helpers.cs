#define LOCALE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HarmonyLib;
using System.Diagnostics;
using UnityEngine;
using System.Reflection;
using Config;
using System.IO;
using Klei.AI;

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

        public static SimHashes ToSimHash(this Tag tag)
        {
            return (SimHashes)tag.GetHash();
        }

        public static Tag ToTag(this SimHashes hash)
        {
            return new Tag(hash.ToString());
        }

        public static Element ToElement(this Tag tag)
        {
            if (ElementLoader.elementTable == null)
                return null;

            ElementLoader.elementTable.TryGetValue(tag.GetHash(), out Element element);
            return element;
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

        #region UI
        public static void AddButton(GameObject go, string title, string tooltip, System.Action onPress, float order)
        {
            Game.Instance.userMenu.AddButton(go, new KIconButtonMenu.ButtonInfo(text: title,
                                                                                on_click: new System.Action(onPress),
                                                                                shortcutKey: (Action)Enum.Parse(typeof(Action), "NumActions"),  // DLC and vanilla have different enums
                                                                                tooltipText: tooltip),
                                                                                sort_order: order);
        }
        #endregion

        #region Utility
        public static bool IsModActive(string title, bool exact = false)
        {
            if (exact)
                return Global.Instance.modManager.mods.FirstOrDefault(s => s.staticID == title)?.IsEnabledForActiveDlc() ?? false;

            return Global.Instance.modManager.mods.FirstOrDefault(s => s.staticID.EqualIgnoreCase(title) || s.title.EqualIgnoreCase(title))?.IsEnabledForActiveDlc() ?? false;
        }

        public static bool EqualIgnoreCase(this string str1, string str2)
        {
            return String.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
        }

        public static HashSet<Tag> RemoveRange(this HashSet<Tag> set, IEnumerable<Tag> itemsToRemove)
        {
            var result = new HashSet<Tag>();

            foreach (Tag item in set)
                if (!itemsToRemove.Contains(item))
                    result.Add(item);

            return result;
        }

        public static T[] AppendAndReplace<T>(ref T[] orig, params T[] objs)
        {
            if (orig == null) orig = new T[0];

            int i, j;
            T[] result = new T[orig.Length + objs.Length];
            for (i = 0; i < orig.Length; i++)
                result[i] = orig[i];
            for (j = 0; i < result.Length; i++)
                result[i] = objs[j++];
            orig = result;
            return result;
        }

        public static T[] AddToArray<T>(this T[] array, params T[] objs)
        {
            if (array == null) array = new T[0];

            int i, j;
            T[] result = new T[array.Length + objs.Length];
            for (i = 0; i < array.Length; i++)
                result[i] = array[i];
            for (j = 0; i < result.Length; i++)
                result[i] = objs[j++];
            return result;
        }

        public static T[] RemoveFromArray<T>(this T[] array, params T[] objs)
        {
            int count = 0;
            for (int i = 0; i < array.Length; i++)
                if (objs.Contains(array[i]))
                    count++;

            T[] result = new T[array.Length - count];
            for (int i = 0; i < array.Length; i++)
                if (!objs.Contains(array[i]))
                    result[i] = array[i];
            return result;
        }

        public static T[] RemoveFromArray<T>(this IEnumerable<T> array, params T[] objs)
        {
            int count = array.Count();

            foreach (var entry in array)
                if (objs.Contains(entry))
                    count--;

            T[] result = new T[count];
            int j = 0;
            foreach (var entry in array)
                if (!objs.Contains(entry))
                    result[j++] = entry;
            return result;
        }

        /// <summary>Returns string inside quotation marks. Respects escape character.</summary>
        public static string GetQuotationString(this string line, int occurrence, char delimiter = '"')
        {
            if (line == null) return null;
            occurrence--;

            bool isEscape = false;
            int start = -1;
            int stop = -1;
            int count = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (isEscape)
                {
                    isEscape = false;
                }
                else if (line[i] == '\\')
                {
                    isEscape = true;
                    continue;
                }
                else if (line[i] == delimiter)
                {
                    if (occurrence == count / 2)
                    {
                        if (0 == count % 2)
                        {
                            start = i + 1;
                        }
                        else
                        {
                            stop = i;
                            break;
                        }
                    }
                    // o = 0; c = 0 -> start
                    // o = 0; c = 1 -> stop
                    // o = 1; c = 2 -> start
                    // o = 1; c = 3 -> stop
                    // o = 2; c = 4 -> start
                    // o = 2; c = 5 -> stop
                    count++;
                }
            }

            if (start >= 0 && stop >= start)
                return line.Substring(start, stop - start); //"str"; start=1, stop=4
            return null;
        }

        public static string GetQuotationString(this string line)
        {
            if (line == null) return null;
            int index1 = line.IndexOf('"') + 1;
            int index2 = line.LastIndexOf('"');
            if (index1 > 0 && index2 >= index1)
                return line.Substring(index1, index2 - index1);
            return null;
        }

        public static string GetUndoLiteralString(this string line)
        {
            if (line == null) return null;
            line = line.Replace("\\\\", "\\");
            line = line.Replace("\\\"", "\"");
            line = line.Replace("\\t", "\t");
            line = line.Replace("\\n", "\n");
            return line;
        }

        public static string GetLiteralString(this string line)
        {
            if (line == null) return null;
            line = line.Replace("\\", "\\\\");
            line = line.Replace("\"", "\\\"");
            line = line.Replace("\t", "\\t");
            line = line.Replace("\n", "\\n");
            return line;
        }
        #endregion

        #region Locale
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

        public static string PathLocale
        {
            get
            {
                string code = Localization.GetCurrentLanguageCode();
                if (code == null || code == "" || code.Length < 2)
                    code = "en";
                code = code.Substring(0, 2);
                return Path.Combine(Config.PathHelper.AssemblyDirectory, "strings_" + code + ".pot");
            }
        }
#if LOCALE
        public static Dictionary<string, string> StringsDic = new Dictionary<string, string>();

        /// Use this to create a translation file. Only works, if you have used StringsAdd or StringsTag to add to the list.
        /// Can be called multiple times. Consecutive calls will appended. Must #define LOCALE to be used.
        public static bool StringsAppend = false;
        public static void StringsPrint(string path = null)
        {
            using (StreamWriter sw = new StreamWriter(path ?? Path.Combine(Config.PathHelper.AssemblyDirectory, "strings_en.pot"), StringsAppend))
            {
                foreach (var keyPair in StringsDic)
                {
                    sw.WriteLine($"#. {keyPair.Key}");
                    sw.WriteLine($"msgctxt \"{keyPair.Key}\"");
                    sw.WriteLine($"msgid \"{keyPair.Value.GetLiteralString()}\"");
                    sw.WriteLine($"msgstr \"\"\n");
                }
            }

            StringsAppend = true;
            StringsDic.Clear();
        }
#endif

        /// Adds string to 'Strings' table.
        public static void StringsAdd(string key, string text)
        {
#if LOCALE
            StringsDic[key] = text;
#endif
            Strings.Add(key, text);
        }

        [System.Diagnostics.Conditional("LOCALE")]
        public static void StringsAddProperty(string key, string text)
        {
            StringsAdd(key, text);
        }

        [System.Diagnostics.Conditional("LOCALE")]
        public static void StringsAddClass(Type @class)
        {
            foreach (var field in @class.GetFields())
            {
                StringsAdd($"{ModName}.PROPERTY.{field.Name}", field.Name);
            }
        }

        /// Adds string to TagManager.
        public static void StringsTag(string key, string id, string proper = null)
        {
#if LOCALE
            key = key.Replace(' ', '_');
            StringsDic[key] = id;
#endif
            TagManager.Create(id, proper ?? id);
        }

        public static void StringsLoad(string path = null)
        {
            try
            {
                if (path == null)
                    path = Helpers.PathLocale;

                if (!File.Exists(path))
                {
                    Print("Language file does not exist: " + path);
                    return;
                }

                Print("Read language file: " + path);

                foreach (var pair in Localization.LoadStringsFile(path, false))
                    Strings.Add(pair.Key, pair.Value);

                //string key = null;
                //string id = null;
                //bool isTag = false;
                //var lines = File.ReadAllLines(path);
                //int i = 0;
                //for (; i < lines.Length; i++) if (lines[i].StartsWith("msgctxt", StringComparison.Ordinal)) break; // ignore everything until the first msgctxt
                //for (; i < lines.Length; i++)
                //{
                //    string line = lines[i];
                //    string quote = line.GetQuotationString();
                //    int j = 0;
                //    // resolve multi-line quotes
                //    while (true)
                //    {
                //        if (quote == null)
                //            break;
                //        if (lines.Length <= i + 1)
                //            break;
                //        string quote2 = lines[i + 1];
                //        if (!quote2.StartsWith("\"", StringComparison.Ordinal))
                //            break;
                //        quote2 = quote2.GetQuotationString();
                //        if (quote2 == null)
                //            break;
                //        quote += quote2;
                //        i++;
                //        j++;
                //    }
                //    if (line.StartsWith("msgctxt", StringComparison.Ordinal))
                //    {
                //        isTag = line.Contains(".TAG.");
                //        key = quote;
                //        continue;
                //    }
                //    if (line.StartsWith("msgid", StringComparison.Ordinal))
                //    {
                //        id = quote;
                //        continue;
                //    }
                //    if (line.StartsWith("msgstr", StringComparison.Ordinal))
                //    {
                //        if (quote == null)
                //        {
                //            Print($"Error: quote is null at i={i} j={j}");
                //            continue;
                //        }
                //        if (!isTag && (key == null || key == ""))
                //        {
                //            Print($"Error: key is null at i={i} j={j} for '{quote}'");
                //            continue;
                //        }
                //        if (isTag && (id == null || id == ""))
                //        {
                //            Print($"Error: tag id is null at i={i} j={j} for '{quote}'");
                //            continue;
                //        }
                //        if (!isTag)
                //            Strings.Add(key, quote.GetUndoLiteralString());
                //        else
                //            TagManager.Create(id, quote.GetUndoLiteralString());
                //        key = null;
                //        id = null;
                //        continue;
                //    }
                //}
            }
            catch (System.Exception e)
            {
                Print("Error reading language file: " + e.ToString());
            }
        }


        public static void LocalizeTypeToPOT(Type type, string path = null)
        {
            string typename = type.FullName.Replace('+', '.');
            using (StreamWriter sw = new StreamWriter(path ?? "STRINGS.pot", true))
            {
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    if (field.FieldType == typeof(LocString))
                    {
                        var loc = (LocString)field.GetValue(null);
                        sw.WriteLine($"#. {typename}.{field.Name}");
                        sw.WriteLine($"msgctxt \"{typename}.{field.Name}\"");
                        sw.WriteLine($"msgid \"{loc.text}\"");
                        sw.WriteLine($"msgstr \"\"\n");
                    }
                }
            }
        }
        #endregion

        #region AttributeModifier
        public static PropertyInfo _AttributeModifierValue = AccessTools.Property(typeof(AttributeModifier), nameof(AttributeModifier.Value));
        public static PropertyInfo _AttributeModifierIsMultiplier = AccessTools.Property(typeof(AttributeModifier), nameof(AttributeModifier.IsMultiplier));

        public static void EnsureAttributeModifier(this List<AttributeModifier> list, string AttributeId, float value, bool is_multiplier, string description = null, bool uiOnly = false, bool is_readonly = false)
        {
            var attribute = list.Find(s => s.AttributeId == AttributeId);
            if (attribute == null)
            {
                list.Add(new AttributeModifier(AttributeId, value, description, is_multiplier, uiOnly, is_readonly));
            }
            else
            {
                _AttributeModifierValue.SetValue(attribute, value, null);
                _AttributeModifierIsMultiplier.SetValue(attribute, is_multiplier, null);
            }
        }

        public class AttributeContainer
        {
            public string Description;
            public string AttributeId;
            public float Value;
            public bool IsMultiplier;

            public AttributeContainer()
            { }

            public AttributeContainer(AttributeModifier source)
            {
                this.Description = source.Description;
                this.AttributeId = source.AttributeId;
                this.Value = source.Value;
                this.IsMultiplier = source.IsMultiplier;
            }

            public static implicit operator AttributeModifier(AttributeContainer container)
            {
                return new AttributeModifier(container.AttributeId, container.Value, container.Description, container.IsMultiplier, uiOnly: false, is_readonly: false);
            }
        }
        #endregion

        #region Components
        public static void PrintAllPatches(Type type, string method)
        {
            try
            {
                var harmony = new Harmony("com.company.project.product");

                var original = type.GetMethod(method);
                var info = Harmony.GetPatchInfo(original);

                if (info == null) return;

                foreach (var patch in info.Prefixes)
                {
                    Debug.Log("Prefix index: " + patch.index);
                    Debug.Log("Prefix owner: " + patch.owner);
                    Debug.Log("Prefix patch: " + patch.PatchMethod.Name);
                    Debug.Log("Prefix priority: " + patch.priority);
                    Debug.Log("Prefix before: " + String.Join(" ", patch.before.ToArray()));
                    Debug.Log("Prefix after: " + String.Join(" ", patch.after.ToArray()));
                }
                foreach (var patch in info.Postfixes)
                {
                    Debug.Log("Postfix index: " + patch.index);
                    Debug.Log("Postfix owner: " + patch.owner);
                    Debug.Log("Postfix patch: " + patch.PatchMethod.Name);
                    Debug.Log("Postfix priority: " + patch.priority);
                    Debug.Log("Postfix before: " + String.Join(" ", patch.before.ToArray()));
                    Debug.Log("Postfix after: " + String.Join(" ", patch.after.ToArray()));
                }
                foreach (var patch in info.Transpilers)
                {
                    Debug.Log("Transpiler index: " + patch.index);
                    Debug.Log("Transpiler owner: " + patch.owner);
                    Debug.Log("Transpiler patch: " + patch.PatchMethod.Name);
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

    }
}