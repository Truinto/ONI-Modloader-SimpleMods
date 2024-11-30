//#define LOCALE
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System;
using KSerialization;
using TUNING;
using Klei.AI;
using Common;
using System.Runtime.CompilerServices;

namespace CustomizeBuildings
{
    internal static class SkillStationCosts
    {
        internal static bool IsEnabled => CustomizeBuildingsState.StateManager.State.SkillStationEnabled;
        internal static float CostTime => CustomizeBuildingsState.StateManager.State.SkillStationCostTime;
        internal static float CostReset => CustomizeBuildingsState.StateManager.State.SkillStationCostReset;
        internal static float CostRemoveTrait => CustomizeBuildingsState.StateManager.State.SkillStationCostRemoveTrait;
        internal static float CostAddTrait => CustomizeBuildingsState.StateManager.State.SkillStationCostAddTrait;
        internal static float CostBadTrait => CustomizeBuildingsState.StateManager.State.SkillStationCostBadTrait;
        internal static float CostAddAptitude => CustomizeBuildingsState.StateManager.State.SkillStationCostAddAptitude;
        internal static float CostAddAttribute => CustomizeBuildingsState.StateManager.State.SkillStationCostAddAttribute;

        /// <summary>
        /// Returns true if minion can spend <paramref name="cost"/> without going into negative skill points.
        /// </summary>
        internal static bool HasEnoughExp(this MinionResume minion, float cost)
        {
            if (cost <= 0f)
                return true;

            int skillpoints_next = MinionResume.CalculateTotalSkillPointsGained(minion.TotalExperienceGained - cost);
            int skillpoints_used = minion.SkillsMastered - minion.GrantedSkillIDs.Count;
            return minion.TotalExperienceGained >= cost && skillpoints_next >= skillpoints_used;
        }
    }

    [HarmonyPatch(typeof(Filterable), nameof(Filterable.GetTagOptions))]
    public class SkillStation_Options_Patch
    {
        public static bool Prepare()
        {
            return SkillStationCosts.IsEnabled;
        }

        /*
         * Debug.Log($"[SkillStation] 1 PrefabID={go.PrefabID()} name={go.name} GetProperName={go.GetProperName()}");
         * Debug.Log("[SkillStation] 2 " + minion);
         * Debug.Log($"[SkillStation] 3 {minion.GetIdentity.GetProperName()} : {minion.TotalExperienceGained} - {minion.AptitudeBySkillGroup}");
         * Debug.Log("[SkillStation] 4 " + traits?.GetTraitIds()?.Join());
         * [20:13:37.227] [1] [INFO] [SkillStation] 1 PrefabID=ResetSkillsStation name=ResetSkillsStationComplete GetProperName=<link="RESETSKILLSSTATION">Skill Scrubber</link>
         * [20:13:37.227] [1] [INFO] [SkillStation] 2 Jean (MinionResume)
         * [20:13:37.227] [1] [INFO] [SkillStation] 3 Jean : 1016.572 - System.Collections.Generic.Dictionary`2[HashedString,System.Single]
         * [20:13:37.227] [1] [INFO] [SkillStation] 4 DiggingDown, FastLearner, Aggressive, SparkleStreaker
         *
         * Debug.Log("[SkillStation] AptitudeBySkillGroup: " + minion.AptitudeBySkillGroup.Select(x => x.Key + ":" + x.Value).Join());
         * Debug.Log("[SkillStation] AptitudeByRoleGroup: " + minion.AptitudeByRoleGroup.Select(x => x.Key + ":" + x.Value).Join());
         * __result["-Chore Groups"] = new HashSet<Tag>(Db.Get().ChoreGroups.resources.Select(x => (Tag)x.Id));
         * __result["-Attributes"] = new HashSet<Tag>(Db.Get().ChoreGroups.resources.Select(x => (Tag)x.attribute.Id));
         * 
         * [19:45:37.550] [1] [INFO] [SkillStation] OnAssign Refresh
         * [20:49:38.541] [1] [INFO] [SkillStation] Good Traits: Twinkletoes, StrongArm, Greasemonkey, DiversLung, IronGut, StrongImmuneSystem, EarlyBird, NightOwl, MoleHands, FastLearner, InteriorDecorator, Uncultured, SimpleTastes, Foodie, BedsideManner, DecorUp, Thriver, GreenThumb, ConstructionUp, RanchingUp, GrantSkill_Mining1, GrantSkill_Mining2, GrantSkill_Mining3, GrantSkill_Farming2, GrantSkill_Ranching1, GrantSkill_Cooking1, GrantSkill_Arting1, GrantSkill_Arting2, GrantSkill_Arting3, GrantSkill_Suits1, GrantSkill_Technicals2, GrantSkill_Engineering1, GrantSkill_Basekeeping2, GrantSkill_Medicine2
         * [20:49:38.541] [1] [INFO] [SkillStation] Bad Traits: CantResearch, CantDig, CantCook, CantBuild, Hemophobia, ScaredyCat, ConstructionDown, RanchingDown, CaringDown, BotanistDown, ArtDown, CookingDown, MachineryDown, DiggingDown, SlowLearner, NoodleArms, DecorDown, Anemic, Flatulence, IrritableBowel, Snorer, MouthBreather, SmallBladder, CalorieBurner, WeakImmuneSystem, Allergies, NightLight, Narcolepsy
         * [20:49:38.541] [1] [INFO] [SkillStation] Gene Traits: Regeneration, DeeperDiversLungs, SunnyDisposition, RockCrusher
         * [20:49:38.541] [1] [INFO] [SkillStation] Congenital Traits: None, Joshua, Ellie, Stinky, Liam
         * [20:49:38.541] [1] [INFO] [SkillStation] Stress Traits: Aggressive, StressVomiter, UglyCrier, BingeEater
         * [20:49:38.541] [1] [INFO] [SkillStation] Joy Traits: BalloonArtist, SparkleStreaker, StickerBomber, SuperProductive
         * [20:49:38.541] [1] [INFO] [SkillStation] Aptitudes: Mining, Building, Farming, Ranching, Cooking, Art, Research, Rocketry, Suits, Hauling, Technicals, MedicalAid, Basekeeping
         * [20:49:38.541] [1] [INFO] [SkillStation] Attributes: Attribute Strength, Attribute Caring, Attribute Construction, Attribute Digging, Attribute Machinery, Attribute Learning, Attribute Cooking, Attribute Botanist, Attribute Art, Attribute Ranching, Attribute Athletics, Attribute SpaceNavigation
         * [20:49:38.541] [1] [INFO] [SkillStation] Dupe traits: DiggingDown, FastLearner, Aggressive, Liam, Allergies
         * [20:49:38.541] [1] [INFO] [SkillStation] Dupe attributes: SpaceNavigation:0, Construction:4, Digging:0, Machinery:0, Athletics:0, Learning:0, Cooking:0, Caring:0, Strength:5, Art:0, Botanist:0, Ranching:0, PowerTinker:0, FarmTinker:0, Immunity:0, LifeSupport:0, Toggle:0
         * [20:49:38.541] [1] [INFO] [SkillStation] Dupe aptitudes: Building, Hauling
         */

        public static HashSet<Tag> GoodTraits;
        public static HashSet<Tag> BadTraits;
        public static HashSet<Tag> GeneTraits;
        public static HashSet<Tag> CongenitalTraits;
        public static HashSet<Tag> StressTraits;
        public static HashSet<Tag> JoyTraits;
        public static HashSet<Tag> Aptitudes;
        public static HashSet<Tag> Attributes;

        public static void Init()
        {
            BadTraits = new HashSet<Tag>(DUPLICANTSTATS.BADTRAITS.Select(GetTag));
            GoodTraits = new HashSet<Tag>(DUPLICANTSTATS.GOODTRAITS.Select(GetTag));
            GeneTraits = new HashSet<Tag>(DUPLICANTSTATS.GENESHUFFLERTRAITS.Select(GetTag));
            CongenitalTraits = new HashSet<Tag>(DUPLICANTSTATS.CONGENITALTRAITS.Select(GetTag));
            StressTraits = new HashSet<Tag>(DUPLICANTSTATS.STRESSTRAITS.Select(GetTag));
            JoyTraits = new HashSet<Tag>(DUPLICANTSTATS.JOYTRAITS.Select(GetTag));
            Attributes = new HashSet<Tag>(DUPLICANTSTATS.ALL_ATTRIBUTES.Select(x => GetTag("Attribute " + x)));
            Aptitudes = new HashSet<Tag>(Db.Get().SkillGroups.resources.Select(x => GetTag(x.Id)));

            int count = 0;
            count += BadTraits.RemoveWhere(w => GoodTraits.Contains(w) || GeneTraits.Contains(w) || CongenitalTraits.Contains(w) || StressTraits.Contains(w) || JoyTraits.Contains(w) || Attributes.Contains(w) || Aptitudes.Contains(w));
            count += GoodTraits.RemoveWhere(w => GeneTraits.Contains(w) || CongenitalTraits.Contains(w) || StressTraits.Contains(w) || JoyTraits.Contains(w) || Attributes.Contains(w) || Aptitudes.Contains(w));
            count += GeneTraits.RemoveWhere(w => CongenitalTraits.Contains(w) || StressTraits.Contains(w) || JoyTraits.Contains(w) || Attributes.Contains(w) || Aptitudes.Contains(w));
            count += CongenitalTraits.RemoveWhere(w => StressTraits.Contains(w) || JoyTraits.Contains(w) || Attributes.Contains(w) || Aptitudes.Contains(w));
            count += StressTraits.RemoveWhere(w => JoyTraits.Contains(w) || Attributes.Contains(w) || Aptitudes.Contains(w));
            count += JoyTraits.RemoveWhere(w => Attributes.Contains(w) || Aptitudes.Contains(w));
            count += Attributes.RemoveWhere(w => Aptitudes.Contains(w));
            Helpers.Print($"Skillstation cleaned {count} duplicates."); //should be zero
        }

        private static Tag GetTag(string id)
        {
            Tag result = (Tag)id;
            if (!TagManager.ProperNames.ContainsKey(result))
            {
                Helpers.StringsTagShort(id, id);
            }
            return result;
        }

        private static Tag GetTag(DUPLICANTSTATS.TraitVal traitVal)
        {
            Tag result = (Tag)traitVal.id;
            if (!TagManager.ProperNames.ContainsKey(result))
            {
                Helpers.StringsTagShort(traitVal.id, traitVal.id, (Db.Get().traits?.TryGet(traitVal.id))?.Name);
            }
            return result;
        }

        public static string ConvertAptitude(HashedString hashed)
        {
            foreach (var aptitude in Db.Get().SkillGroups.resources)
            {
                if (aptitude.Id == hashed)
                    return aptitude.Id;
            }
            return null;
        }

        public static bool Prefix(ref Dictionary<Tag, HashSet<Tag>> __result, Filterable __instance)
        {
            if (GoodTraits == null) Init();

            if (__instance.filterElementState == Filterable.ElementState.None && __instance.gameObject.PrefabID() == "ResetSkillsStation")
            {
                __result = new Dictionary<Tag, HashSet<Tag>>();

                var minion = __instance.gameObject.GetComponent<Assignable>()?.assignee?.GetSoleOwner()?.GetComponent<MinionAssignablesProxy>()?.GetTargetGameObject()?.GetComponent<MinionResume>();
                if (minion != null)
                {
                    var dupeTraits = new HashSet<Tag>(minion.gameObject.GetComponent<Traits>().GetTraitIds().Select(x => (Tag)x));
                    var dupeAptitudes = minion.AptitudeBySkillGroup.Select(x => (Tag)ConvertAptitude(x.Key));

                    __result["Bad Traits"] = BadTraits.RemoveRange(dupeTraits);
                    if (minion.HasEnoughExp(SkillStationCosts.CostRemoveTrait))
                        __result["Remove Traits"] = dupeTraits;
                    if (minion.HasEnoughExp(SkillStationCosts.CostAddTrait))
                    {
                        __result["Good Traits"] = GoodTraits.RemoveRange(dupeTraits);
                        __result["Gene Traits"] = GeneTraits.RemoveRange(dupeTraits);
                        __result["Congenital Traits"] = CongenitalTraits.RemoveRange(dupeTraits);
                        __result["Stress Traits"] = StressTraits.RemoveRange(dupeTraits);
                        __result["Joy Traits"] = JoyTraits.RemoveRange(dupeTraits);
                    }
                    if (minion.HasEnoughExp(SkillStationCosts.CostAddAptitude))
                        __result["Aptitudes"] = Aptitudes.RemoveRange(dupeAptitudes);
                    if (minion.HasEnoughExp(SkillStationCosts.CostAddAttribute))
                        __result["Attributes"] = Attributes;
#if false
                    Debug.Log("[SkillStation] Good Traits: " + DUPLICANTSTATS.GOODTRAITS.Select(x => x.id).Join());
                    Debug.Log("[SkillStation] Bad Traits: " + DUPLICANTSTATS.BADTRAITS.Select(x => x.id).Join());
                    Debug.Log("[SkillStation] Gene Traits: " + DUPLICANTSTATS.GENESHUFFLERTRAITS.Select(x => x.id).Join());
                    Debug.Log("[SkillStation] Congenital Traits: " + DUPLICANTSTATS.CONGENITALTRAITS.Select(x => x.id).Join());
                    Debug.Log("[SkillStation] Stress Traits: " + DUPLICANTSTATS.STRESSTRAITS.Select(x => x.id).Join());
                    Debug.Log("[SkillStation] Joy Traits: " + DUPLICANTSTATS.JOYTRAITS.Select(x => x.id).Join());
                    Debug.Log("[SkillStation] Aptitudes: " + Db.Get().SkillGroups.resources.Select(x => x.Id).Join());
                    Debug.Log("[SkillStation] Attributes: " + DUPLICANTSTATS.ALL_ATTRIBUTES.Select(x => (Tag)("Attribute " + x)).Join());

                    var dupeAttributes = (List<AttributeLevel>)attributes_levels.GetValue(minion.gameObject.GetComponent<AttributeLevels>());
                    Debug.Log("[SkillStation] Dupe traits: " + dupeTraits.Join());
                    Debug.Log("[SkillStation] Dupe attributes: " + dupeAttributes.Select(x => x.attribute.Attribute.Id + ":" + x.level).Join());
                    Debug.Log("[SkillStation] Dupe aptitudes: " + dupeAptitudes.Join());
#endif
                }
#if DEBUG
                __result["-All Traits"] = new HashSet<Tag>(Db.Get().traits.resources.Select(x => (Tag)x.Id));
#endif
                __result["Void"] = new HashSet<Tag>() { "Void" };
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(ResetSkillsStation), nameof(ResetSkillsStation.OnCompleteWork))]
    public class SkillStation_TaskComplete_Patch
    {
        public static bool Prepare()
        {
            return SkillStationCosts.IsEnabled;
        }

        public static bool Prefix(WorkerBase worker, ResetSkillsStation __instance)
        {
            // get basic components
            var minion = worker.GetComponent<MinionResume>();
            var filter = __instance.GetComponent<Filterable>();
            if (minion == null || filter == null)
            {
                Debug.Log("[SkillStation] Minion or Filter is null");
                return true;
            }

            // print debug message
            Debug.Log($"[SkillStation] Dupe={minion.GetProperName()} Exp={minion.TotalExperienceGained} Points={minion.TotalSkillPointsGained}");

            // remember assignee, in case we want to reassign it
            var assignee = __instance.assignable.assignee;
            __instance.assignable.Unassign();

            // check which purchase was selected
            Trait trait = Db.Get().traits.TryGet(filter.SelectedTag.Name);
            SkillType type;
            if (!filter.SelectedTag.IsValid || filter.SelectedTag == (Tag)"Void")
                type = SkillType.Reset;
            else if (trait != null && minion.gameObject.GetComponent<Traits>().HasTrait(trait))
                type = SkillType.RemoveTrait;
            else if (trait != null && SkillStation_Options_Patch.BadTraits.Contains(trait.Id))
                type = SkillType.BadTrait;
            else if (trait != null)
                type = SkillType.GoodTrait;
            else if (filter.SelectedTag.Name.StartsWith("Attribute ", StringComparison.Ordinal))
                type = SkillType.Attribute;
            else if (Db.Get().SkillGroups.resources.Any(s => s.Id == filter.SelectedTag.Name))
                type = SkillType.Aptitude;
            else
                type = SkillType.Error;

            // check if exp are sufficient
            float exp_cost = type switch
            {
                SkillType.Reset => SkillStationCosts.CostReset,
                SkillType.RemoveTrait => SkillStationCosts.CostRemoveTrait,
                SkillType.GoodTrait => SkillStationCosts.CostAddTrait,
                SkillType.BadTrait => SkillStationCosts.CostBadTrait,
                SkillType.Attribute => SkillStationCosts.CostAddAttribute,
                SkillType.Aptitude => SkillStationCosts.CostAddAptitude,
                _ => 0f
            };
            if (minion.TotalExperienceGained < exp_cost)
                type = SkillType.MissingExp;

            // execute
            string tooltip;
            switch (type)
            {
                default:
                    Debug.Log("[SkillStation] Wups, no Tag match.");
                    throw new Exception("SkillType.Error");

                case SkillType.MissingExp:
                    tooltip = string.Format(Strings.Get("CustomizeBuildings.LOCSTRINGS.SkillStationFailure"), minion.GetProperName());
                    break;

                case SkillType.Reset:
                    Debug.Log("[SkillStation] Tag execute: Void");
                    minion.ResetSkillLevels(true);
                    minion.SetHats(minion.CurrentHat, null);
                    minion.ApplyTargetHat();
                    minion.AddExperience(-SkillStationCosts.CostReset);
                    tooltip = string.Format(Strings.Get("CustomizeBuildings.LOCSTRINGS.SkillStationReset"), minion.GetProperName());
                    break;

                case SkillType.RemoveTrait:
                    Debug.Log("[SkillStation] Tag execute: " + filter.SelectedTag);
                    minion.gameObject.GetComponent<Traits>().Remove(trait);
                    minion.AddExperience(-SkillStationCosts.CostRemoveTrait);
                    tooltip = string.Format(Strings.Get("CustomizeBuildings.LOCSTRINGS.SkillStationRemoveTrait"), minion.GetProperName(), filter.SelectedTag.ProperName());
                    break;

                case SkillType.GoodTrait:
                    Debug.Log("[SkillStation] Tag execute: " + filter.SelectedTag);
                    minion.gameObject.GetComponent<Traits>().Add(trait);
                    minion.AddExperience(-SkillStationCosts.CostAddTrait);
                    tooltip = string.Format(Strings.Get("CustomizeBuildings.LOCSTRINGS.SkillStationAddTrait"), minion.GetProperName(), filter.SelectedTag.ProperName());
                    break;

                case SkillType.BadTrait:
                    Debug.Log("[SkillStation] Tag execute: " + filter.SelectedTag);
                    minion.gameObject.GetComponent<Traits>().Add(trait);
                    minion.AddExperience(-SkillStationCosts.CostBadTrait);
                    tooltip = string.Format(Strings.Get("CustomizeBuildings.LOCSTRINGS.SkillStationAddTrait"), minion.GetProperName(), filter.SelectedTag.ProperName());
                    break;

                case SkillType.Attribute:
                    Debug.Log("[SkillStation] Tag execute: " + filter.SelectedTag);
                    string attributeId = filter.SelectedTag.Name.Substring(10);
                    var attributes = minion.gameObject.GetComponent<AttributeLevels>();
                    var attribute = attributes.GetAttributeLevel(attributeId);
                    if (attribute != null)
                    {
                        int level = attribute.GetLevel();
                        int levelplus = CustomizeBuildingsState.StateManager.State.SkillStationAttributeStep;
                        attribute.SetLevel(level + levelplus);
                        attribute.Apply(attributes);
                        minion.AddExperience(-SkillStationCosts.CostAddAttribute);
                        tooltip = string.Format(Strings.Get("CustomizeBuildings.LOCSTRINGS.SkillStationAttributeUp"), minion.GetProperName(), filter.SelectedTag.ProperName(), level, level + levelplus);
                    }
                    else
                    {
                        tooltip = "Wups, critical error: Attribute could not be resolved.";
                    }
                    break;

                case SkillType.Aptitude:
                    Debug.Log("[SkillStation] Tag execute: " + filter.SelectedTag);
                    //minion.AptitudeBySkillGroup[filter.SelectedTag.Name] = 1f;
                    minion.SetAptitude(filter.SelectedTag.Name, 1f);
                    minion.AddExperience(-SkillStationCosts.CostAddAptitude);
                    //tooltip = $"{minion.GetProperName()} sparked new Interests in {filter.SelectedTag.ProperName()}";   //SkillStationAptitude
                    tooltip = string.Format(Strings.Get("CustomizeBuildings.LOCSTRINGS.SkillStationAptitude"), minion.GetProperName(), filter.SelectedTag.ProperName());
                    break;
            }

            // cleanup
            if (minion.TotalExperienceGained < 0)
            {
                Debug.Log("[SkillStation] Warning: Minion had negative exp.");
                minion.AddExperience(-minion.TotalExperienceGained);
            }
            if (minion.AvailableSkillpoints < 0)
            {
                Debug.Log("[SkillStation] Reset skills, because of negative Skill Point count.");
                minion.ResetSkillLevels(true);
                minion.SetHats(minion.CurrentHat, null);
                minion.ApplyTargetHat();
            }

            // re-apply job, if attribute selected and exp and skill points sufficient
            if (type == SkillType.Attribute)
            {
                if (minion.HasEnoughExp(SkillStationCosts.CostAddAttribute))
                {
                    __instance.assignable.Assign(assignee);
                }
            }
            else
            {
                filter.SelectedTag = "Void";
            }

            if (__instance.GetComponent<KSelectable>().IsSelected)
                DetailsScreen.Instance.Refresh(__instance.gameObject);

            // display notification
            Debug.Log("[SkillStation] " + tooltip);
            worker.GetComponent<Notifier>().Add(new Notification(
                title: "Skills Station",
                type: NotificationType.Good,
                tooltip: (List<Notification> notificationList, object data) =>
                {
                    string text = null;
                    foreach (var notification in notificationList)
                        text = text + (string)notification.tooltipData + "\n";
                    return text;
                },
                tooltip_data: tooltip,
                expires: true,
                delay: 0f,
                custom_click_callback: null,
                custom_click_data: null,
                click_focus: null,
                volume_attenuation: true),
                suffix: "");

            return false;
        }

        private enum SkillType
        {
            Error,
            MissingExp,
            Reset,
            RemoveTrait,
            GoodTrait,
            BadTrait,
            Attribute,
            Aptitude,
        }
    }


    [HarmonyPatch(typeof(FilterSideScreen), nameof(FilterSideScreen.IsValidForTarget))]
    public class SkillStation_Sidescreen_Patch
    {
        public static bool Prepare()
        {
            return SkillStationCosts.IsEnabled;
        }

        public static void Postfix(GameObject target, ref bool __result, FilterSideScreen __instance)
        {
            if (!__result)
            {
                var filter = target.GetComponent<Filterable>();
                __result = (!__instance.isLogicFilter && filter && filter.filterElementState == Filterable.ElementState.None);
                //Debug.Log($"[SkillStation] IsValidForTarget: {__result} : {filter} : {filter?.filterElementState}");
            }
        }
    }

    [HarmonyPatch(typeof(ResetSkillsStationConfig), nameof(ResetSkillsStationConfig.ConfigureBuildingTemplate))]
    public class SkillStation_AddFilter_Patch
    {
        public static bool Prepare()
        {
            return SkillStationCosts.IsEnabled;
        }

        public static void Postfix(GameObject go)
        {
            go.AddOrGet<ResetSkillsStation>().workTime = SkillStationCosts.CostTime;
            go.AddOrGet<Filterable>().filterElementState = Filterable.ElementState.None;
        }
    }

    [HarmonyPatch(typeof(ResetSkillsStation), nameof(ResetSkillsStation.OnAssign))]
    public class SkillStation_RefreshUI_Patch
    {
        public static bool Prepare()
        {
            return SkillStationCosts.IsEnabled;
        }

        public static void Postfix(ResetSkillsStation __instance)
        {
            try
            {
                //__instance.GetComponent<Filterable>().SelectedTag = "Void";
                if (__instance.GetComponent<KSelectable>().IsSelected)
                    DetailsScreen.Instance.Refresh(__instance.gameObject);
                Helpers.PrintDebug("SkillStation OnAssign Refresh");
            } catch (System.Exception)
            {
                Helpers.PrintDebug("SkillStation OnAssign wups");
            }
        }
    }

    [HarmonyPatch(typeof(SingleItemSelectionSideScreenBase), nameof(SingleItemSelectionSideScreenBase.RecycleItemRow))]
    public static class SkillStation_FixCrash1
    {
        public static bool Prepare()
        {
            return SkillStationCosts.IsEnabled;
        }

        public static bool Prefix(SingleItemSelectionRow row, SingleItemSelectionSideScreenBase __instance)
        {
            // the original method recycles the item and throws if an item has duplicate tag; thus throwing if two items have the same tag
            // this method deletes the item; I believe doing otherwise will end in a memory leak (are unity objects garbage collected?)

            try
            {
                if (__instance.CurrentSelectedItem == row)
                    __instance.SetSelectedItem((SingleItemSelectionRow)null);
                row.Clicked = null;
                row.SetSelected(selected: false);
                row.transform.SetParent(__instance.original_ItemRow.transform.parent.parent);
                row.gameObject.SetActive(value: false);

                if (__instance.pooledRows.ContainsKey(row.tag))
                    UnityEngine.Object.Destroy(row);
                else
                    __instance.pooledRows[row.tag] = row;
            } catch (Exception ex)
            {
                Helpers.Print(ex.ToString());
            }

            return false;
        }
    }
}
