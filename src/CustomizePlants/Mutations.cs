using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Common;
using Harmony;
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

#if DLC1
    [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
    public class Db_Initialize_MutationsPatch
    {
        public static void Postfix()
        {
            var mutations = Db.Get().PlantMutations.resources;

            if (mutations == null)
            {
                return;
            }

            if (CustomizePlantsState.StateManager.State.print_mutations)
            {
                MutationHelper.PrintMutations(mutations);
                return;
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
        public static FastGetter Get_bonusCropID = Helpers.CreateGetter(typeof(PlantMutation), "bonusCropID");
        public static FastGetter Get_bonusCropAmount = Helpers.CreateGetter(typeof(PlantMutation), "bonusCropAmount");
        public static FastSetter Set_bonusCropID = Helpers.CreateSetter(typeof(PlantMutation), "bonusCropID");
        public static FastSetter Set_bonusCropAmount = Helpers.CreateSetter(typeof(PlantMutation), "bonusCropAmount");

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
                mutation.restrictedPrefabIDs = new List<string>(setting.restrictedPrefabIDs);
            }
            #endregion
            #region requiredPrefabIDs
            if (setting.requiredPrefabIDs != null)
            {
                mutation.requiredPrefabIDs = new List<string>(setting.requiredPrefabIDs);
            }
            #endregion
            #region attributes
            if (setting.attributes != null)
            {
                mutation.SelfModifiers = setting.attributes.Select(s => (AttributeModifier)s).ToList();
            }
            #endregion
            #region bonusCrop
            if (setting.bonusCropID != null)
            {
                Set_bonusCropID(mutation, (Tag)setting.bonusCropID);
            }
            if (setting.bonusCropAmount != null)
            {
                Set_bonusCropAmount(mutation, setting.bonusCropAmount.Value);
            }
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

                Tag bonusCrop = (Tag)Get_bonusCropID(mutation);
                if (bonusCrop.IsValid)
                {
                    data.bonusCropID = bonusCrop.ToString();
                    data.bonusCropAmount = (float)Get_bonusCropAmount(mutation);
                }

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
        public string targetSymbolName;    //symbolOverrideInfo
        public string sourceAnim;    //symbolOverrideInfo
        public string sourceSymbol;    //symbolOverrideInfo
        public List<string> symbolScaleTargets;
        public List<float> symbolScales;
        public string bGFXAnim;
        public string fGFXAnim;

        /// <summary>
        /// Holds settings for mutation.
        /// </summary>
        /// <param name="id">Required. Defines which mutation is changed.</param>
        /// 

        public PlantMutationData(string id, bool? originalMutation = null, List<string> restrictedPrefabIDs = null, List<string> requiredPrefabIDs = null, List<AttributeContainer> attributes = null, string bonusCropID = null, float? bonusCropAmount = null, byte? droppedDiseaseID = null, int? droppedDiseaseOnGrowAmount = null, int? droppedDiseaseContinuousAmount = null, byte? harvestDiseaseID = null, int? harvestDiseaseAmount = null, bool? forcePrefersDarkness = null, bool? forceSelfHarvestOnGrown = null, PlantElementAbsorber.ConsumeInfo? ensureIrrigationInfo = null, Color? plantTint = null, List<string> symbolTintTargets = null, List<Color> symbolTints = null, string targetSymbolName = null, string sourceAnim = null, string sourceSymbol = null, List<string> symbolScaleTargets = null, List<float> symbolScales = null, string bGFXAnim = null, string fGFXAnim = null)
        {
            this.id = id;
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
            this.targetSymbolName = targetSymbolName;    //symbolOverrideInfo
            this.sourceAnim = sourceAnim;    //symbolOverrideInfo
            this.sourceSymbol = sourceSymbol;    //symbolOverrideInfo
            this.symbolScaleTargets = symbolScaleTargets;
            this.symbolScales = symbolScales;
            this.bGFXAnim = bGFXAnim;
            this.fGFXAnim = fGFXAnim;
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
