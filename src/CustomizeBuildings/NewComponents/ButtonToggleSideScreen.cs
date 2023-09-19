using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomizeBuildings
{
    //[SkipSaveFileSerialization]
    [SerializationConfig(MemberSerialization.OptIn)]
    public abstract class ButtonToggleSideScreen : KMonoBehaviour, ISidescreenButtonControl
    {
        [Serialize] public bool Enabled = false;
        [NonSerialized] public int SortOrder;

        public string SidescreenButtonText => this.Enabled ? GetButtonTextOverride().Text : GetButtonTextOverride().CancelText;
        public string SidescreenButtonTooltip => this.Enabled ? GetButtonTextOverride().ToolTip : GetButtonTextOverride().CancelToolTip;
        public int ButtonSideScreenSortOrder() => this.SortOrder;
        public virtual int HorizontalGroupID() => -1;
        public bool SidescreenEnabled() => true;
        public bool SidescreenButtonInteractable() => true;
        public virtual void SetButtonTextOverride(ButtonMenuTextOverride textOverride) { }
        public abstract ButtonMenuTextOverride GetButtonTextOverride();

        public void OnSidescreenButtonPressed()
        {
            this.Enabled = !this.Enabled;
            Update();
        }

        public abstract void Update();

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Subscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
        }

        //[OnDeserialized]
        //public virtual void OnDeserialized()
        //{
        //    Update();
        //}

        public static readonly EventSystem.IntraObjectHandler<ButtonToggleSideScreen> OnCopySettingsDelegate = new((ButtonToggleSideScreen target, object source) =>
        {
            var component = ((GameObject)source).GetComponent<ButtonToggleSideScreen>();
            if (component != null)
            {
                target.Enabled = component.Enabled;
            }
        });
    }
}
