using Klei.AI;
using System;
using UnityEngine;
using TUNING;
using Harmony;
using System.Linq;
using System.Collections.Generic;
using KSerialization;

namespace EggCritterSurplus
{
    public class Patches
    {
        public static void OnLoad()
        {
            Strings.Add("STRINGS.BUILDINGS.PREFABS.EGGCRITTERSURPLUS.NAME", EggCritterSurplusConfig.DisplayName);
            Strings.Add("STRINGS.BUILDINGS.PREFABS.EGGCRITTERSURPLUS.DESC", EggCritterSurplusConfig.Description);
            Strings.Add("STRINGS.BUILDINGS.PREFABS.EGGCRITTERSURPLUS.EFFECT", EggCritterSurplusConfig.Effect);
            TechHelper.AddBuildingToPlanScreen(TechHelper.Food, EggCritterSurplusConfig.Id);
            TechHelper.AddBuildingToTechnology(TechHelper.RanchingTech, EggCritterSurplusConfig.Id);
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

    public class EggCritterSurplus : KMonoBehaviour, IUserControlledCapacity, ICheckboxControl, ISim4000ms
    {
        private int _creatureCount;
        [Serialize]
        public float threshold = 8;
        [Serialize]
        public bool attackSurplus;
        //private Guid _roomStatusGuid;
        //private KSelectable _selectable;
        private static StatusItem capacityStatusItem;


        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();

            if (EggCritterSurplus.capacityStatusItem == null)
            {
                EggCritterSurplus.capacityStatusItem = new StatusItem("StorageLocker", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
                EggCritterSurplus.capacityStatusItem.resolveStringCallback = (Func<string, object, string>)((str, data) =>
                {
                    IUserControlledCapacity controlledCapacity = (IUserControlledCapacity)data;
                    string newValue1 = Util.FormatWholeNumber(Mathf.Floor(controlledCapacity.AmountStored));
                    string newValue2 = Util.FormatWholeNumber(controlledCapacity.UserMaxCapacity);
                    str = str.Replace("{Stored}", newValue1).Replace("{Capacity}", newValue2).Replace("{Units}", (string)controlledCapacity.CapacityUnits);
                    return str;
                });
            }
            KSelectable selectable = this.GetComponent<KSelectable>();
            if (selectable != null) selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, EggCritterSurplus.capacityStatusItem, (object)this);
        }

        #region ICheckboxControl

        public bool GetCheckboxValue()
        {
            return this.attackSurplus;
        }

        public void SetCheckboxValue(bool value)
        {
            this.attackSurplus = value;
        }

        public string CheckboxTitleKey
        {
            get
            {
                return STRINGS.UI.UISIDESCREENS.CAPTURE_POINT_SIDE_SCREEN.TITLE.key.String;
            }
        }

        public string CheckboxLabel
        {
            get
            {
                return new LocString("Wrangle critters as well");
            }
        }

        public string CheckboxTooltip
        {
            get
            {
                return new LocString("Mark youngest eggs for sweeping. If checked, then also mark critters for wrangle.");
            }
        }

        public bool IsMarkedForSweeping(Clearable clearable)
        {
            if (clearable == null) return false;

            return (bool)AccessTools.Field(typeof(Clearable), "isMarkedForClear").GetValue(clearable);
        }

        #endregion

        #region IUserControlledCapacity

        public float UserMaxCapacity
        {
            get
            {
                return (float)this.threshold;
            }

            set
            {
                this.threshold = Mathf.RoundToInt(value);
            }
        }

        public float AmountStored
        {
            get
            {
                return (float)this._creatureCount;
            }
        }

        public float MinCapacity
        {
            get
            {
                return 0.0f;
            }
        }

        public float MaxCapacity
        {
            get
            {
                return 100f;
            }
        }

        public bool WholeValues
        {
            get
            {
                return true;
            }
        }

        public LocString CapacityUnits
        {
            get
            {
                return STRINGS.UI.UISIDESCREENS.CAPTURE_POINT_SIDE_SCREEN.UNITS_SUFFIX;
            }
        }

        #endregion

        public void Sim4000ms(float dt)
        {
            try
            {
                CavityInfo room = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(this));
                if (room == null) return;

                int eggCount = room.eggs.Count;
                int critterCount = room.creatures.Count;
                this._creatureCount = eggCount + critterCount;

                if (threshold >= this._creatureCount) return;

                List<Indexer> ageList = new List<Indexer>();

                for (int i = 0; i < room.eggs.Count; i++) //foreach(KPrefabID egg in room.eggs)
                {
                    GameObject egg = null;
                    Clearable clearable = null;
                    AmountInstance incubation = null;
                    try { egg = room.eggs[i].gameObject; } catch (Exception) { Debug.Log("egg.gameObject was null"); }
                    if (egg != null)
                    {
                        clearable = egg.GetComponent<Clearable>();
                        incubation = Db.Get().Amounts.Incubation.Lookup(egg);
                    }

                    if (incubation != null && clearable != null)
                    {
                        if (!IsMarkedForSweeping(clearable))
                        {
                            ageList.Add(new Indexer(i, incubation.value));
                            //Debug.Log("egg is age:" + incubation.value);
                        }
                        else
                        {
                            ageList.Add(new Indexer(i, -1f));
                            eggCount--;
                        }
                    }
                    else
                    {
                        eggCount--;
                        ageList.Add(new Indexer(i, -2f));
                    }
                }

                if (threshold >= eggCount + critterCount) return;

                ageList = ageList.OrderBy(r => r.value).ToList();  //OrderByDescending

                //Debug.Log("Egg Age List: ");
                //for (int i = 0; i < ageList.Count; i++)
                //{
                //    Debug.Log("No: " + i + "age: " + ageList[i].value + "index: " + ageList[i].index);
                //}

                for (int i = 0; i < ageList.Count; i++)
                {
                    if (ageList[i].value >= 0f)
                    {
                        Clearable clearable = room.eggs[ageList[i].index].gameObject.GetComponent<Clearable>();
                        clearable.MarkForClear();
                        eggCount--;
                        if (threshold >= eggCount + critterCount) return;
                    }
                }

                if (!attackSurplus) return;

                for (int i = 0; i < room.creatures.Count; i++) //foreach (KPrefabID creature in room.creatures)
                {
                    GameObject creature = null;
                    try { creature = room.creatures[i].gameObject; } catch (Exception) { Debug.Log("creature.gameObject was null"); }

                    if (creature != null)
                    {
                        Capturable capturable = creature.GetComponent<Capturable>();
                        if (capturable != null && (capturable.IsMarkedForCapture || capturable.gameObject.HasTag(GameTags.Creatures.Bagged) || capturable.gameObject.HasTag(GameTags.Stored)))
                        {
                            critterCount--;
                            if (threshold >= eggCount + critterCount) return;
                        }
                    }
                    else
                    {
                        critterCount--;
                        if (threshold >= eggCount + critterCount) return;
                    }
                }

                for (int i = 0; i < room.creatures.Count; i++) //foreach (KPrefabID creature in room.creatures)
                {
                    GameObject creature = null;
                    try { creature = room.creatures[i].gameObject; } catch (Exception) { Debug.Log("creature.gameObject was null"); }

                    if (creature != null)
                    {
                        Capturable capturable = creature.GetComponent<Capturable>();
                        if (capturable != null &&
                            !capturable.IsMarkedForCapture &&
                            !capturable.gameObject.HasTag(GameTags.Trapped) &&
                            !capturable.gameObject.HasTag(GameTags.Stored) &&
                            !capturable.gameObject.HasTag(GameTags.Creatures.Bagged))
                        {
                            var priority = this.GetComponent<Prioritizable>();

                            //bool flag = capturable.allowCapture;
                            capturable.allowCapture = true;
                            if (priority == null)
                                capturable.MarkForCapture(true);
                            else
                                capturable.MarkForCapture(true, priority.GetMasterPriority());
                            //capturable.allowCapture = flag;
                            critterCount--;
                            if (threshold >= eggCount + critterCount) return;
                        }
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