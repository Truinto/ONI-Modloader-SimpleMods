using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace FumLib
{
    public class UserControlledValue : KMonoBehaviour, IUserControlledCapacity
    {
        public FieldInfo Info;
        public int Max = 10;
        public int Min = 0;
        public LocString Unit = "";
        public Type ComponentType;
        private int currentvalue;
        private object component;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            component = this.GetComponent(ComponentType);
            currentvalue = (int)Info.GetValue(component);
        }

        public float AmountStored
        {
            get
            {
                return currentvalue;
            }
        }

        public LocString CapacityUnits
        {
            get
            {
                return Unit;
            }
        }

        public float MaxCapacity
        {
            get
            {
                return Max;
            }
        }

        public float MinCapacity
        {
            get
            {
                return Min;
            }
        }

        public float UserMaxCapacity
        {
            get
            {
                return currentvalue;
            }

            set
            {
                currentvalue = Math.Min(Max, (int)value);
                Info.SetValue(component, currentvalue);
            }
        }

        public bool WholeValues
        {
            get
            {
                return true;
            }
        }
    }
}
