using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using KSerialization;
using Shared;
using System.Reflection.Emit;
using System.Reflection;
using Common;
using Klei.AI;
using Klei;

namespace CustomizeBuildings
{
    [HarmonyPatch]
    public static class PatchEquilibrium
    {
        [HarmonyPatch(typeof(RefrigeratorConfig), nameof(RefrigeratorConfig.DoPostConfigureComplete))]
        [HarmonyPrefix]
        public static void Fridge_Prefix(GameObject go)
        {
            //go.AddOrGet<SharedStorage>();
            go.AddOrGet<EquilibriumStorage>();
        }

        [HarmonyPatch(typeof(StorageLockerConfig), nameof(StorageLockerConfig.ConfigureBuildingTemplate))]
        [HarmonyPrefix]
        public static void StorageLocker_Prefix(GameObject go)
        {
            //go.AddOrGet<SharedStorage>();
            go.AddOrGet<EquilibriumStorage>();
        }
    }

    [SkipSaveFileSerialization]
    public class EquilibriumStorage : KMonoBehaviour, ISim200ms
    {
        public static List<EquilibriumStorage> AllStorages = new();
        //public static Tag[] FoodTags = EdiblesManager.s_allFoodTypes.Select(s => s.Id.ToTag()).ToArray(); //new Tag[] { GameTags.Edible, GameTags.CookingIngredient };
        public static Tag[] ClothsTags = new Tag[] { GameTags.Suit, GameTags.AtmoSuit, GameTags.JetSuit, GameTags.LeadSuit, GameTags.Clothes, GameTags.WarmVest, GameTags.CoolVest, GameTags.FunkyVest };
        public static float MinGeneral = 100f;
        public static float MinFood = 3f;
        public static float MinClothes = 3f;

        private Storage storage;
        private TreeFilterable filter;
        private List<GameObject> find = new();
        private float capacityMargin;

        public override void OnSpawn()
        {
            base.OnSpawn();
            this.storage = GetComponent<Storage>();
            this.filter = GetComponent<TreeFilterable>();
            this.capacityMargin = this.storage.capacityKg * 0.9f;
            AllStorages.Add(this);
            //Subscribe((int)GameHashes.OnStorageChange, OnStorageChanged);
        }

        public override void OnCleanUp()
        {
            //Unsubscribe((int)GameHashes.OnStorageChange, OnStorageChanged);
            AllStorages.Remove(this);
            base.OnCleanUp();
        }

        public void Sim200ms(float dt)
        {
            //if (UnityEngine.Random.Range(0, AllStorages.Count) > 0)
            //    return;
            if (this.storage.items.Count == 0)
                return;

            try
            {
                var go = this.storage.items.GetRandom();
                var prefab = go.GetComponent<KPrefabID>();
                float minAmount = prefab.tags.Contains(GameTags.Edible) ? MinFood : ClothsTags.Contains(prefab.PrefabTag) ? MinClothes : MinGeneral;

                GetSmallAndBigStorage(prefab.PrefabTag, minAmount, out var smallest, out var biggest, out float min, out float max);
                if (smallest == null || biggest == null || smallest == biggest)
                    return;

                float transfer = Mathf.Floor((min + max) * 0.5f - min);
                if (transfer < minAmount)
                    return;

                biggest.storage.Transfer(smallest.storage, prefab.PrefabTag, transfer, false, false);
            }
            catch (Exception)
            {
            }
        }

        public void OnStorageChanged(object data)
        {
            if (data is not GameObject item)
                return;

            var pickupable = item.GetComponent<Pickupable>();
            if (pickupable == null)
                return;

            var tag = pickupable.KPrefabID.PrefabTag;
            this.storage.FindFirst(tag);

            var amount = pickupable.primaryElement._units;
            Helpers.Print($"stored {pickupable.KPrefabID.PrefabTag} {amount} storage={pickupable.storage?.PrefabID()}");
            // pickupable.storage is not null and amount equals full item mass when material is taken (or new item stored)
            // pickupable.storage is null, when material was merged; amount equals mass merged only

            //if (amount > 0f)
            //{
            //    var storage2 = AllStorages.Find(f => f != null && f != this);
            //    if (storage2 != null)
            //        storage.Transfer(storage2.storage, pickupable.KPrefabID.PrefabTag, amount, true, true);
            //}
        }

        public void GetSmallAndBigStorage(Tag tag, float ignoreLessThan, out EquilibriumStorage smallest, out EquilibriumStorage biggest, out float min, out float max)
        {
            smallest = null;
            biggest = null;
            min = float.MaxValue;
            max = ignoreLessThan;

            for (int i = 0; i < AllStorages.Count; i++)
            {
                var eStorage = AllStorages[i];
                if (!eStorage.filter.acceptedTagSet.Contains(tag))
                    continue;

                float amount = eStorage.Find(tag);

                if (amount < min && eStorage.storage.MassStored() < this.capacityMargin)
                {
                    min = amount;
                    smallest = eStorage;
                }

                if (amount > max)
                {
                    max = amount;
                    biggest = eStorage;
                }
            }
        }

        public float Find(Tag tag)
        {
            this.find.Clear();
            this.storage.Find(tag, this.find);
            return this.find.Sum(s => s.GetComponent<PrimaryElement>()._units);
        }
    }

    // Game.Instance.fetchManager.pickups
    // position is stored in gameObject.transform.GetPosition()
    //[HarmonyPatch]
    public static class PatchSharedStorage
    {
        [HarmonyPatch(typeof(RefrigeratorConfig), nameof(RefrigeratorConfig.DoPostConfigureComplete))]
        [HarmonyPrefix]
        public static void Fridge_Prefix(GameObject go)
        {
            go.AddOrGet<SharedStorage>();
        }

        [HarmonyPatch(typeof(StorageLockerConfig), nameof(StorageLockerConfig.ConfigureBuildingTemplate))]
        [HarmonyPrefix]
        public static void StorageLocker_Prefix(GameObject go)
        {
            go.AddOrGet<SharedStorage>();
        }

        //[HarmonyPatch(typeof(EntitySplitter), nameof(EntitySplitter.Split))]
        //[HarmonyPrefix]
        //public static void OnSplit_Prefix(Pickupable pickupable, float amount, ref GameObject prefab, ref Pickupable __result)
        //{
        //    if (pickupable.storage is SharedStorage)
        //        prefab = Assets.GetPrefab(pickupable.KPrefabID.PrefabTag); // this makes zeromass objects stick
        //}

        //[HarmonyPatch(typeof(FetchManager), nameof(FetchManager.IsFetchablePickup))]
        //[HarmonyPrefix]
        //public static bool FetchPickup_Prefix(Pickupable pickup, FetchChore chore, Storage destination)
        //{
        //    var shared = pickup.storage as SharedStorage;
        //    if (shared == null)
        //        return true;

        //    Helpers.Print($"FetchPickup_Prefix1 id={pickup.KPrefabID.PrefabTag} chore={chore} dest={destination.PrefabID()}");

        //    if (destination is SharedStorage) // if source AND target is SharedStorage, do nothing (__result default to false)
        //        return false;

        //    float missing = pickup.UnreservedAmount - chore.amount;
        //    shared.Steal(chore.tagsFirst, missing);

        //    Helpers.Print($"FetchPickup_Prefix1 org={chore.originalAmount} total={chore.amount} missing={missing} unreserved={pickup.UnreservedAmount} stolen={missing - (pickup.UnreservedAmount - chore.amount)}");

        //    return true;
        //}

        //[HarmonyPatch(typeof(FetchManager), nameof(FetchManager.IsFetchablePickup_Exclude))]
        //[HarmonyPrefix]
        //public static bool FetchPickup_Prefix2(KPrefabID pickup_id, Storage source, float pickup_unreserved_amount, HashSet<Tag> exclude_tags, Tag required_tag, Storage destination)
        //{
        //    var shared = source as SharedStorage;
        //    if (shared == null)
        //        return true;

        //    Helpers.Print($"FetchPickup_Prefix2 id={pickup_id.PrefabTag} dest={destination.PrefabID()}");

        //    if (destination is SharedStorage) // if source AND target is SharedStorage, do nothing (__result default to false)
        //        return false;

        //    float missing = pickup_unreserved_amount - 1f;
        //    shared.Steal(pickup_id.PrefabTag, missing);
        //    Helpers.Print($"FetchPickup_Prefix2 pickup_unreserved_amount={pickup_unreserved_amount} missing={missing}");
        //    return true;
        //}

        //[HarmonyPatch(typeof(Pickupable), nameof(Pickupable.CouldBePickedUpCommon))]
        //[HarmonyPrefix]
        //public static bool FetchPickup_Prefix3(GameObject carrier, Pickupable __instance)
        //{
        //    var shared = __instance.storage as SharedStorage;
        //    if (shared == null)
        //        return true;

        //    shared.Steal(__instance.KPrefabID.PrefabTag, 1f);

        //    Helpers.Print($"FetchPickup_Prefix3 id={__instance.KPrefabID.PrefabTag} dest={carrier.PrefabID()}");
        //    return true;
        //}

        //[HarmonyPatch(typeof(FetchAreaChore.StatesInstance), nameof(FetchAreaChore.StatesInstance.Begin))]
        //[HarmonyPrefix]
        //public static bool FetchPickup_Prefix4()
        //{
        //    var shared = __instance.storage as SharedStorage;
        //    if (shared == null)
        //        return true;

        //    shared.Steal(__instance.KPrefabID.PrefabTag, 1f);

        //    Helpers.Print($"FetchPickup_Prefix3 id={__instance.KPrefabID.PrefabTag} dest={carrier.PrefabID()}");
        //    return true;
        //}
    }

    [SerializationConfig(MemberSerialization.OptIn)]
    public class SharedStorage : Storage
    {
        //public static List<GameObject> Shared = new();
        public static Dictionary<int, SharedStorage> Masters = new();
        public static List<SharedStorage> AllStorages = new();

        public int Priority;
        public int WorldId;

        private TreeFilterable filterable;

        public SharedStorage Master => Masters.GetValueSafe(this.WorldId);

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.WorldId = this.GetMyWorldId();
            //OnPriorityChanged(this.prioritizable.masterPrioritySetting);
            //this.prioritizable.onPriorityChanged += OnPriorityChanged;

            //this.storageFilters
            //this.items.First().GetComponent<Pickupable>().storage
            //this.items = Shared;

            //GameUtil.SubscribeToTags(this, Storage.OnDeadTagAddedDelegate, triggerImmediately: true);
            //base.Subscribe((int)GameHashes.QueueDestroyObject, Storage.OnQueueDestroyObjectDelegate);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            this.filterable = GetComponent<TreeFilterable>();
            this.filterable.OnFilterChanged += OnFilterChanged;
            OnFilterChanged();
            AllStorages.Add(this);
        }

        public override void OnCleanUp()
        {
            AllStorages.Remove(this);
            //this.prioritizable.onPriorityChanged -= OnPriorityChanged;
            this.filterable.OnFilterChanged -= OnFilterChanged;
            base.OnCleanUp();
        }

        /// <summary>
        /// Add fake placeholder that redirects to other storage.
        /// </summary>
        public Pickupable StorePlacebo(Tag tag)
        {
            var go = this.items.Find(f => f != null && f.GetComponent<KPrefabID>().PrefabTag == tag);
            if (go != null)
                return go.GetComponent<Pickupable>();

            var prefab = Assets.GetPrefab(tag); // pickupable.KPrefabID.PrefabTag);
            if (prefab == null)
            {
                Helpers.Print($"StorePlacebo prefab for {tag} is null!");
                return null;
            }
            go = GameUtil.KInstantiate(prefab, this.gameObject.transform.GetPosition(), Grid.SceneLayer.Ore);
            Helpers.Print($"spawn placebo at {this.gameObject.transform.GetPosition()}");
            var pickupable = go.GetComponent<Pickupable>();
            var primaryElement = go.AddOrGet<PrimaryElement>();
            primaryElement.KeepZeroMassObject = true;
            primaryElement.Units = 0f;
            primaryElement.Temperature = 293.15f;
            pickupable.primaryElement = primaryElement;
            go.SetActive(true);

            //primaryElement.ElementID = 
            Store(go, hide_popups: true, block_events: false, do_disease_transfer: false, is_deserializing: false);
            return pickupable;
        }

        /// <summary>
        /// Transfer requested material from other storages.
        /// </summary>
        public void Steal(Tag tag, float amount)
        {
            if (amount <= 0f)
                return;

            var target_pickup = StorePlacebo(tag);
            if (target_pickup == null)
            {
                Helpers.Print($"SharedStorage.Steal target is null");
                return;
            }

            float target_temp = target_pickup.primaryElement.Temperature;
            float target_amount = target_pickup.primaryElement.Units;

            Helpers.Print($"SharedStorage.Steal storage count={AllStorages.Count}");
            foreach (var storage in AllStorages)
            {
                if (storage == this)
                    continue;


                var source_go = storage.items.Find(f => f.GetComponent<KPrefabID>().PrefabTag == tag);
                if (source_go == null)
                {
                    Helpers.Print($"SharedStorage.Steal {storage.PrefabID()} storage has no {tag}");
                    continue;
                }

                var source_pickup = source_go.GetComponent<Pickupable>();
                float source_temp = source_pickup.primaryElement.Temperature;
                float source_amount = source_pickup.UnreservedAmount;
                if (source_amount <= 0f)
                {
                    Helpers.Print($"SharedStorage.Steal source has no unreserved amount");
                    continue;
                }

                float take_amount = Math.Min(amount, source_amount);
                target_temp = target_amount == 0 ? source_temp : SimUtil.CalculateFinalTemperature(target_amount, target_temp, take_amount, source_temp);
                target_amount += take_amount;

                target_pickup.primaryElement.SetTemperature(target_temp);
                target_pickup.primaryElement.Units = target_amount;
                source_pickup.primaryElement.Units -= take_amount;

                amount -= take_amount;
                Helpers.Print($"SharedStorage.Steal stole {take_amount}");
                if (amount <= 0f)
                    break;
            }
        }

        public void OnFilterChanged(HashSet<Tag> filters = null)
        {
            filters ??= this.filterable.GetTags();
            foreach (var tag in filters)
            {
                if (!this.items.Any(a => a.GetComponent<KPrefabID>().PrefabTag == tag))
                {
                    StorePlacebo(tag);
                    Steal(tag, 1000f);
                }
            }
        }

        public new void OnPriorityChanged(PrioritySetting priority)
        {
            this.Priority = priority.priority_value;
            if (this.Master == null)
            {
                Masters[this.WorldId] = this;
                return;
            }

            if (this.Priority < this.Master.Priority)
                Masters[this.WorldId] = this;
        }
    }
}
