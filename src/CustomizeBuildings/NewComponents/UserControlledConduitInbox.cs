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
        private FilteredStorage filteredStorage;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();

            try {
                filteredStorage = (FilteredStorage)AccessTools.Field(typeof(SolidConduitInbox), "filteredStorage").GetValue(this.GetComponent<SolidConduitInbox>());
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

        public float AmountStored
        {
            get
            {
                return this.GetComponent<Storage>().MassStored();
            }
        }

        public LocString CapacityUnits
        {
            get
            {
                return GameUtil.GetCurrentMassUnit(false);
            }
        }

        public float MaxCapacity
        {
            get
            {
                return this.GetComponent<Storage>().capacityKg;
            }
        }

        public float MinCapacity
        {
            get
            {
                return 0f;
            }
        }

        public float UserMaxCapacity
        {
            get
            {
                return Mathf.Min(this.userMaxCapacity, this.GetComponent<Storage>().capacityKg);
            }
            set
            {
                this.userMaxCapacity = value;
                this.filteredStorage?.FilterChanged();
            }
        }

        public bool WholeValues
        {
            get
            {
                return false;
            }
        }
    }
    
}
