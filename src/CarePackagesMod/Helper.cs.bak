using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CarePackageMod
{
    public class Helper
    {
        public static void AddButton(GameObject go, string title, string tooltip, System.Action onPress, float order)
        {
            Game.Instance.userMenu.AddButton(go, new KIconButtonMenu.ButtonInfo(text: title,
                                                                                on_click: new System.Action(onPress),
                                                                                shortcutKey: (Action)Enum.Parse(typeof(Action), "NumActions"),  // DLC and vanilla have different enums
                                                                                tooltipText: tooltip),
                                                                                sort_order: order);
        }
    }
}
