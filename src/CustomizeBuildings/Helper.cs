using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace CustomizeBuildings
{
    public static class Helper
    {
        /// <summary>Prints text only if run in DEBUG.</summary>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void PrintDebug(string text)
        {
            Debug.Log(text);
        }

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

        public static bool IsModActive(string title)
        {
            return Global.Instance.modManager.mods.FirstOrDefault(s => s.title == title)?.IsEnabledForActiveDlc() ?? false;
        }

        public static SimHashes ToSimHash(this string str, SimHashes fallback = SimHashes.Vacuum)
        {
            SimHashes result = (SimHashes)Hash.SDBMLower(str);

            if (ElementLoader.GetElementIndex(result) < 0)
                return fallback;

            return result;
        }

        public static HashSet<Tag> RemoveRange(this HashSet<Tag> set, IEnumerable<Tag> itemsToRemove)
        {
            var result = new HashSet<Tag>();

            foreach (Tag item in set)
                if (!itemsToRemove.Contains(item))
                    result.Add(item);

            return result;
        }

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
    }

    public delegate void FastSetter(object source, object value);
    public delegate object FastGetter(object source);
    public delegate object FastInvoke(object target, params object[] paramters);
}
