using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Common;
using HarmonyLib;
using Klei.AI;
using UnityEngine;
using static HarmonyLib.AccessTools;

namespace CustomizePlants
{
    public class Access
    {
        #region PlantMutation
        public static readonly FieldRef<PlantMutation, Tag> bonusCropID;
        public static readonly FieldRef<PlantMutation, float> bonusCropAmount;
        public static readonly FieldRef<PlantMutation, byte> droppedDiseaseID;
        public static readonly FieldRef<PlantMutation, int> droppedDiseaseOnGrowAmount;
        public static readonly FieldRef<PlantMutation, int> droppedDiseaseContinuousAmount;
        public static readonly FieldRef<PlantMutation, byte> harvestDiseaseID;
        public static readonly FieldRef<PlantMutation, int> harvestDiseaseAmount;
        public static readonly FieldRef<PlantMutation, bool> forcePrefersDarkness;
        public static readonly FieldRef<PlantMutation, bool> forceSelfHarvestOnGrown;
        public static readonly FieldRef<PlantMutation, PlantElementAbsorber.ConsumeInfo> ensureIrrigationInfo;
        public static readonly FieldRef<PlantMutation, Color> plantTint;
        public static readonly FieldRef<PlantMutation, List<string>> symbolTintTargets;
        public static readonly FieldRef<PlantMutation, List<Color>> symbolTints;
        public static readonly FieldRef<PlantMutation, System.Collections.IList> symbolOverrideInfo;
        public static readonly FieldRef<object, string> targetSymbolName;
        public static readonly FieldRef<object, string> sourceAnim;
        public static readonly FieldRef<object, string> sourceSymbol;
        public static readonly FieldRef<PlantMutation, List<string>> symbolScaleTargets;
        public static readonly FieldRef<PlantMutation, List<float>> symbolScales;
        public static readonly FieldRef<PlantMutation, string> bGFXAnim;
        public static readonly FieldRef<PlantMutation, string> fGFXAnim;
        public static readonly FieldRef<PlantMutation, List<string>> additionalSoundEvents;
        #endregion

        #region PlantablePlot
        public static readonly FieldRef<PlantablePlot, bool> accepts_irrigation;
        public static readonly FieldRef<PlantablePlot, bool> accepts_fertilizer;
        public static readonly FieldRef<SingleEntityReceptacle, List<Tag>> possibleDepositTagsList;
        #endregion

        static Access()
        {
            // no inline definitions, so we get more meaningful debug exceptions
            try
            {
                //Helpers.Print(typeof(PlantMutation).GetNestedTypes(BindingFlags.NonPublic).Select(s => s.AssemblyQualifiedName).Join());
                Type TSymbolOverrideInfo = Type.GetType("Klei.AI.PlantMutation+SymbolOverrideInfo, Assembly-CSharp");

                #region PlantMutation
                bonusCropID = FieldRefAccess<PlantMutation, Tag>("bonusCropID");    // TODO: Tag is a struct and probably doesn't work. needs testing!
                bonusCropAmount = FieldRefAccess<PlantMutation, float>("bonusCropAmount");
                droppedDiseaseID = FieldRefAccess<PlantMutation, byte>("droppedDiseaseID");
                droppedDiseaseOnGrowAmount = FieldRefAccess<PlantMutation, int>("droppedDiseaseOnGrowAmount");
                droppedDiseaseContinuousAmount = FieldRefAccess<PlantMutation, int>("droppedDiseaseContinuousAmount");
                harvestDiseaseID = FieldRefAccess<PlantMutation, byte>("harvestDiseaseID");
                harvestDiseaseAmount = FieldRefAccess<PlantMutation, int>("harvestDiseaseAmount");
                forcePrefersDarkness = FieldRefAccess<PlantMutation, bool>("forcePrefersDarkness");
                forceSelfHarvestOnGrown = FieldRefAccess<PlantMutation, bool>("forceSelfHarvestOnGrown");
                ensureIrrigationInfo = FieldRefAccess<PlantMutation, PlantElementAbsorber.ConsumeInfo>("ensureIrrigationInfo");
                plantTint = FieldRefAccess<PlantMutation, Color>("plantTint");
                symbolTintTargets = FieldRefAccess<PlantMutation, List<string>>("symbolTintTargets");
                symbolTints = FieldRefAccess<PlantMutation, List<Color>>("symbolTints");
                symbolOverrideInfo = FieldRefAccess<PlantMutation, System.Collections.IList>("symbolOverrideInfo");
                targetSymbolName = FieldRefAccess<object, string>(Field(TSymbolOverrideInfo, "targetSymbolName"));
                sourceAnim = FieldRefAccess<object, string>(Field(TSymbolOverrideInfo, "sourceAnim"));
                sourceSymbol = FieldRefAccess<object, string>(Field(TSymbolOverrideInfo, "sourceSymbol"));
                symbolScaleTargets = FieldRefAccess<PlantMutation, List<string>>("symbolScaleTargets");
                symbolScales = FieldRefAccess<PlantMutation, List<float>>("symbolScales");
                bGFXAnim = FieldRefAccess<PlantMutation, string>("bGFXAnim");
                fGFXAnim = FieldRefAccess<PlantMutation, string>("fGFXAnim");
                additionalSoundEvents = FieldRefAccess<PlantMutation, List<string>>("additionalSoundEvents");
                #endregion

                #region PlantablePlot
                accepts_irrigation = FieldRefAccess<PlantablePlot, bool>("accepts_irrigation");
                accepts_fertilizer = FieldRefAccess<PlantablePlot, bool>("accepts_fertilizer");
                possibleDepositTagsList = FieldRefAccess<SingleEntityReceptacle, List<Tag>>("possibleDepositTagsList");
                #endregion

                foreach (var field in typeof(Access).GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    if (field.GetValue(null) == null)
                        Helpers.Print($"Error: Field Access.{field.Name} is null!");
                }
            }
            catch (Exception e)
            {
                Helpers.Print(e.ToString());
            }
        }

        public static void Dispose()
        {
            foreach (var field in typeof(Access).GetFields(BindingFlags.Public | BindingFlags.Static))
                field.SetValue(null, null);
        }
    }
}