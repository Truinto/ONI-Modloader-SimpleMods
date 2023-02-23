using Klei.AI;
using System;
using UnityEngine;
using TUNING;
using HarmonyLib;
using System.Linq;
using System.Collections.Generic;
using KSerialization;
using System.Reflection;
using Common;

namespace EggCritterSurplus
{
    public class Patches
    {
        public static void OnLoad()
        {
            Helpers.Print("Loading EggCritterSurplus building.");

            Strings.Add("STRINGS.BUILDINGS.PREFABS.EGGCRITTERSURPLUS.NAME", EggCritterSurplusConfig.DisplayName);
            Strings.Add("STRINGS.BUILDINGS.PREFABS.EGGCRITTERSURPLUS.DESC", EggCritterSurplusConfig.Description);
            Strings.Add("STRINGS.BUILDINGS.PREFABS.EGGCRITTERSURPLUS.EFFECT", EggCritterSurplusConfig.Effect);
            //TechHelper.AddBuildingStrings(EggCritterSurplusConfig.Id, EggCritterSurplusConfig.DisplayName, EggCritterSurplusConfig.Description, EggCritterSurplusConfig.Effect);

            TechHelper.AddBuildingToPlanScreen(EggCritterSurplusConfig.Id, PlanScreens.Food);
            TechHelper.AddOnLoad.Add(new TechContainer(EggCritterSurplusConfig.Id, TechGroups.Ranching));
        }
    }

    public class EggCritterSurplusConfig : IBuildingConfig
    {
        public static string Id = "EggCritterSurplus";
        public static string DisplayName = "Ranching Bulletin Board";
        public static string Description = "Tells your rancher how many critters are allowed in this stable. Surplus will be marked for removal. The youngest eggs will always be picked first.";
        public static string Effect = "Marks excess eggs for sweeping.";

        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(EggCritterSurplusConfig.Id, 1, 1, "critter_sensor_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER0, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
            buildingDef.AudioCategory = "Metal";
            buildingDef.ViewMode = OverlayModes.Rooms.ID;
            buildingDef.Floodable = false;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            Prioritizable.AddRef(go);
            go.AddOrGet<EggCritterSurplus>();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
        }
    }

    public class EggCritterSurplus : KMonoBehaviour, IUserControlledCapacity, ISim4000ms//, ICheckboxControl
    {
        private int creatureCount;
        [Serialize] public float threshold = 8;
        [Serialize] public bool attackSurplus;
        private static StatusItem capacityStatusItem;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();

            if (EggCritterSurplus.capacityStatusItem == null)
            {
                EggCritterSurplus.capacityStatusItem = new StatusItem("StorageLocker", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
                EggCritterSurplus.capacityStatusItem.resolveStringCallback = (str, data) =>
                {
                    IUserControlledCapacity controlledCapacity = (IUserControlledCapacity)data;
                    string newValue1 = Util.FormatWholeNumber(Mathf.Floor(controlledCapacity.AmountStored));
                    string newValue2 = Util.FormatWholeNumber(controlledCapacity.UserMaxCapacity);
                    str = str.Replace("{Stored}", newValue1).Replace("{Capacity}", newValue2).Replace("{Units}", controlledCapacity.CapacityUnits);
                    return str;
                };
            }
            this.GetComponent<KSelectable>()?.SetStatusItem(Db.Get().StatusItemCategories.Main, EggCritterSurplus.capacityStatusItem, this);
        }

        #region ICheckboxControl
        public bool GetCheckboxValue() => this.attackSurplus;
        public void SetCheckboxValue(bool value) => this.attackSurplus = value;
        public string CheckboxTitleKey => STRINGS.UI.UISIDESCREENS.CAPTURE_POINT_SIDE_SCREEN.TITLE.key.String;
        public string CheckboxLabel => new LocString("Wrangle critters as well");
        public string CheckboxTooltip => new LocString("Mark youngest eggs for sweeping. If checked, then also mark critters for wrangle.");
        #endregion

        #region IUserControlledCapacity
        public float UserMaxCapacity
        {
            get => (float)this.threshold;
            set => this.threshold = Mathf.RoundToInt(value);
        }
        public float AmountStored => (float)this.creatureCount;
        public float MinCapacity => 0f;
        public float MaxCapacity => 100f;
        public bool WholeValues => true;
        public LocString CapacityUnits => STRINGS.UI.UISIDESCREENS.CAPTURE_POINT_SIDE_SCREEN.UNITS_SUFFIX;
        #endregion

        public static FieldInfo _isMarkedForClear = AccessTools.Field(typeof(Clearable), "isMarkedForClear");
        public bool IsMarkedForSweeping(GameObject egg)
        {
            var clearable = egg.GetComponent<Clearable>();
            if (clearable != null)
                return (bool)_isMarkedForClear.GetValue(clearable);
            return true;    //we don't want to work with it, if Clearable were to be missing
        }
        public bool IsInStorage(GameObject egg)
        {
            var pickupable = egg.GetComponent<Pickupable>();
            if (pickupable != null)
                return pickupable?.storage != null;
            return true;    //we don't want to work with it, if Pickupable were to be missing
        }

        public bool IsInRoom(GameObject egg, CavityInfo room)
        {
            return Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(egg))?.handle == room.handle;
        }

        public void RefreshCavityEggs(CavityInfo room)
        {
            room.eggs.Clear();
            var allEggs = this.GetMyWorld().worldInventory.GetPickupables(GameTags.Egg);
            foreach (var egg in allEggs)
                if (IsInRoom(egg.gameObject, room))
                    room.eggs.Add(egg.KPrefabID);
        }

        public void Sim4000ms(float dt)
        {
            try
            {
                CavityInfo room = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(this.gameObject));
                if (room == null || room.numCells > 200) return;
                //this.Trigger((int)GameHashes.UpdateRoom, room.room);
                //Game.Instance.roomProber.UpdateRoom(room);

                int eggCount = room.eggs.Count;
                int critterCount = room.creatures.Count;
                this.creatureCount = eggCount + critterCount;

                if (threshold >= this.creatureCount) return;

                List<Indexer> ageList = new List<Indexer>();

                for (int i = room.eggs.Count - 1; i >= 0; i--)
                {
                    GameObject egg = room.eggs[i]?.gameObject;
                    float incubation = egg != null ? Db.Get().Amounts.Incubation.Lookup(egg).value : -2f;

                    if (egg == null)
                    {
                        ageList.Add(new Indexer(i, -1f));
                    }
                    else if (IsMarkedForSweeping(egg) || IsInStorage(egg))
                    {
                        ageList.Add(new Indexer(i, -1f));
                        eggCount--;
                    }
                    else if (IsInRoom(egg, room))
                    {
                        ageList.Add(new Indexer(i, incubation));
                        //Debug.Log("egg is age:" + incubation);
                    }
                    else
                    {
                        ageList.Add(new Indexer(i, -1f));
                        Debug.Log("egg isn't in room?!");
                        RefreshCavityEggs(room);
                        return;
                    }
                }

                if (threshold >= eggCount + critterCount) return;

                ageList = ageList.OrderBy(r => r.value).ToList();  //OrderByAscending

                //Debug.Log("Egg Age List: ");
                //for (int i = 0; i < ageList.Count; i++)
                //{
                //    Debug.Log("No: " + i + "age: " + ageList[i].value + "index: " + ageList[i].index);
                //}

                var priority = this.GetComponent<Prioritizable>()?.GetMasterPriority() ?? new PrioritySetting(PriorityScreen.PriorityClass.basic, 5);

                for (int i = 0; i < ageList.Count; i++)
                {
                    if (ageList[i].value >= 0f)
                    {
                        var clearable = room.eggs[ageList[i].index].GetComponent<Clearable>();
                        clearable.MarkForClear();
                        clearable.GetComponent<Prioritizable>()?.SetMasterPriority(priority);
                        eggCount--;
                        if (threshold >= eggCount + critterCount)
                            return;
                    }
                }

                if (!attackSurplus) return;

                for (int i = 0; i < room.creatures.Count; i++)
                {
                    Capturable capturable = room.creatures[i]?.GetComponent<Capturable>();
                    if (capturable != null && (capturable.IsMarkedForCapture || capturable.HasTag(GameTags.Creatures.Bagged) || capturable.HasTag(GameTags.Stored)))
                    {
                        critterCount--;
                    }
                }

                if (threshold >= eggCount + critterCount) return;

                for (int i = 0; i < room.creatures.Count; i++)
                {
                    Capturable capturable = room.creatures[i]?.GetComponent<Capturable>();
                    if (capturable != null &&
                            !capturable.IsMarkedForCapture &&
                            !capturable.HasTag(GameTags.Trapped) &&
                            !capturable.HasTag(GameTags.Stored) &&
                            !capturable.HasTag(GameTags.Creatures.Bagged))
                    {
                        //bool flag = capturable.allowCapture;
                        capturable.allowCapture = true;
                        capturable.MarkForCapture(true, priority);
                        //capturable.allowCapture = flag;
                        critterCount--;
                        if (threshold >= eggCount + critterCount)
                            return;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Wups, this shouldn't happen: " + e.ToString());
            }

        }

        public struct Indexer
        {
            public int index;
            public float value;

            public Indexer(int index, float value)
            {
                this.index = index;
                this.value = value;
            }
        }
    }
}