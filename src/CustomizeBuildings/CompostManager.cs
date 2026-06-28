using Common;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            return CustomizeBuildingsState.Instance.CompostFreshnessPercent > 0f;
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

    public readonly struct FoodStuff : IComparable<FoodStuff>
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
            return other.Freshness.CompareTo(this.Freshness);
        }
    }

    public class CompostManager : KMonoBehaviour, ISim4000ms
    {
        public static GameObject? Master;
        public static CompostManager? Instance;

        public int NextIndex;
        private readonly List<FoodStuff> _Foods = new();

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            CompostManager.Instance = this;
        }

        public void MarkForCompost(Pickupable pickupable, Compostable compostable)
        {
            pickupable.storage?.Drop(base.gameObject);
            EntitySplitter.Split(pickupable, pickupable.TotalAmount, compostable.compostPrefab);
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
            _Foods.Clear();
            foreach (var food in world.worldInventory.GetPickupables(GameTags.Edible) ?? Array.Empty<Pickupable>())
            {
                var rottable = food.GetSMI<Rottable.Instance>();
                if (rottable.IsNullOrStopped())
                    continue;

                float freshness = rottable.RotConstitutionPercentage;
                Helpers.PrintDebug($"CompostManager food={food.PrefabID()} percent={freshness} calories={food.GetComponent<Edible>().Calories}");

                var compostable = food.GetComponent<Compostable>();
                if (compostable == null || compostable.isMarkedForCompost)
                    continue;

                _Foods.Add(new FoodStuff(freshness, food));
            }

            // sort through fresh food first and count calories; when enough calories are available, mark stale food for compost
            _Foods.Sort();
            float calories = dupes * 1000f * CustomizeBuildingsState.Instance.CompostCaloriesPerDupe;
            float minimumFreshness = CustomizeBuildingsState.Instance.CompostFreshnessPercent;
            foreach (var foodstuff in _Foods)
            {
                Helpers.PrintDebug($"CompostManager food2={foodstuff.Pickupable.PrefabID()} percent={foodstuff.Freshness} calories={foodstuff.Pickupable.GetComponent<Edible>().Calories}");
                if (calories > 0)
                {
                    Helpers.PrintDebug($"skipped because colonie needs the calories");
                    calories -= foodstuff.Pickupable.GetComponent<Edible>().Calories;
                    continue;
                }

                if (foodstuff.Freshness < minimumFreshness)
                {
                    Helpers.PrintDebug($"composting");
                    MarkForCompost(foodstuff.Pickupable, foodstuff.Pickupable.GetComponent<Compostable>());
                }
            }

            // if we don't have enough food, don't compost any ingredients
            if (calories > 0)
                return;

            // mark stale ingredients for compost; ingredients don't have calories
            foreach (var food in world.worldInventory.GetPickupables(GameTags.CookingIngredient)?.ToArray() ?? [])
            {
                var rottable = food.GetSMI<Rottable.Instance>();
                if (rottable.IsNullOrStopped())
                    continue;

                float freshness = rottable.RotConstitutionPercentage;
                Helpers.PrintDebug($"CompostManager food3={food.PrefabID()} percent={freshness}");

                var compostable = food.GetComponent<Compostable>();
                if (compostable == null || compostable.isMarkedForCompost)
                    continue;

                if (freshness < minimumFreshness)
                {
                    Helpers.PrintDebug($"composting");
                    MarkForCompost(food, compostable);
                }
            }
        }
    }

    public class CompostSlider : KMonoBehaviour, IThresholdSwitch
    {
        #region IThresholdSwitch
        public float Threshold
        {
            get => CustomizeBuildingsState.Instance.CompostFreshnessPercent * 100f;
            set
            {
                float newvalue = value / 100f;
                if (newvalue != CustomizeBuildingsState.Instance.CompostFreshnessPercent)
                {
                    CustomizeBuildingsState.Instance.CompostFreshnessPercent = value / 100f;
                    CustomizeBuildingsState.Instance.TrySave();
                }
            }
        }
        public bool ActivateAboveThreshold
        {
            get => false;
            set => _ = value;
        }
        public float CurrentValue => 0f;
        public float RangeMin => 0f;
        public float RangeMax => 100f;
        public LocString Title => new("Compost Manager", "CustomizeBuildings.LOCSTRINGS.CompostManager");
        public LocString ThresholdValueName => STRINGS.CREATURES.STATS.ROT.NAME;
        public string AboveToolTip => "";
        public string BelowToolTip => "";
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
