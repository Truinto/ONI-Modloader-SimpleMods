using Common;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomizeBuildings
{
    [HarmonyPatch]
    public class CompostManagerPatch
    {
        public static bool Prepare()
        {
            return CustomizeBuildingsState.StateManager.State.CompostFreshnessPercent > 0f;
        }

        [HarmonyPatch(typeof(CompostConfig), nameof(CompostConfig.ConfigureBuildingTemplate))]
        [HarmonyPostfix]
        public static void GameLoad(GameObject go)
        {
            go.AddOrGet<CompostSlider>();

            if (CompostManager.Instance == null)
            {
                //CompostManager.Instance = new();
                GameObject go2 = new("CompostManager");
                go2.AddOrGet<CompostManager>();
                CompostManager.Master = go2;
            }
        }

        [HarmonyPatch(typeof(Game), "DestroyInstances")]
        [HarmonyPostfix]
        public static void GameDestroy()
        {
            CompostManager.Instance = null;
        }
    }

    public struct FoodStuff : IComparable<FoodStuff>
    {
        public FoodStuff(float Freshness, Pickupable Pickupable)
        {
            this.Freshness = Freshness;
            this.Pickupable = Pickupable;
        }

        public readonly float Freshness;
        public readonly Pickupable Pickupable;

        // fresh food comes first
        public int CompareTo(FoodStuff other)
        {
            if (this.Freshness > other.Freshness)
                return -1;
            if (this.Freshness < other.Freshness)
                return 1;
            return 0;
        }
    }

    public class CompostManager : KMonoBehaviour, ISim4000ms
    {
        public static GameObject Master;
        public static CompostManager Instance;

        public int NextIndex;
        private readonly List<FoodStuff> foods = new();

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            CompostManager.Instance = this;
        }

        public void Sim4000ms(float dt)
        {
            // only one world per call
            var world = ClusterManager.Instance.WorldContainers.ElementAtOrDefault(NextIndex++);
            if (world == null)
            {
                NextIndex = 0;
                return;
            }

            int dupes = Components.LiveMinionIdentities.GetWorldItems(world.id, false).Count;
            Helpers.PrintDebug($"CompostManager world={world.id} dupes={dupes}");
            if (dupes == 0)
                return;

            // get all edible food and record freshness
            foreach (var food in world.worldInventory.GetPickupables(GameTags.Edible) ?? Array.Empty<Pickupable>())
            {
                Helpers.PrintDebug($"CompostManager food={food.PrefabID()}");

                var compostable = food.GetComponent<Compostable>();
                if (compostable == null || compostable.isMarkedForCompost)
                    continue;

                var rottable = food.GetSMI<Rottable.Instance>();
                if (rottable.IsNullOrStopped())
                    continue;

                float freshness = rottable.RotConstitutionPercentage;
                foods.Add(new FoodStuff(freshness, food));
                Helpers.PrintDebug($"CompostManager food={food.PrefabID()} percent={freshness} calories={food.GetComponent<Edible>().Calories}");
            }

            // sort through fresh food first and count calories; when enough calories are available, mark stale food for compost
            foods.Sort();
            float calories = dupes * 1000f * CustomizeBuildingsState.StateManager.State.CompostCaloriesPerDupe;
            float minimumFreshness = CustomizeBuildingsState.StateManager.State.CompostFreshnessPercent;
            foreach (var foodstuff in foods)
            {
                if (calories > 0)
                {
                    calories -= foodstuff.Pickupable.GetComponent<Edible>().Calories;
                    continue;
                }

                if (foodstuff.Freshness <= minimumFreshness)
                {
                    foodstuff.Pickupable.GetComponent<Compostable>().isMarkedForCompost = true;
                    foodstuff.Pickupable.storage?.Drop(foodstuff.Pickupable.gameObject, true);
                }
            }
            foods.Clear();

            // if we don't have enough food, don't compost any ingredients
            if (calories > 0)
                return;

            // mark stale ingredients for compost; ingredients don't have calories
            foreach (var food in world.worldInventory.GetPickupables(GameTags.CookingIngredient) ?? Array.Empty<Pickupable>())
            {
                var compostable = food.GetComponent<Compostable>();
                if (compostable == null || compostable.isMarkedForCompost)
                    continue;

                var rottable = food.GetSMI<Rottable.Instance>();
                if (rottable.IsNullOrStopped())
                    continue;

                float freshness = rottable.RotConstitutionPercentage;
                if (freshness <= minimumFreshness)
                {
                    compostable.isMarkedForCompost = true;
                    food.storage?.Drop(food.gameObject, true);
                }
                Helpers.PrintDebug($"CompostManager food={food.PrefabID()} percent={freshness}");
            }
        }
    }

    public class CompostSlider : KMonoBehaviour, IThresholdSwitch
    {
        #region IThresholdSwitch
        public float Threshold
        {
            get
            {
                return CustomizeBuildingsState.StateManager.State.CompostFreshnessPercent * 100f;
            }
            set
            {
                CustomizeBuildingsState.StateManager.State.CompostFreshnessPercent = value / 100f;
            }
        }
        public bool ActivateAboveThreshold
        {
            get
            {
                return false;
            }

            set
            {
                CustomizeBuildingsState.StateManager.TrySaveConfigurationState();
            }
        }
        public float CurrentValue => 0f;
        public float RangeMin => 0f;
        public float RangeMax => 100f;
        public LocString Title => "Compost Manager";
        public LocString ThresholdValueName => STRINGS.CREATURES.STATS.ROT.NAME;
        public string AboveToolTip => "save to settings";
        public string BelowToolTip => "save to settings";
        public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;
        public int IncrementScale => 1;
        public NonLinearSlider.Range[] GetRanges => NonLinearSlider.GetDefaultRange(RangeMax);
        public string Format(float value, bool units) => value.ToString("0");
        public float GetRangeMaxInputField() => 100f;
        public float GetRangeMinInputField() => 0f;
        public float ProcessedInputValue(float input) => input;
        public float ProcessedSliderValue(float input) => Mathf.Round(input);
        public LocString ThresholdValueUnits() => STRINGS.CREATURES.STATS.ROT.NAME;
        #endregion
    }
}
