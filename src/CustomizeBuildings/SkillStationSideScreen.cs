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
        //public static FieldInfo _Attributes_levels;

        public static void Init()
        {
#if LOCALE
            _ProperNames = (Dictionary<Tag, string>)AccessTools.Field(typeof(TagManager), "ProperNames").GetValue(null);
#endif
            //_Attributes_levels = AccessTools.Field(typeof(AttributeLevels), "levels");
            GoodTraits = new HashSet<Tag>(DUPLICANTSTATS.GOODTRAITS.Select(x => GetTag(x.id)));
            BadTraits = new HashSet<Tag>(DUPLICANTSTATS.BADTRAITS.Select(x => GetTag(x.id)));
            GeneTraits = new HashSet<Tag>(DUPLICANTSTATS.GENESHUFFLERTRAITS.Select(x => GetTag(x.id)));
            CongenitalTraits = new HashSet<Tag>(DUPLICANTSTATS.CONGENITALTRAITS.Select(x => GetTag(x.id)));
            StressTraits = new HashSet<Tag>(DUPLICANTSTATS.STRESSTRAITS.Select(x => GetTag(x.id)));
            JoyTraits = new HashSet<Tag>(DUPLICANTSTATS.JOYTRAITS.Select(x => GetTag(x.id)));
            Aptitudes = new HashSet<Tag>(Db.Get().SkillGroups.resources.Select(x => GetTag(x.Id)));
            Attributes = new HashSet<Tag>(DUPLICANTSTATS.ALL_ATTRIBUTES.Select(x => GetTag("Attribute " + x)));
#if LOCALE
            Helpers.StringsTag("CustomizeBuildings.TAG.Bad_Traits", "Bad Traits");
            Helpers.StringsTag("CustomizeBuildings.TAG.Remove_Traits", "Remove Traits");
            Helpers.StringsTag("CustomizeBuildings.TAG.Good_Traits", "Good Traits");
            Helpers.StringsTag("CustomizeBuildings.TAG.Gene_Traits", "Gene Traits");
            Helpers.StringsTag("CustomizeBuildings.TAG.Congenital_Traits", "Congenital Traits");
            Helpers.StringsTag("CustomizeBuildings.TAG.Stress_Traits", "Stress Traits");
            Helpers.StringsTag("CustomizeBuildings.TAG.Joy_Traits", "Joy Traits");
            Helpers.StringsTag("CustomizeBuildings.TAG.Aptitudes", "Aptitudes");
            Helpers.StringsTag("CustomizeBuildings.TAG.Attributes", "Attributes");
            Helpers.StringsPrint();
#endif
        }

#if LOCALE
        private static Dictionary<Tag, string> _ProperNames;
        private static Tag GetTag(string id)
        {
            Tag result = (Tag)id;
            if (!_ProperNames.ContainsKey(result))
            {
                Helpers.StringsTag("CustomizeBuildings.TAG." + id, id);
            }
            return result;
        }
#else
        private static Tag GetTag(string id) => (Tag)id;
#endif

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
                    float exp = minion.TotalExperienceGained;

                    __result["Bad Traits"] = BadTraits.RemoveRange(dupeTraits);
                    if (exp >= SkillStationCosts.CostRemoveTrait)
                        __result["Remove Traits"] = dupeTraits;
                    if (exp >= SkillStationCosts.CostAddTrait)
                    {
                        __result["Good Traits"] = GoodTraits.RemoveRange(dupeTraits);
                        __result["Gene Traits"] = GeneTraits.RemoveRange(dupeTraits);
                        __result["Congenital Traits"] = CongenitalTraits.RemoveRange(dupeTraits);
                        __result["Stress Traits"] = StressTraits.RemoveRange(dupeTraits);
                        __result["Joy Traits"] = JoyTraits.RemoveRange(dupeTraits);
                    }
                    if (exp >= SkillStationCosts.CostAddAptitude)
                        __result["Aptitudes"] = Aptitudes.RemoveRange(dupeAptitudes);
                    if (exp >= SkillStationCosts.CostAddAttribute)
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

    [HarmonyPatch(typeof(ResetSkillsStation), "OnCompleteWork")]
    public class SkillStation_TaskComplete_Patch
    {
        public static bool Prepare()
        {
            return SkillStationCosts.IsEnabled;
        }

        public static bool Prefix(Worker worker, ResetSkillsStation __instance)
        {
            // get basic components
            var minion = worker.GetComponent<MinionResume>();
            var filter = __instance.GetComponent<Filterable>();
            if (minion == null || filter == null)
            {
                Debug.Log("[SkillStation] Minion or Filter is null");
                return false;
            }

            // print debug message
            Debug.Log($"[SkillStation] Dupe={minion.GetProperName()} Exp={minion.TotalExperienceGained} Points={minion.TotalSkillPointsGained}");
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
            switch (type)
            {
                case SkillType.Reset:
                    if (minion.TotalExperienceGained < SkillStationCosts.CostReset)
                        type = SkillType.MissingExp;
                    break;
                case SkillType.RemoveTrait:
                    if (minion.TotalExperienceGained < SkillStationCosts.CostRemoveTrait)
                        type = SkillType.MissingExp;
                    break;
                case SkillType.GoodTrait:
                    if (minion.TotalExperienceGained < SkillStationCosts.CostAddTrait)
                        type = SkillType.MissingExp;
                    break;
                case SkillType.BadTrait:
                    if (minion.TotalExperienceGained < SkillStationCosts.CostBadTrait)
                        type = SkillType.MissingExp;
                    break;
                case SkillType.Attribute:
                    if (minion.TotalExperienceGained < SkillStationCosts.CostAddAttribute)
                        type = SkillType.MissingExp;
                    break;
                case SkillType.Aptitude:
                    if (minion.TotalExperienceGained < SkillStationCosts.CostAddAptitude)
                        type = SkillType.MissingExp;
                    break;
            }

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
                    tooltip = string.Format(Strings.Get("CustomizeBuildings.LOCSTRINGS.SkillStationRemoveTrait"), minion.GetProperName(), trait.Name);
                    break;

                case SkillType.GoodTrait:
                    Debug.Log("[SkillStation] Tag execute: " + filter.SelectedTag);
                    minion.gameObject.GetComponent<Traits>().Add(trait);
                    minion.AddExperience(-SkillStationCosts.CostAddTrait);
                    tooltip = string.Format(Strings.Get("CustomizeBuildings.LOCSTRINGS.SkillStationAddTrait"), minion.GetProperName(), trait.Name);
                    break;

                case SkillType.BadTrait:
                    Debug.Log("[SkillStation] Tag execute: " + filter.SelectedTag);
                    minion.gameObject.GetComponent<Traits>().Add(trait);
                    minion.AddExperience(-SkillStationCosts.CostBadTrait);
                    tooltip = string.Format(Strings.Get("CustomizeBuildings.LOCSTRINGS.SkillStationAddTrait"), minion.GetProperName(), trait.Name);
                    break;

                case SkillType.Attribute:
                    Debug.Log("[SkillStation] Tag execute: " + filter.SelectedTag);
                    string attributeId = filter.SelectedTag.Name.Substring(10);
                    var attribute = minion.gameObject.GetComponent<AttributeLevels>().GetAttributeLevel(attributeId);
                    if (attribute != null)
                    {
                        int level = attribute.GetLevel();
                        attribute.SetLevel(level + 1);
                        minion.AddExperience(-SkillStationCosts.CostAddAttribute);
                        tooltip = string.Format(Strings.Get("CustomizeBuildings.LOCSTRINGS.SkillStationAttributeUp"), minion.GetProperName(), attributeId, level, level + 1);
                    }
                    else
                    {
                        tooltip = "Wups, cricital error: Attribute could not be resolved.";
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
            filter.SelectedTag = "Void";
            DetailsScreen.Instance.Refresh(__instance.gameObject);

            // display notification
            Debug.Log("[SkillStation] " + tooltip);
            worker.GetComponent<Notifier>().Add(new Notification(
#if !DLC1
                group: HashedString.Invalid,
#endif
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

            // skip base method
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

    [HarmonyPatch(typeof(ResetSkillsStation), "OnAssign")]
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
                DetailsScreen.Instance.Refresh(__instance.gameObject);
                Helpers.PrintDebug("SkillStation OnAssign Refresh");
            }
            catch (System.Exception)
            {
                Helpers.PrintDebug("SkillStation OnAssign wups");
            }
        }
    }
}