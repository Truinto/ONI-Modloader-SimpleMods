using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HarmonyLib;
using KSerialization;

namespace CustomizeBuildings
{
    public class UserControlledConduitInbox : KMonoBehaviour, IUserControlledCapacity
    {
        [Serialize]
        public float userMaxCapacity = float.PositiveInfinity;
        private FilteredStorage? filteredStorage;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();

            try {
                filteredStorage = (FilteredStorage)AccessTools.Field(typeof(SolidConduitInbox), "filteredStorage").GetValue(GetComponent<SolidConduitInbox>());
                AccessTools.Field(typeof(FilteredStorage), "capacityControl").SetValue(filteredStorage, this);
            } catch (Exception) {
                Debug.LogWarning("[CustomizeBuildings] Wups. UserControlledConduitInbox couldn't be set up.");
            }
        }

        public override void OnCleanUp()
        {
            filteredStorage = null;
            base.OnCleanUp();
        }

        bool IUserControlledCapacity.ControlEnabled() => true;

        public float AmountStored => GetComponent<Storage>().MassStored();

        public LocString CapacityUnits => GameUtil.GetCurrentMassUnit(false);

        public float MaxCapacity => GetComponent<Storage>().capacityKg;

        public float MinCapacity => 0f;

        public float UserMaxCapacity
        {
            get
            {
                return Mathf.Min(this.userMaxCapacity, GetComponent<Storage>().capacityKg);
            }
            set
            {
                this.userMaxCapacity = value;
                this.filteredStorage?.FilterChanged();
            }
        }

        public bool WholeValues => false;
    }
    
}
