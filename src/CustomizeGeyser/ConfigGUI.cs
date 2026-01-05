using Common;
using HarmonyLib;
using System;
using UnityEngine;

namespace CustomizeGeyser.ConfigGUI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigClassAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigMemberAttribute(int group, string title) : Attribute
    {
        public int Group = group;
        public string Title = title;
        public string? Tooltip;
    }

    /// <summary>
    /// Display a modifable list.
    /// Fields must be public to be editable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigListAttribute : Attribute
    {
        public bool AddRemove = true;
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class FormatAttribute(string format) : Attribute
    {
        public string Format = format;
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class MinMaxAttribute(float min, float max) : Attribute
    {
        public float Min = min;
        public float Max = max;
    }

    [HarmonyPatch(typeof(ModsScreen), nameof(ModsScreen.BuildDisplay))]
    public static class GUIHookPatch
    {
        // see PLibOptions.AddModOptions
        public static void Postfix(ModsScreen __instance)
        {
            var mods = Global.Instance.modManager.mods;
            var parent = __instance.entryPrefab;

            foreach (var mod in __instance.displayedMods)
            {
                if (mod.mod_index < 0 || mod.mod_index >= mods.Count)
                    continue;
                if (mod.rect_transform == null)
                    continue;

                var kmod = mods[mod.mod_index];
                if (kmod.staticID != FumiKMod.ModName) // todo: dynamic id
                    continue;
                if (!kmod.IsEnabledForActiveDlc())
                    continue;

                // add new button at index 4
                Helpers.Print("Adding mod options button");
                var kButton = Util.KInstantiateUI<KButton>(MainMenu.Instance.buttonPrefab.gameObject, mod.rect_transform.gameObject, force_active: true);
                kButton.transform.SetSiblingIndex(4);
                var kimage = kButton.GetComponent<KImage>();
                kimage.colorStyleSetting = MainMenu.Instance.normalButtonStyle;
                kimage.ApplyColorStyleSetting();
                var text = kButton.GetComponentInChildren<LocText>();
                text.text = "Options";
                text.fontSize = 12;
                kButton.onClick += ConfigGUI.ShowGUI;
            }
        }

        //public static GameObject CreateGameObject(string name, GameObject? parent = null)
        //{
        //    var gameObject = new GameObject(name);
        //    if (parent != null)
        //        gameObject.transform.SetParent(parent?.transform, false);
        //    // stretch in both directions
        //    var transform = gameObject.AddOrGet<RectTransform>();
        //    transform.localScale = Vector3.one;
        //    transform.anchorMin = new(0f, 0f);
        //    transform.anchorMax = new(1f, 1f);
        //    transform.pivot = new(0.5f, 0.5f);
        //    transform.anchoredPosition = Vector2.zero;
        //    transform.offsetMax = Vector2.zero;
        //    transform.offsetMin = Vector2.zero;
        //    gameObject.AddOrGet<CanvasRenderer>();
        //    gameObject.layer = LayerMask.NameToLayer("UI");
        //    return gameObject;
        //}

        //public static GameObject Build(string name)
        //{
        //    var button = CreateGameObject(name, null);
        //    GameObject sprite = null, text = null;
        //    // Background
        //    var bgImage = button.AddComponent<KImage>();
        //    var bgColorStyle = Color ?? PUITuning.Colors.ButtonPinkStyle;
        //    UIDetours.COLOR_STYLE_SETTING.Set(bgImage, bgColorStyle);
        //    SetupButtonBackground(bgImage);
        //    // Set on click event
        //    var kButton = button.AddComponent<KButton>();
        //    var evt = OnClick;
        //    if (evt != null)
        //        // Detouring an Event is not worth the effort
        //        kButton.onClick += () => evt?.Invoke(button);
        //    SetupButton(kButton, bgImage);
        //    // Add foreground image since the background already has one
        //    if (Sprite != null)
        //    {
        //        var fgImage = ImageChildHelper(button, this);
        //        UIDetours.FG_IMAGE.Set(kButton, fgImage);
        //        sprite = fgImage.gameObject;
        //    }
        //    // Add text
        //    if (!string.IsNullOrEmpty(Text))
        //        text = TextChildHelper(button, TextStyle ?? PUITuning.Fonts.UILightStyle,
        //            Text).gameObject;
        //    // Add tooltip
        //    PUIElements.SetToolTip(button, ToolTip).SetActive(true);
        //    // Arrange the icon and text
        //    var layout = button.AddComponent<RelativeLayoutGroup>();
        //    layout.Margin = Margin;
        //    GameObject inner;
        //    ArrangeComponent(layout, inner = WrapTextAndSprite(text, sprite), TextAlignment);
        //    if (!DynamicSize) layout.LockLayout();
        //    layout.flexibleWidth = FlexSize.x;
        //    layout.flexibleHeight = FlexSize.y;
        //    DestroyLayoutIfPossible(button);
        //    InvokeRealize(button);
        //    return button;
        //}
    }

    public class ConfigGUI : MonoBehaviour
    {
        private static ConfigGUI? _instance;
        public static ConfigGUI Instance => _instance ?? (ConfigGUI)new GameObject(typeof(ConfigGUI).FullName).AddComponent(typeof(ConfigGUI));

        public static void ShowGUI()
        {
            try
            {
                Instance.IsVisible = true;
            } catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        //public void Start() { }
        //public void OnDestroy() { }
        //public void Update() { }

        public void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(this);
            StyleLine = new GUIStyle() { fixedHeight = 1, margin = new RectOffset(0, 0, 4, 4), };
            StyleLine.normal.background = new Texture2D(1, 1);
        }

        public Rect windowRect = new(20, 20, 200, 50);
        public GUIStyle StyleLine = null!;
        public bool IsVisible;

        public void OnGUI()
        {
            if (!IsVisible)
                return;
            // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/GUI.Window.html
            windowRect = GUILayout.Window(0, windowRect, DoMyWindow, "Debug Window");
            void DoMyWindow(int windowID)
            {
                GUILayout.Label("Foo bar Foo bar Foo bar Foo bar Foo bar Foo bar Foo bar Foo bar Foo bar Foo bar Foo bar Foo bar Foo bar Foo bar Foo bar Foo bar Foo bar.\nSecond line.");

                //if (GUILayout.Button("Button", GUILayout.ExpandWidth(false))) { }
                //GUILayout.BeginHorizontal();
                //GUILayout.EndHorizontal();
                //GUILayout.Space(5);
                //text1 = GUILayout.TextField(text1);
                //text2 = GUILayout.TextArea(text2);
                //bToggle = GUILayout.Toggle(bToggle, "Toggle");

                if (GUILayout.Button("Close", GUILayout.ExpandWidth(false))) IsVisible = false;
                GUI.DragWindow(new Rect(0, 0, 10000, 10000));
            }
        }
    }
}
