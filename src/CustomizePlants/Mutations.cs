using System;
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

    [HarmonyPatch(typeof(PlantMutations), nameof(PlantMutations.GetRandomMutation))]
    public class PlantMutations_GetRandomMutationPatch
    {
        public static bool Prepare()
        {
            return CustomizePlantsState.StateManager.State.MutantPlantsDropSeeds;
        }

        public static bool Prefix(string targetPlantPrefabID, ref PlantMutation __result)
        {
            __result = (from m in Db.Get().PlantMutations.resources
                        where !m.originalMutation && !m.restrictedPrefabIDs.Contains(targetPlantPrefabID) && (m.requiredPrefabIDs.Count == 0 || m.requiredPrefabIDs.Contains(targetPlantPrefabID))
                        select m).ToList().GetRandom();

            return false;
        }
    }

#if DLC1

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

            foreach (var data in CustomizePlantsState.StateManager.State.MutationSettings.Where(w => mutations.Any(a => a.Id == w.id)))
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
                Access.bonusCropID(mutation) = (Tag)setting.bonusCropID;
            }
            if (setting.bonusCropAmount != null)
            {
                Access.bonusCropAmount(mutation) = setting.bonusCropAmount.Value;
            }
            #endregion
            
            #region DiseaseDropper
            if (setting.droppedDiseaseID != null)
                Access.droppedDiseaseID(mutation) = setting.droppedDiseaseID.Value;
            if (setting.droppedDiseaseOnGrowAmount != null)
                Access.droppedDiseaseOnGrowAmount(mutation) = setting.droppedDiseaseOnGrowAmount.Value;
            if (setting.droppedDiseaseContinuousAmount != null)
                Access.droppedDiseaseContinuousAmount(mutation) = setting.droppedDiseaseContinuousAmount.Value;
            #endregion
            #region AddDiseaseToHarvest
            if (setting.harvestDiseaseID != null)
                Access.harvestDiseaseID(mutation) = setting.harvestDiseaseID.Value;
            if (setting.harvestDiseaseAmount != null)
                Access.harvestDiseaseAmount(mutation) = setting.harvestDiseaseAmount.Value;
            #endregion
            #region ForcePrefersDarkness
            if (setting.forcePrefersDarkness != null)
                Access.forcePrefersDarkness(mutation) = setting.forcePrefersDarkness.Value;
            #endregion
            #region ForceSelfHarvestOnGrown
            if (setting.forceSelfHarvestOnGrown != null)
                Access.forceSelfHarvestOnGrown(mutation) = setting.forceSelfHarvestOnGrown.Value;
            #endregion
            #region EnsureIrrigated
            if (setting.ensureIrrigationInfo != null)
                Access.ensureIrrigationInfo(mutation) = setting.ensureIrrigationInfo.Value;
            #endregion
            #region VisualTint
            if (setting.plantTint != null)
                Access.plantTint(mutation) = setting.plantTint.Value;
            #endregion
            #region VisualSymbolTint
            if (setting.symbolTintTargets != null)
                Access.symbolTintTargets(mutation) = setting.symbolTintTargets;
            if (setting.symbolTints != null)
                Access.symbolTints(mutation) = setting.symbolTints;
            #endregion
            #region VisualSymbolOverride
            if (setting.symbolOverrideInfo != null)
            {
                Access.symbolOverrideInfo(mutation)?.Clear();
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
                Access.symbolScaleTargets(mutation) = setting.symbolScaleTargets;
            if (setting.symbolScales != null)
                Access.symbolScales(mutation) = setting.symbolScales;
            #endregion
            #region VisualBGFX
            if (setting.bGFXAnim != null)
                Access.bGFXAnim(mutation) = setting.bGFXAnim;
            #endregion
            #region VisualFGFX
            if (setting.fGFXAnim != null)
                Access.fGFXAnim(mutation) = setting.fGFXAnim;
            #endregion
            #region AddSoundEvent
            if (setting.additionalSoundEvents != null)
                Access.additionalSoundEvents(mutation) = setting.additionalSoundEvents;
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
                Tag bonusCrop = (Tag)Access.bonusCropID(mutation);
                if (bonusCrop.IsValid)
                {
                    data.bonusCropID = bonusCrop.ToString();
                    data.bonusCropAmount = (float)Access.bonusCropAmount(mutation);
                }

                byte droppedDiseaseID = Access.droppedDiseaseID(mutation);
                if (droppedDiseaseID != byte.MaxValue)
                    data.droppedDiseaseID = droppedDiseaseID;

                int droppedDiseaseOnGrowAmount = Access.droppedDiseaseOnGrowAmount(mutation);
                if (droppedDiseaseOnGrowAmount != 0)
                    data.droppedDiseaseOnGrowAmount = droppedDiseaseOnGrowAmount;

                int droppedDiseaseContinuousAmount = Access.droppedDiseaseContinuousAmount(mutation);
                if (droppedDiseaseContinuousAmount != 0)
                    data.droppedDiseaseOnGrowAmount = droppedDiseaseContinuousAmount;

                byte harvestDiseaseID = Access.harvestDiseaseID(mutation);
                if (harvestDiseaseID != byte.MaxValue)
                    data.harvestDiseaseID = harvestDiseaseID;

                int harvestDiseaseAmount = Access.harvestDiseaseAmount(mutation);
                if (harvestDiseaseAmount != 0)
                    data.harvestDiseaseAmount = harvestDiseaseAmount;

                bool forcePrefersDarkness = Access.forcePrefersDarkness(mutation);
                if (forcePrefersDarkness != false)
                    data.forcePrefersDarkness = forcePrefersDarkness;

                bool forceSelfHarvestOnGrown = Access.forceSelfHarvestOnGrown(mutation);
                if (forceSelfHarvestOnGrown != false)
                    data.forceSelfHarvestOnGrown = forceSelfHarvestOnGrown;

                var ensureIrrigationInfo = Access.ensureIrrigationInfo(mutation);
                if (ensureIrrigationInfo.tag.IsValid)
                    data.ensureIrrigationInfo = ensureIrrigationInfo;

                //Color plantTint = Access.plantTint(mutation);
                //if (plantTint != Color.white)
                //    data.plantTint = plantTint;

                List<string> symbolTintTargets = Access.symbolTintTargets(mutation);
                if (symbolTintTargets.Count > 0)
                    data.symbolTintTargets = symbolTintTargets;

                //List<Color> symbolTints = Access.symbolTints(mutation);
                //if (symbolTints.Count > 0)
                //    data.symbolTints = symbolTints;

                IList symbolOverrideInfo = Access.symbolOverrideInfo(mutation);
                if (symbolOverrideInfo != null && symbolOverrideInfo.Count > 0)
                {
                    data.symbolOverrideInfo = new List<string>();
                    foreach (var obj in symbolOverrideInfo)
                        data.symbolOverrideInfo.Add($"{Access.targetSymbolName(obj)};{Access.sourceAnim(obj)};{Access.sourceSymbol(obj)}");
                }

                List<string> symbolScaleTargets = Access.symbolScaleTargets(mutation);

                if (symbolScaleTargets.Count > 0)
                    data.symbolScaleTargets = symbolScaleTargets;

                List<float> symbolScales = Access.symbolScales(mutation);
                if (symbolScales.Count > 0)
                    data.symbolScales = symbolScales;

                data.bGFXAnim = Access.bGFXAnim(mutation);
                data.fGFXAnim = Access.fGFXAnim(mutation);

                List<string> additionalSoundEvents = Access.additionalSoundEvents(mutation);
                if (additionalSoundEvents.Count > 0)
                    data.additionalSoundEvents = additionalSoundEvents;
                #endregion

                CustomizePlantsState.StateManager.State.MutationSettings.Add(data);
            }

            CustomizePlantsState.StateManager.State.print_mutations = false;
            CustomizePlantsState.StateManager.TrySaveConfigurationState();
        }
    }

#endif

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
