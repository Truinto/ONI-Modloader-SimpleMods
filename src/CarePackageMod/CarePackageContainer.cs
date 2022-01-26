//#define LOCALE
using System;

namespace CarePackageMod
{
    public class CarePackageContainer
    {
        public string ID;
        public float amount;
        public int? onlyAfterCycle;
        public int? onlyUntilCycle;
        [NonSerialized]
        public float multiplier = 1f;

        public CarePackageContainer(string ID, float amount, int? onlyAfterCycle = null, int? onlyUntilCycle = null)
        {
            this.ID = ID;
            this.amount = amount;
            this.onlyAfterCycle = onlyAfterCycle; 
            this.onlyUntilCycle = onlyUntilCycle;
        }

        public CarePackageInfo ToInfo()
        {
            float amount = (float)Math.Max(Math.Round(this.amount * this.multiplier, 0), 1.0);
            return new CarePackageInfo(this.ID, amount, (() => CheckCondition(this.ID, onlyAfterCycle, onlyUntilCycle)));
        }

        public static implicit operator CarePackageInfo(CarePackageContainer container)
        {
            return container.ToInfo();
        }

        public static implicit operator CarePackageContainer(CarePackageInfo info)
        {
            return new CarePackageContainer(info.id, info.quantity, 0);
        }

        public static bool CheckCondition(string id, int? afterCycle = null, int? untilCycle = null)
        {
            bool after = GameClock.Instance.GetCycle() > (afterCycle ?? 0);
            bool until = GameClock.Instance.GetCycle() <= (untilCycle ?? int.MaxValue);
            bool discover = !CarePackageState.StateManager.State.allowOnlyDiscoveredElements || DiscoveredResources.Instance.IsDiscovered(id.ToTag());

            return after && until && discover;
        }
    }
}