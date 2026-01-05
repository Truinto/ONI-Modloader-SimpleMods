using Common;
using HarmonyLib;
using KSerialization;
using Shared;
using Shared.CollectionNS;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CustomizeGeyser
{
    /// <summary>
    /// Save data, to remember which tuning has been selected.
    /// </summary>
    [SerializationConfig(MemberSerialization.OptIn)]
    public class GeoTunerSelectMod : KMonoBehaviour
    {
        [Serialize] public string? Id;
        [Serialize] public string? FutureId;
    }

    [HarmonyPatch]
    public static class GeoTunerPatch
    {
        public static bool Prepare() => CustomizeGeyserState.StateManager.State.TuningEnabled;

        [HarmonyPatch(typeof(GeoTunerConfig), nameof(GeoTunerConfig.ConfigureBuildingTemplate))]
        [HarmonyPostfix]
        public static void Patch1(GameObject go)
        {
            go.AddOrGet<GeoTunerSelectMod>();
        }

        /// <summary>
        /// GeoTuner usually only point to a geyser and the modification is defined by the type of geyser.
        /// To have multiple different modifications, we store an ID string in a Component.
        /// Unfortunately, the game checks for this in GeoTuner.Def, which cannot store Components and has no reference to the GeoTuner GameObject.
        /// Saving the Component in 'geyser' is also not an option, since it can receive different modifications from multiple GeoTuners.
        /// 
        /// Calls to GetSettingsForGeyser must be redirected in a way that GeoTuner instance is accessible.
        /// </summary>
        [HarmonyPatch(typeof(GeoTuner), nameof(GeoTuner.RefreshStorageRequirements))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Patch2(IEnumerable<CodeInstruction> code, ILGenerator generator, MethodBase original)
        {
            var tool = new TranspilerTool(code, generator, original);
            tool.ReplaceAllCalls(typeof(GeoTuner.Def), nameof(GeoTuner.Def.GetSettingsForGeyser), patch);
            return tool;

            static GeoTunerConfig.GeotunedGeyserSettings patch(GeoTuner.Def instance, Geyser geyser, GeoTuner.Instance smi)
            {
                return GetSettingsForGeyser(smi.GetComponent<GeoTunerSelectMod>()?.Id, geyser);
            }
        }

        [HarmonyPatch(typeof(GeoTuner), nameof(GeoTuner.DropStorageIfNotMatching))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Patch3(IEnumerable<CodeInstruction> code, ILGenerator generator, MethodBase original) => Patch2(code, generator, original);

        [HarmonyPatch(typeof(GeoTuner), nameof(GeoTuner.ResetExpirationTimer))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Patch4(IEnumerable<CodeInstruction> code, ILGenerator generator, MethodBase original) => Patch2(code, generator, original);

        [HarmonyPatch(typeof(GeoTuner.Instance), nameof(GeoTuner.Instance.RefreshModification))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Patch5(IEnumerable<CodeInstruction> code, ILGenerator generator, MethodBase original)
        {
            var tool = new TranspilerTool(code, generator, original);
            tool.ReplaceAllCalls(typeof(GeoTuner.Def), nameof(GeoTuner.Def.GetSettingsForGeyser), patch);
            return tool;

            static GeoTunerConfig.GeotunedGeyserSettings patch(GeoTuner.Def instance, Geyser geyser, GeoTuner.Instance __instance)
            {
                return GetSettingsForGeyser(__instance.GetComponent<GeoTunerSelectMod>()?.Id, geyser);
            }
        }

        public static GeoTunerConfig.GeotunedGeyserSettings GetSettingsForGeyser(string? id, Geyser geyser)
        {
            int idHash = Hash.SDBMLower(id);
            if (CustomizeGeyserState.StateManager.State.Tunings.TryGet(f => f.idHash == idHash && f.IsValidForGeyser(geyser), out var tuning))
                return tuning;
            if (GeoTunerConfig.geotunerGeyserSettings.TryGetValue(geyser.configuration.typeId, out var value))
                return value;
            return GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.DEFAULT_CATEGORY];
        }

        public static bool Equals(GeoTunerConfig.GeotunedGeyserSettings s1, GeoTunerConfig.GeotunedGeyserSettings s2)
        {
            if (s1.material != s2.material) return false;
            if (s1.quantity != s2.quantity) return false;
            if (s1.duration != s2.duration) return false;
            //if (s1.template.originID != s2.template.originID) return false;
            if (s1.template.massPerCycleModifier != s2.template.massPerCycleModifier) return false;
            if (s1.template.temperatureModifier != s2.template.temperatureModifier) return false;
            if (s1.template.iterationDurationModifier != s2.template.iterationDurationModifier) return false;
            if (s1.template.iterationPercentageModifier != s2.template.iterationPercentageModifier) return false;
            if (s1.template.yearDurationModifier != s2.template.yearDurationModifier) return false;
            if (s1.template.yearPercentageModifier != s2.template.yearPercentageModifier) return false;
            if (s1.template.maxPressureModifier != s2.template.maxPressureModifier) return false;
            if (s1.template.modifyElement != s2.template.modifyElement) return false;
            if (s1.template.newElement != s2.template.newElement) return false;
            return true;
        }


        /// <summary>
        /// Need to add new rows for custom tuning options. It will be stored in GeoTunerSelectMod.
        /// </summary>
        [HarmonyPatch(typeof(GeoTunerSideScreen), nameof(GeoTunerSideScreen.RefreshOptions))]
        [HarmonyPrefix]
        public static bool Patch6(GeoTunerSideScreen __instance)
        {
            int num = 0;
            SetRow(__instance, num++, UI.UISIDESCREENS.GEOTUNERSIDESCREEN.NOTHING, Assets.GetSprite("action_building_disabled"), null, studied: true);
            var geysers = Components.Geysers.GetItems(__instance.targetGeotuner.GetMyWorldId());
            foreach (var geyser in geysers)
            {
                if (geyser.GetComponent<Studyable>().Studied)
                {
                    bool any = false;
                    foreach (var tuning in CustomizeGeyserState.StateManager.State.Tunings)
                    {
                        if (tuning.IsValidForGeyser(geyser))
                        {
                            any = true;
                            string displayText;
                            if (tuning.name == null)
                                displayText = $"{UI.StripLinkFormatting(geyser.GetProperName())} {tuning.id}";
                            else if (tuning.name.StartsWith("CustomizeGeyser.", StringComparison.Ordinal))
                                displayText = string.Format(Strings.Get(tuning.name), UI.StripLinkFormatting(geyser.GetProperName()));
                            else
                                displayText = $"{UI.StripLinkFormatting(geyser.GetProperName())} {tuning.name}";
                            SetRow(__instance, num++, displayText, Def.GetUISprite(geyser.gameObject).first, geyser, studied: true, tuning);
                        }
                    }
                    if (!any)
                        SetRow(__instance, num++, UI.StripLinkFormatting(geyser.GetProperName()), Def.GetUISprite(geyser.gameObject).first, geyser, studied: true);
                }
            }
            foreach (var geyser in geysers)
            {
                if (!geyser.GetComponent<Studyable>().Studied && Grid.Visible[Grid.PosToCell(geyser)] > 0 && geyser.GetComponent<Uncoverable>().IsUncovered)
                {
                    SetRow(__instance, num++, UI.StripLinkFormatting(geyser.GetProperName()), Def.GetUISprite(geyser.gameObject).first, geyser, studied: false);
                }
            }
            for (int i = num; i < __instance.rowContainer.childCount; i++)
            {
                __instance.rowContainer.GetChild(i).gameObject.SetActive(false);
            }
            return false;
        }

        /// <summary>
        /// This is copied from <see cref="GeoTunerSideScreen.SetRow"/>. GeoTunerStruct is a new argument to hold the optional tuning modifiers.
        /// Following functions are changed:
        /// - check FutureId to correctly highlight the button
        /// - onClick sets FutureId to the tuning modifier (of this row)
        /// </summary>
        private static void SetRow(GeoTunerSideScreen @this, int idx, string name, Sprite icon, Geyser? geyser, bool studied, GeoTunerStruct? tuning = null)
        {
            // check if button should be displayed as pressed
            var selection = @this.targetGeotuner.GetComponent<GeoTunerSelectMod>();
            bool buttonPressed = @this.targetGeotuner.GetFutureGeyser() == geyser && selection?.FutureId == tuning?.id;

            // simple variables
            bool optionClear = geyser == null;
            bool usingStudiedTooltip = geyser != null && studied;
            int countFuture = Components.GeoTuners.GetItems(@this.targetGeotuner.GetMyWorldId()).Count(f => f.GetFutureGeyser() == geyser);
            int countAssignedAndFuture = Components.GeoTuners.GetItems(@this.targetGeotuner.GetMyWorldId()).Count(f => f.GetAssignedGeyser() == geyser || f.GetFutureGeyser() == geyser);

            // get objects
            var go = (idx >= @this.rowContainer.childCount) ? Util.KInstantiateUI(@this.rowPrefab, @this.rowContainer.gameObject, force_active: true) : @this.rowContainer.GetChild(idx).gameObject;
            var hierarchy = go.GetComponent<HierarchyReferences>();

            // primary label
            var label1 = hierarchy.GetReference<LocText>("label");
            label1.text = name;
            label1.textStyleSetting = (studied || optionClear) ? @this.AnalyzedTextStyle : @this.UnanalyzedTextStyle;
            label1.ApplySettings();

            // icon
            var image = hierarchy.GetReference<Image>("icon");
            image.sprite = icon;
            image.color = (studied ? Color.white : new Color(0f, 0f, 0f, 0.5f));
            if (optionClear)
                image.color = Color.black;

            // primary tooltip (modificator info)
            var tooltips = go.GetComponentsInChildren<ToolTip>();
            var tooltip = tooltips.First();
            tooltip.SetSimpleTooltip(usingStudiedTooltip ? UI.UISIDESCREENS.GEOTUNERSIDESCREEN.STUDIED_TOOLTIP.ToString() : UI.UISIDESCREENS.GEOTUNERSIDESCREEN.UNSTUDIED_TOOLTIP.ToString());
            tooltip.enabled = geyser != null;
            tooltip.OnToolTip = delegate
            {
                if (geyser == null || !usingStudiedTooltip)
                    return UI.UISIDESCREENS.GEOTUNERSIDESCREEN.UNSTUDIED_TOOLTIP.ToString();
                if (geyser != @this.targetGeotuner.GetFutureGeyser() && countAssignedAndFuture >= 5)
                    return UI.UISIDESCREENS.GEOTUNERSIDESCREEN.GEOTUNER_LIMIT_TOOLTIP.ToString();

                // generate side screen info
                var settingsForGeyser = tuning ?? GetSettingsForGeyser(null, geyser);
                var sb = GlobalStringBuilderPool.Alloc();
                sb.Append(UI.UISIDESCREENS.GEOTUNERSIDESCREEN.STUDIED_TOOLTIP_MATERIAL.Replace("{MATERIAL}", settingsForGeyser.material.ProperName()));
                sb.Append($" {settingsForGeyser.quantity} kg");
                sb.Append("\n\n");

                const string formatFloat = "+0.###%;-0.###%;0";

                if (settingsForGeyser.template.massPerCycleModifier != 0f)
                {
                    sb.Append(string.Format(Strings.Get("CustomizeGeyser.LOCSTRINGS.MassPerCycleModifier"),
                        settingsForGeyser.template.massPerCycleModifier.ToString(formatFloat))); // Geyser.massModificationMethod
                    sb.Append('\n');
                }
                if (settingsForGeyser.template.temperatureModifier != 0f)
                {
                    float temperature = settingsForGeyser.template.temperatureModifier;
                    sb.Append(string.Format(Strings.Get("CustomizeGeyser.LOCSTRINGS.TemperatureModifier"),
                        ((temperature > 0f) ? "+" : "") + GameUtil.GetFormattedTemperature(temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Relative)));
                    sb.Append('\n');
                }
                if (settingsForGeyser.template.iterationDurationModifier != 0f)
                {
                    sb.Append(string.Format(Strings.Get("CustomizeGeyser.LOCSTRINGS.IterationDurationModifier"),
                        settingsForGeyser.template.iterationDurationModifier.ToString(formatFloat)));
                    sb.Append('\n');
                }
                if (settingsForGeyser.template.iterationPercentageModifier != 0f)
                {
                    sb.Append(string.Format(Strings.Get("CustomizeGeyser.LOCSTRINGS.IterationPercentageModifier"),
                        settingsForGeyser.template.iterationPercentageModifier.ToString(formatFloat)));
                    sb.Append('\n');
                }
                if (settingsForGeyser.template.yearDurationModifier != 0f)
                {
                    sb.Append(string.Format(Strings.Get("CustomizeGeyser.LOCSTRINGS.YearDurationModifier"),
                        settingsForGeyser.template.yearDurationModifier.ToString(formatFloat)));
                    sb.Append('\n');
                }
                if (settingsForGeyser.template.yearPercentageModifier != 0f)
                {
                    sb.Append(string.Format(Strings.Get("CustomizeGeyser.LOCSTRINGS.YearPercentageModifier"),
                        settingsForGeyser.template.yearPercentageModifier.ToString(formatFloat)));
                    sb.Append('\n');
                }
                if (settingsForGeyser.template.maxPressureModifier != 0f)
                {
                    sb.Append(string.Format(Strings.Get("CustomizeGeyser.LOCSTRINGS.MaxPressureModifier"),
                        settingsForGeyser.template.maxPressureModifier.ToString(formatFloat)));
                    sb.Append('\n');
                }
                if (settingsForGeyser.template.modifyElement)
                {
                    sb.Append(string.Format(Strings.Get("CustomizeGeyser.LOCSTRINGS.NewElement"),
                        settingsForGeyser.template.newElement.ToElement().name));
                    sb.Append('\n');
                }

                sb.Append("\n\n");
                sb.Append(UI.UISIDESCREENS.GEOTUNERSIDESCREEN.STUDIED_TOOLTIP_VISIT_GEYSER);

                return GlobalStringBuilderPool.ReturnAndFree(sb);
            };

            // tooltip (awaiting new geotuners)
            if (usingStudiedTooltip && countFuture > 0)
            {
                var tooltip2 = tooltips.Last();
                tooltip2.SetSimpleTooltip("");
                tooltip2.OnToolTip = () => UI.UISIDESCREENS.GEOTUNERSIDESCREEN.STUDIED_TOOLTIP_NUMBER_HOVERED.ToString().Replace("{0}", countFuture.ToString());
            }

            // label amount
            var label2 = hierarchy.GetReference<LocText>("amount");
            label2.SetText(countFuture.ToString());
            label2.transform.parent.gameObject.SetActive(!optionClear && countFuture > 0);

            // button functionality
            var multiToggle = go.GetComponent<MultiToggle>();
            multiToggle.ChangeState(buttonPressed ? 1 : 0);
            multiToggle.onClick = delegate
            {
                // always allow clear
                if (geyser == null)
                {
                    @this.targetGeotuner.AssignFutureGeyser(null);
                    @this.RefreshOptions();
                    return;
                }

                // disabled when not studied
                if (!geyser.GetComponent<Studyable>().Studied)
                    return;

                // disabled when total count would be > 5
                if (geyser != @this.targetGeotuner.GetAssignedGeyser()) // only check for geyser count, if switching to a different geyser
                {
                    int num = Components.GeoTuners.GetItems(@this.targetGeotuner.GetMyWorldId()).Count(f => f.GetAssignedGeyser() == geyser || f.GetFutureGeyser() == geyser);
                    if (num >= 5)
                        return;
                }

                // set new future id ('selection' should never be null)
                if (selection != null)
                {
                    selection.FutureId = tuning?.id;
                }

                //if (geyser != @this.targetGeotuner.GetFutureGeyser()) // always assign in case the tuning modifier is different
                @this.targetGeotuner.AssignFutureGeyser(geyser);
                @this.RefreshOptions();
            };
            multiToggle.onDoubleClick = delegate
            {
                if (geyser != null)
                {
                    GameUtil.FocusCamera(geyser.transform.GetPosition());
                    return true;
                }
                return false;
            };
        }


        /// <summary>
        /// Set new assigned geyser and apply the correct tuning.
        /// AssignGeyser does not need to be patched, since it is only called by OnSwitchGeyserChoreCompleted.
        /// </summary>
        [HarmonyPatch(typeof(GeoTuner.Instance), nameof(GeoTuner.Instance.OnSwitchGeyserChoreCompleted))]
        [HarmonyPrefix]
        public static bool Patch7(Chore chore, GeoTuner.Instance __instance)
        {
            if (!chore.isComplete)
                goto exit;

            var assignedGeyser = __instance.GetAssignedGeyser();
            var futureGeyser = __instance.GetFutureGeyser();

            // set Id from FutureId
            var selection = __instance.GetComponent<GeoTunerSelectMod>();
            if (selection != null)
            {
                selection.Id = selection.FutureId;
                //selection.FutureId = null; // FutureId must not be cleared; this is checked for button highlighting; btw, same for FutureGeyser
            }

            // chose no modification, clear any existing modifiers, then exit
            if (futureGeyser == null)
            {
                // if already unset, do nothing
                if (assignedGeyser == null)
                    goto exit;

                // remove old geyser + modification
                assignedGeyser.RemoveModification(__instance.smi.currentGeyserModification);
                assignedGeyser.Unsubscribe((int)GameHashes.GeyserEruption, __instance.smi.OnEruptionStateChanged);
                __instance.sm.AssignedGeyser.Set(null, __instance);
                __instance.smi.currentGeyserModification = default;
                GeoTuner.RefreshStorageRequirements(__instance);
                GeoTuner.DropStorageIfNotMatching(__instance);

                // trigger events
                assignedGeyser.Trigger((int)GameHashes.GeotunerChange);
                __instance.sm.geyserSwitchSignal.Trigger(__instance);
                goto exit;
            }

            var futureId = GetSettingsForGeyser(selection?.FutureId, futureGeyser);

            if (assignedGeyser == futureGeyser)
            {
                // if geyser did not change,
                // replace geyser modification
                assignedGeyser.RemoveModification(__instance.smi.currentGeyserModification);
                __instance.sm.AssignedGeyser.Set(futureGeyser, __instance);
                __instance.smi.currentGeyserModification = futureId.template;
                __instance.smi.currentGeyserModification.originID = __instance.smi.originID;
                __instance.smi.enhancementDuration = futureId.duration;
                GeoTuner.RefreshStorageRequirements(__instance);
                GeoTuner.DropStorageIfNotMatching(__instance);

                // trigger events
                assignedGeyser.Trigger((int)GameHashes.GeotunerChange);
                __instance.sm.geyserSwitchSignal.Trigger(__instance);
            }
            else
            {
                // if geyser did change,
                // remove old geyser + modification
                if (assignedGeyser != null)
                {
                    assignedGeyser.RemoveModification(__instance.smi.currentGeyserModification);
                    assignedGeyser.Unsubscribe((int)GameHashes.GeyserEruption, __instance.smi.OnEruptionStateChanged);
                }

                // set new geyser + modification (normally happens in RefreshModification)
                __instance.sm.AssignedGeyser.Set(futureGeyser, __instance);
                __instance.smi.currentGeyserModification = futureId.template;
                __instance.smi.currentGeyserModification.originID = __instance.smi.originID;
                __instance.smi.enhancementDuration = futureId.duration;
                GeoTuner.RefreshStorageRequirements(__instance);
                GeoTuner.DropStorageIfNotMatching(__instance);

                // trigger events
                assignedGeyser?.Trigger((int)GameHashes.GeotunerChange);
                futureGeyser.Trigger((int)GameHashes.GeotunerChange);
                __instance.sm.geyserSwitchSignal.Trigger(__instance);
            }

        exit:
            __instance.Trigger((int)GameHashes.UIRefresh);
            return false;
        }


        /// <summary>
        /// This patch allows the switch chore to apply, even if the change happens on the same geyser.
        /// </summary>
        [HarmonyPatch(typeof(GeoTuner.Instance), nameof(GeoTuner.Instance.AssignFutureGeyser))]
        [HarmonyPrefix]
        public static bool Patch8(Geyser newFutureGeyser, GeoTuner.Instance __instance)
        {
            var assignedGeyser = __instance.GetAssignedGeyser();
            var selection = __instance.GetComponent<GeoTunerSelectMod>();
            var assignedId = selection?.Id;
            var futureId = selection?.FutureId;

            // set value
            __instance.sm.FutureGeyser.Set(newFutureGeyser, __instance);

            // stop chore, if target changed back to current state
            if (newFutureGeyser == assignedGeyser && assignedId == futureId)
            {
                __instance.AbortSwitchGeyserChore("Future Geyser was set to current Geyser");
            }
            // normally here would be a check if the geyser was changed at all, but we cannot tell, since the tuning can be changed on the same geyser
            // so we just always restart the chore (only noticable when using the CopySettings feature or repeatedly clicking the same option)
            else
            {
                __instance.RecreateSwitchGeyserChore();
            }
            return false;
        }

        [HarmonyPatch(typeof(Geyser), nameof(Geyser.GetDescriptors))]
        [HarmonyPrefix]
        public static bool Patch9(GameObject go, Geyser __instance, ref List<Descriptor> __result)
        {
            __result = new List<Descriptor>();
            //var sb = GlobalStringBuilderPool.Alloc();

            // geyser output per second
            //List<GeoTuner.Instance> items = Components.GeoTuners.GetItems(__instance.gameObject.GetMyWorldId());
            //GeoTuner.Instance instance = items.Find(f => f.GetAssignedGeyser() == __instance);
            //int num = items.Count(f => f.GetAssignedGeyser() == __instance);
            var element = ElementLoader.FindElementByHash(__instance.configuration.GetElement());
            string tooltip = string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_PRODUCTION, element.name, GameUtil.GetFormattedMass(__instance.configuration.GetEmitRate(), GameUtil.TimeSlice.PerSecond), GameUtil.GetFormattedTemperature(__instance.configuration.GetTemperature()));
            //if (num > 0)
            //{
            //    Func<float, float> obj = delegate (float emissionPerCycleModifier)
            //    {
            //        float num10 = 600f / __instance.configuration.GetIterationLength();
            //        return emissionPerCycleModifier / num10 / __instance.configuration.GetOnDuration();
            //    };
            //    int amountOfGeotunersAffectingThisGeyser = __instance.GetAmountOfGeotunersAffectingThisGeyser();
            //    float num2 = ((Geyser.temperatureModificationMethod == Geyser.ModificationMethod.Percentages) ? (instance.currentGeyserModification.temperatureModifier * __instance.configuration.geyserType.temperature) : instance.currentGeyserModification.temperatureModifier);
            //    float num3 = obj((Geyser.massModificationMethod == Geyser.ModificationMethod.Percentages) ? (instance.currentGeyserModification.massPerCycleModifier * __instance.configuration.scaledRate) : instance.currentGeyserModification.massPerCycleModifier);
            //    float num4 = amountOfGeotunersAffectingThisGeyser * num2;
            //    float num5 = amountOfGeotunersAffectingThisGeyser * num3;
            //    string arg2 = ((num4 > 0f) ? "+" : "") + GameUtil.GetFormattedTemperature(num4, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Relative);
            //    string arg3 = ((num5 > 0f) ? "+" : "") + GameUtil.GetFormattedMass(num5, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}");
            //    string text2 = ((num2 > 0f) ? "+" : "") + GameUtil.GetFormattedTemperature(num2, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Relative);
            //    string text3 = ((num3 > 0f) ? "+" : "") + GameUtil.GetFormattedMass(num3, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}");
            //    tooltip = string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_PRODUCTION_GEOTUNED, element.name, GameUtil.GetFormattedMass(__instance.configuration.GetEmitRate(), GameUtil.TimeSlice.PerSecond), GameUtil.GetFormattedTemperature(__instance.configuration.GetTemperature()));
            //    tooltip += "\n";
            //    tooltip = tooltip + "\n" + string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_PRODUCTION_GEOTUNED_COUNT, amountOfGeotunersAffectingThisGeyser.ToString(), num.ToString());
            //    tooltip += "\n";
            //    tooltip = tooltip + "\n" + string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_PRODUCTION_GEOTUNED_TOTAL, arg3, arg2);
            //    for (int num6 = 0; num6 < amountOfGeotunersAffectingThisGeyser; num6++)
            //    {
            //        string text4 = "\n    • " + UI.UISIDESCREENS.GEOTUNERSIDESCREEN.STUDIED_TOOLTIP_GEOTUNER_MODIFIER_ROW_TITLE.ToString();
            //        text4 = text4 + text3 + " " + text2;
            //        tooltip += text4;
            //    }
            //}
            __result.Add(new Descriptor(
                string.Format(UI.BUILDINGEFFECTS.GEYSER_PRODUCTION, element.tag.ProperName(), GameUtil.GetFormattedMass(__instance.configuration.GetEmitRate(), GameUtil.TimeSlice.PerSecond), GameUtil.GetFormattedTemperature(__instance.configuration.GetTemperature())),
                tooltip));

            // geyser disease (if any)
            if (__instance.configuration.GetDiseaseIdx() != byte.MaxValue)
                __result.Add(new Descriptor(
                    string.Format(UI.BUILDINGEFFECTS.GEYSER_DISEASE, GameUtil.GetFormattedDiseaseName(__instance.configuration.GetDiseaseIdx())),
                    string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_DISEASE, GameUtil.GetFormattedDiseaseName(__instance.configuration.GetDiseaseIdx()))));

            // geyser iteration
            __result.Add(new Descriptor(
                string.Format(UI.BUILDINGEFFECTS.GEYSER_PERIOD, GameUtil.GetFormattedTime(__instance.configuration.GetOnDuration()), GameUtil.GetFormattedTime(__instance.configuration.GetIterationLength())),
                string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_PERIOD, GameUtil.GetFormattedTime(__instance.configuration.GetOnDuration()), GameUtil.GetFormattedTime(__instance.configuration.GetIterationLength()))));


            if (__instance.GetComponent<Studyable>()?.Studied != true)
            {
                __result.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_YEAR_UNSTUDIED), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_UNSTUDIED)));
                __result.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.GEYSER_YEAR_AVR_OUTPUT_UNSTUDIED), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_AVR_OUTPUT_UNSTUDIED)));
            }
            else
            {
                // geyser year
                __result.Add(new Descriptor(
                    string.Format(UI.BUILDINGEFFECTS.GEYSER_YEAR_PERIOD, GameUtil.GetFormattedCycles(__instance.configuration.GetYearOnDuration()), GameUtil.GetFormattedCycles(__instance.configuration.GetYearLength())),
                    string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_PERIOD, GameUtil.GetFormattedCycles(__instance.configuration.GetYearOnDuration()), GameUtil.GetFormattedCycles(__instance.configuration.GetYearLength()))));

                // geyser dormancy
                if (__instance.smi.IsInsideState(__instance.smi.sm.dormant))
                    __result.Add(new Descriptor(
                        string.Format(UI.BUILDINGEFFECTS.GEYSER_YEAR_NEXT_ACTIVE, GameUtil.GetFormattedCycles(__instance.RemainingDormantTime())),
                        string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_NEXT_ACTIVE, GameUtil.GetFormattedCycles(__instance.RemainingDormantTime()))));
                else
                    __result.Add(new Descriptor(
                        string.Format(UI.BUILDINGEFFECTS.GEYSER_YEAR_NEXT_DORMANT, GameUtil.GetFormattedCycles(__instance.RemainingActiveTime())),
                        string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_NEXT_DORMANT, GameUtil.GetFormattedCycles(__instance.RemainingActiveTime()))));

                // geyser average
                tooltip = UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_AVR_OUTPUT.Replace("{average}", GameUtil.GetFormattedMass(__instance.configuration.GetAverageEmission(), GameUtil.TimeSlice.PerSecond)).Replace("{element}", __instance.configuration.geyserType.element.CreateTag().ProperName());
                //if (num > 0)
                //{
                //    text5 += "\n";
                //    text5 = text5 + "\n" + UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_AVR_OUTPUT_BREAKDOWN_TITLE;
                //    int amountOfGeotunersAffectingThisGeyser2 = __instance.GetAmountOfGeotunersAffectingThisGeyser();
                //    float num7 = ((Geyser.massModificationMethod == Geyser.ModificationMethod.Percentages) ? (instance.currentGeyserModification.massPerCycleModifier * 100f) : (instance.currentGeyserModification.massPerCycleModifier * 100f / __instance.configuration.scaledRate));
                //    float num8 = num7 * amountOfGeotunersAffectingThisGeyser2;
                //    text5 = text5 + GameUtil.AddPositiveSign(num8.ToString("0.0"), num8 > 0f) + "%";
                //    for (int num9 = 0; num9 < amountOfGeotunersAffectingThisGeyser2; num9++)
                //    {
                //        string text6 = "\n    • " + UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_AVR_OUTPUT_BREAKDOWN_ROW.ToString();
                //        text6 = text6 + GameUtil.AddPositiveSign(num7.ToString("0.0"), num7 > 0f) + "%";
                //        text5 += text6;
                //    }
                //}
                __result.Add(new Descriptor(
                    string.Format(UI.BUILDINGEFFECTS.GEYSER_YEAR_AVR_OUTPUT, GameUtil.GetFormattedMass(__instance.configuration.GetAverageEmission(), GameUtil.TimeSlice.PerSecond)),
                    tooltip));
            }

            //GlobalStringBuilderPool.Free(sb);
            return false;
        }

        /// <summary>
        /// Would crash if path is null. This seems to be fixed by now. I will leave this patch here anyway.
        /// </summary>
        [HarmonyPatch(typeof(FMODUnity.RuntimeManager), nameof(FMODUnity.RuntimeManager.PathToGUID))]
        [HarmonyPrefix]
        public static void Patch10(ref string path)
        {
            if (path == null)
            {
                Helpers.PrintDebug($"ERROR PathToGUID is null");
                path = "";
            }
        }
    }
}
