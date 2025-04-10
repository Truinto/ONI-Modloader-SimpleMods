﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Common;
using Database;
using HarmonyLib;
using Klei.AI;
using UnityEngine;
using static Common.Helpers;

namespace CustomizePlants
{
    [HarmonyPatch(typeof(MutantPlant), nameof(MutantPlant.ApplyMutations))]
    public class MutantPlant_AnalyzePatch
    {
        public static bool Prepare()
        {
            return CustomizePlantsState.StateManager.State.CheatMutationAnalyze;
        }

        public static void Postfix(MutantPlant __instance)
        {
            __instance.Analyze();
        }
    }

    [HarmonyPatch(typeof(PlantMutation), nameof(PlantMutation.ApplyFunctionalTo))]
    public class PlantMutations_GetRandomMutationPatch
    {
        public static bool Prepare()
        {
            return CustomizePlantsState.StateManager.State.MutantPlantsDropSeeds;
        }

        public static void Prefix(MutantPlant target, out SeedProducer.SeedInfo? __state)
        {
            __state = target.GetComponent<SeedProducer>()?.seedInfo;
        }

        public static void Postfix(MutantPlant target, SeedProducer.SeedInfo? __state)
        {
            if (__state != null)
                target.GetComponent<SeedProducer>().seedInfo = __state.Value;
        }
    }

    [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
    public class Db_Initialize_MutationsPatch
    {
        public static void Postfix()
        {
            var mutations = Db.Get().PlantMutations?.resources;

            if (mutations == null)
            {
                return;
            }

            if (CustomizePlantsState.StateManager.State.print_mutations)
            {
                MutationHelper.PrintMutations(mutations);
                return;
            }

            foreach (var data in CustomizePlantsState.StateManager.State.MutationSettings.Where(w => !mutations.Any(a => a.Id == w.id)))
            {
                Helpers.Print("Added new Mutation: " + data.id);
                mutations.Add(new PlantMutation(data.id, data.name, data.description));
            }

            foreach (var mutation in mutations)
            {
                //Helpers.PrintDebug($"{mutation.SelfModifiers}");
                MutationHelper.ProcessMutation(mutation);
            }
        }
    }

    public class MutationHelper
    {
        public static void ProcessMutation(PlantMutation mutation)
        {
            PlantMutationData setting = CustomizePlantsState.StateManager.State.MutationSettings?.FirstOrDefault(t => t.id == mutation.Id);
            if (setting == null)
                return;

            #region originalMutation
            if (setting.originalMutation != null)
            {
                mutation.originalMutation = setting.originalMutation.Value;
            }
            #endregion
            #region restrictedPrefabIDs
            if (setting.restrictedPrefabIDs != null)
            {
                mutation.restrictedPrefabIDs = setting.restrictedPrefabIDs;
            }
            #endregion
            #region requiredPrefabIDs
            if (setting.requiredPrefabIDs != null)
            {
                mutation.requiredPrefabIDs = setting.requiredPrefabIDs;
            }
            #endregion
            #region attributes
            if (setting.attributes != null)
            {
                mutation.SelfModifiers = setting.attributes.Select(s => (AttributeModifier)s).ToList();
                //mutation.SelfModifiers = setting.attributes.Cast<AttributeModifier>().ToList();
            }
            #endregion
            #region BonusCrop
            if (setting.bonusCropID != null)
            {
                mutation.bonusCropID = setting.bonusCropID.ToTag();
            }
            if (setting.bonusCropAmount != null)
            {
                mutation.bonusCropAmount = setting.bonusCropAmount.Value;
            }
            #endregion
            
            #region DiseaseDropper
            if (setting.droppedDiseaseID != null)
                mutation.droppedDiseaseID = setting.droppedDiseaseID.Value;
            if (setting.droppedDiseaseOnGrowAmount != null)
                mutation.droppedDiseaseOnGrowAmount = setting.droppedDiseaseOnGrowAmount.Value;
            if (setting.droppedDiseaseContinuousAmount != null)
                mutation.droppedDiseaseContinuousAmount = setting.droppedDiseaseContinuousAmount.Value;
            #endregion
            #region AddDiseaseToHarvest
            if (setting.harvestDiseaseID != null)
                mutation.harvestDiseaseID = setting.harvestDiseaseID.Value;
            if (setting.harvestDiseaseAmount != null)
                mutation.harvestDiseaseAmount = setting.harvestDiseaseAmount.Value;
            #endregion
            #region ForcePrefersDarkness
            if (setting.forcePrefersDarkness != null)
                mutation.forcePrefersDarkness = setting.forcePrefersDarkness.Value;
            #endregion
            #region ForceSelfHarvestOnGrown
            if (setting.forceSelfHarvestOnGrown != null)
                mutation.forceSelfHarvestOnGrown = setting.forceSelfHarvestOnGrown.Value;
            #endregion
            #region EnsureIrrigated
            if (setting.ensureIrrigationInfo != null)
                mutation.ensureIrrigationInfo = setting.ensureIrrigationInfo.Value;
            #endregion
            #region VisualTint
            if (setting.plantTint != null)
                mutation.plantTint = setting.plantTint.Value;
            #endregion
            #region VisualSymbolTint
            if (setting.symbolTintTargets != null)
                mutation.symbolTintTargets = setting.symbolTintTargets;
            if (setting.symbolTints != null)
                mutation.symbolTints = setting.symbolTints;
            #endregion
            #region VisualSymbolOverride
            if (setting.symbolOverrideInfo != null)
            {
                mutation.symbolOverrideInfo?.Clear();
                foreach (var info in setting.symbolOverrideInfo)
                {
                    string[] infos = info.Split(';');
                    if (infos.Length == 3)
                        mutation.VisualSymbolOverride(infos[0], infos[1], infos[2]);
                }
            }
            #endregion
            #region VisualSymbolScale
            if (setting.symbolScaleTargets != null)
                mutation.symbolScaleTargets = setting.symbolScaleTargets;
            if (setting.symbolScales != null)
                mutation.symbolScales = setting.symbolScales;
            #endregion
            #region VisualBGFX
            if (setting.bGFXAnim != null)
                mutation.bGFXAnim = setting.bGFXAnim;
            #endregion
            #region VisualFGFX
            if (setting.fGFXAnim != null)
                mutation.fGFXAnim = setting.fGFXAnim;
            #endregion
            #region AddSoundEvent
            if (setting.additionalSoundEvents != null)
                mutation.additionalSoundEvents = setting.additionalSoundEvents;
            #endregion
        
        }

        public static void PrintMutations(List<PlantMutation> mutations)
        {
            CustomizePlantsState.StateManager.State.MutationSettings.Clear();
            foreach (var mutation in mutations)
            {
                PlantMutationData data = new PlantMutationData();

                data.id = mutation.Id;
                data.originalMutation = mutation.originalMutation;
                data.restrictedPrefabIDs = mutation.restrictedPrefabIDs;
                data.requiredPrefabIDs = mutation.requiredPrefabIDs;
                data.attributes = mutation.SelfModifiers.Select(s => new AttributeContainer(s)).ToList();

                #region private readouts
                Tag bonusCrop = mutation.bonusCropID;
                if (bonusCrop.IsValid)
                {
                    data.bonusCropID = bonusCrop.ToString();
                    data.bonusCropAmount = mutation.bonusCropAmount;
                }

                byte droppedDiseaseID = mutation.droppedDiseaseID;
                if (droppedDiseaseID != byte.MaxValue)
                    data.droppedDiseaseID = droppedDiseaseID;

                int droppedDiseaseOnGrowAmount = mutation.droppedDiseaseOnGrowAmount;
                if (droppedDiseaseOnGrowAmount != 0)
                    data.droppedDiseaseOnGrowAmount = droppedDiseaseOnGrowAmount;

                int droppedDiseaseContinuousAmount = mutation.droppedDiseaseContinuousAmount;
                if (droppedDiseaseContinuousAmount != 0)
                    data.droppedDiseaseOnGrowAmount = droppedDiseaseContinuousAmount;

                byte harvestDiseaseID = mutation.harvestDiseaseID;
                if (harvestDiseaseID != byte.MaxValue)
                    data.harvestDiseaseID = harvestDiseaseID;

                int harvestDiseaseAmount = mutation.harvestDiseaseAmount;
                if (harvestDiseaseAmount != 0)
                    data.harvestDiseaseAmount = harvestDiseaseAmount;

                bool forcePrefersDarkness = mutation.forcePrefersDarkness;
                if (forcePrefersDarkness != false)
                    data.forcePrefersDarkness = forcePrefersDarkness;

                bool forceSelfHarvestOnGrown = mutation.forceSelfHarvestOnGrown;
                if (forceSelfHarvestOnGrown != false)
                    data.forceSelfHarvestOnGrown = forceSelfHarvestOnGrown;

                var ensureIrrigationInfo = mutation.ensureIrrigationInfo;
                if (ensureIrrigationInfo.tag.IsValid)
                    data.ensureIrrigationInfo = ensureIrrigationInfo;

                //Color plantTint = Access.plantTint(mutation);
                //if (plantTint != Color.white)
                //    data.plantTint = plantTint;

                List<string> symbolTintTargets = mutation.symbolTintTargets;
                if (symbolTintTargets.Count > 0)
                    data.symbolTintTargets = symbolTintTargets;

                //List<Color> symbolTints = Access.symbolTints(mutation);
                //if (symbolTints.Count > 0)
                //    data.symbolTints = symbolTints;

                var symbolOverrideInfo = mutation.symbolOverrideInfo;
                if (symbolOverrideInfo != null && symbolOverrideInfo.Count > 0)
                {
                    data.symbolOverrideInfo = new List<string>();
                    foreach (var item in symbolOverrideInfo)
                        data.symbolOverrideInfo.Add($"{item.targetSymbolName};{item.sourceAnim};{item.sourceSymbol}");
                }

                List<string> symbolScaleTargets = mutation.symbolScaleTargets;
                if (symbolScaleTargets.Count > 0)
                    data.symbolScaleTargets = symbolScaleTargets;

                List<float> symbolScales = mutation.symbolScales;
                if (symbolScales.Count > 0)
                    data.symbolScales = symbolScales;

                data.bGFXAnim = mutation.bGFXAnim;
                data.fGFXAnim = mutation.fGFXAnim;

                List<string> additionalSoundEvents = mutation.additionalSoundEvents;
                if (additionalSoundEvents.Count > 0)
                    data.additionalSoundEvents = additionalSoundEvents;
                #endregion

                CustomizePlantsState.StateManager.State.MutationSettings.Add(data);
            }

            CustomizePlantsState.StateManager.State.print_mutations = false;
            CustomizePlantsState.StateManager.TrySaveConfigurationState();
        }
    }

    public class PlantMutationData
    {
        private int hash;
        public string id;
        public string name;
        public string description;
        public bool? originalMutation;
        public List<string> restrictedPrefabIDs;
        public List<string> requiredPrefabIDs;
        public List<AttributeContainer> attributes;

        public string bonusCropID;
        public float? bonusCropAmount;
        public byte? droppedDiseaseID;
        public int? droppedDiseaseOnGrowAmount;
        public int? droppedDiseaseContinuousAmount;
        public byte? harvestDiseaseID;
        public int? harvestDiseaseAmount;
        public bool? forcePrefersDarkness;
        public bool? forceSelfHarvestOnGrown;
        public PlantElementAbsorber.ConsumeInfo? ensureIrrigationInfo;
        public Color? plantTint;
        public List<string> symbolTintTargets;
        public List<Color> symbolTints;
        public List<string> symbolOverrideInfo;
        public List<string> symbolScaleTargets;
        public List<float> symbolScales;
        public string bGFXAnim;
        public string fGFXAnim;
        public List<string> additionalSoundEvents;

        /// <summary>
        /// Holds settings for mutation.
        /// </summary>
        /// <param name="id">Required. Defines which mutation is changed.</param>
        /// 
        public PlantMutationData(string id, string name = null, string description = null, bool? originalMutation = null, List<string> restrictedPrefabIDs = null, List<string> requiredPrefabIDs = null, List<AttributeContainer> attributes = null, string bonusCropID = null, float? bonusCropAmount = null, byte? droppedDiseaseID = null, int? droppedDiseaseOnGrowAmount = null, int? droppedDiseaseContinuousAmount = null, byte? harvestDiseaseID = null, int? harvestDiseaseAmount = null, bool? forcePrefersDarkness = null, bool? forceSelfHarvestOnGrown = null, PlantElementAbsorber.ConsumeInfo? ensureIrrigationInfo = null, Color? plantTint = null, List<string> symbolTintTargets = null, List<Color> symbolTints = null, List<string> symbolOverrideInfo = null, string sourceAnim = null, string sourceSymbol = null, List<string> symbolScaleTargets = null, List<float> symbolScales = null, string bGFXAnim = null, string fGFXAnim = null, List<string> additionalSoundEvents = null)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.originalMutation = originalMutation;
            this.restrictedPrefabIDs = restrictedPrefabIDs;
            this.requiredPrefabIDs = requiredPrefabIDs;
            this.attributes = attributes;

            this.bonusCropID = bonusCropID;
            this.bonusCropAmount = bonusCropAmount;
            this.droppedDiseaseID = droppedDiseaseID;
            this.droppedDiseaseOnGrowAmount = droppedDiseaseOnGrowAmount;
            this.droppedDiseaseContinuousAmount = droppedDiseaseContinuousAmount;
            this.harvestDiseaseID = harvestDiseaseID;
            this.harvestDiseaseAmount = harvestDiseaseAmount;
            this.forcePrefersDarkness = forcePrefersDarkness;
            this.forceSelfHarvestOnGrown = forceSelfHarvestOnGrown;
            this.ensureIrrigationInfo = ensureIrrigationInfo;
            this.plantTint = plantTint;
            this.symbolTintTargets = symbolTintTargets;
            this.symbolTints = symbolTints;
            this.symbolOverrideInfo = symbolOverrideInfo;
            this.symbolScaleTargets = symbolScaleTargets;
            this.symbolScales = symbolScales;
            this.bGFXAnim = bGFXAnim;
            this.fGFXAnim = fGFXAnim;
            this.additionalSoundEvents = additionalSoundEvents;
        }

        public PlantMutationData()
        { }

        public override bool Equals(object obj)
        {
            return this.GetHashCode() == (obj as PlantMutationData)?.GetHashCode();
        }

        public override int GetHashCode()
        {
            if (hash == 0)
                hash = Hash.SDBMLower(this.id);
            return hash;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

}
