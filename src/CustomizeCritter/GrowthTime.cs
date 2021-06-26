using HarmonyLib;
using Klei.AI;

namespace CustomizeCritter
{
    [HarmonyPatch(typeof(BabyMonitor), "IsReadyToSpawnAdult")]
    public class Patch_ChangeBabyTime   // (normally 5 days)
    {
        public static bool Prepare()
        {
            return CustomizeCritterState.StateManager.State.babyGrowupTime_inDays > 0f;
        }

        public static bool Prefix(BabyMonitor.Instance smi, ref bool __result)
        {
            AmountInstance amountInstance = Db.Get().Amounts.Age.Lookup(smi.gameObject);
            float num = CustomizeCritterState.StateManager.State.babyGrowupTime_inDays;
            if (Klei.GenericGameSettings.instance.acceleratedLifecycle)
            {
                num = 0.005f;
            }
            __result = amountInstance.value > num;
            return false;
        }
    }

}