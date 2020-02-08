using System;
using System.Reflection;

public static class CustomEventExtension
{

    public static void ClearEvents(this object obj, string eventName)
    {
        var fi = obj.GetType().GetEventField(eventName);
        if (fi == null) return;
        fi.SetValue(obj, null);
    }

    private static FieldInfo GetEventField(this Type type, string eventName)
    {
        FieldInfo field = null;
        while (type != null)
        {
            /* Find events defined as field */
            field = type.GetField(eventName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field != null && (field.FieldType == typeof(MulticastDelegate) || field.FieldType.IsSubclassOf(typeof(MulticastDelegate))))
                break;

            /* Find events defined as property { add; remove; } */
            field = type.GetField("EVENT_" + eventName.ToUpper(), BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field != null)
                break;
            type = type.BaseType;
        }
        return field;
    }
}