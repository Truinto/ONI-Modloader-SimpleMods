using Common;
using HarmonyLib;
using KSerialization;
using System;
using System.Linq;
using TUNING;
using UnityEngine;

namespace CustomizeGeyser
{
    [HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.CreateGeyser), [typeof(string), typeof(string), typeof(int), typeof(int), typeof(string), typeof(string), typeof(HashedString), typeof(float), typeof(string[]), typeof(string[])])]
    public static class MorphPatch
    {
        public static bool Prepare()
        {
            return CustomizeGeyserState.StateManager.State.GeyserMorphEnabled;
        }

        public static void Postfix(GameObject __result)
        {
            __result.AddOrGet<GeyserMorph>();
        }
    }
    
    public class GeyserMorph : Workable
    {
        public string meterTrackerSymbol;
        public string meterAnim;
        private Chore chore;
        private Guid statusItemGuid;
        private bool showCmd = true;

        private string buttonText;
        private int currentGeyserIndex;
        [Serialize]
        private Tag currentGeyserSelectionTag;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenu);

            this.overrideAnims = new KAnimFile[1] { Assets.GetAnim((HashedString) "anim_use_machine_kanim") };
            this.faceTargetWhenWorking = true;
            this.synchronizeAnims = false;
            this.workerStatusItem = Db.Get().DuplicantStatusItems.Studying;
            this.resetProgressOnStop = false;
            this.requiredSkillPerk = Db.Get().SkillPerks.CanStudyWorldObjects.Id;
            this.attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
            this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
            this.skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
            this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
            SetWorkTime( Math.Max(CustomizeGeyserState.StateManager.State.GeyserMorphWorktime, 3) );
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            if (!currentGeyserSelectionTag.IsValid)
            {
                string name = this.gameObject.PrefabID().Name;
                if (name is null or "")
                    name = this.gameObject.name;
                currentGeyserSelectionTag = name.Replace("GeyserGeneric_", "");
            }

            string selection = currentGeyserSelectionTag.Name;
            currentGeyserIndex = GeyserInfo.GeyserTypes.FindIndex(x => x.id == selection);
            
            if (currentGeyserIndex < 0)
            {
                selection = "GeyserGeneric_" + selection;
                currentGeyserIndex = GeyserInfo.GeyserTypes.FindIndex(x => x.id == selection);
            }

            if (currentGeyserIndex < 0)
                currentGeyserIndex = 0;

            UpdateButton();
            //this.morphedIndicator = new MeterController(this.GetComponent<KBatchedAnimController>(), this.meterTrackerSymbol, this.meterAnim, Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[1] { this.meterTrackerSymbol });
        }

        private void OnRefreshUserMenu(object data)
        {
            if (!this.showCmd)
                return;

            Game.Instance.userMenu.AddButton(this.gameObject, this.chore != null ?
                  new KIconButtonMenu.ButtonInfo("action_morph_geyser", Strings.Get("CustomizeGeyser.LOCSTRINGS.Cancel_morphing"), new System.Action(ToggleChore), tooltipText: Strings.Get("CustomizeGeyser.LOCSTRINGS.Cancel_morphing_tooltip"))
                : new KIconButtonMenu.ButtonInfo("action_morph_geyser", Strings.Get("CustomizeGeyser.LOCSTRINGS.Morph_into"), new System.Action(ToggleChore), tooltipText: Strings.Get("CustomizeGeyser.LOCSTRINGS.Morph_into_tooltip")), 1f);

            Game.Instance.userMenu.AddButton(this.gameObject, new KIconButtonMenu.ButtonInfo("action_morph_type", buttonText, new System.Action(NextGeyser), tooltipText: Strings.Get("CustomizeGeyser.LOCSTRINGS.Next_geyser_tooltip")), 2f);

        }

        public void NextGeyser()
        {
            currentGeyserIndex++;
            if (currentGeyserIndex >= GeyserInfo.GeyserTypes.Count) currentGeyserIndex = 0;

            this.currentGeyserSelectionTag = "geyser_" + GeyserInfo.GeyserTypes[currentGeyserIndex].id.Replace("GeyserGeneric_", "");
            UpdateButton();
        }

        private void UpdateButton()
        {
            buttonText = currentGeyserSelectionTag.ProperName();
        }

        public void ToggleChore()
        {
            Helpers.PrintDebug("DEBUG ToggleChore");

            //if (KMonoBehaviour.isLoadingScene) return;
            if (this.chore == null)
            {
                if (DebugHandler.InstantBuildMode)
                {
                    OnCompleteWork(null);
                }
                else
                {
                    this.chore = new WorkChore<GeyserMorph>(chore_type: Db.Get().ChoreTypes.Research, target: this, only_when_operational: false);
                    this.statusItemGuid = GetComponent<KSelectable>().AddStatusItem(this.workerStatusItem);
                }
            }
            else
            {
                CancelChore();
            }
            //this.morphedIndicator.gameObject.SetActive(false);  //is this right?
        }

        public void CancelChore()
        {
            if (this.chore == null)
                return;
            this.chore.Cancel("GeyserMorph.CancelChore");
            this.chore = null;
            this.statusItemGuid = GetComponent<KSelectable>().RemoveStatusItem(this.workerStatusItem, false);
        }

        public override void OnCompleteWork(WorkerBase worker)
        {
            Helpers.Print($"GeyserMorph OnCompleteWork {currentGeyserIndex} {GeyserInfo.GeyserTypes[currentGeyserIndex].id}");
            CancelChore();
            //this.TriggerTextDialog();
            ChangeGeyserElement(this.gameObject, GeyserInfo.GeyserTypes[currentGeyserIndex].id);
        }

		public static bool ChangeGeyserElement(GameObject go, string? new_id = null)
        {
            try
            {
                if (new_id is null)
                    new_id = "GeyserGeneric";
                else if (!new_id.StartsWith("GeyserGeneric"))
                    new_id = $"GeyserGeneric_{new_id}";
                GameUtil.KInstantiate(Assets.GetPrefab(new_id), go.transform.GetPosition(), Grid.SceneLayer.BuildingBack, null, 0).SetActive(true);
                go.DeleteObject();
            } catch (Exception e) {
                Helpers.PrintDialog("ChangeGeyser critical failure: " + e.Message);
				return false;
            }
			return true;
        }

        public void TriggerTextDialog()
        {
            FileNameDialog textDialog = CreateTextDialog("");
            //textDialog.name = StringBoxTitle;
            textDialog.onConfirm = name =>
            {
                if (name != null)
                {
                    name = name.Substring(0, Math.Max(0, name.Length - 4));
                    GeyserConfigurator.GeyserType type = GeyserConfigurator.FindType((HashedString)name);
                    ChangeGeyserElement(this.gameObject, type?.id);
                }
            };

            SpeedControlScreen.Instance.Pause(false);
            textDialog.Activate();
        }

        public static FileNameDialog CreateTextDialog(string title)
        {
            FileNameDialog textDialog = Util.KInstantiateUI<FileNameDialog>(ScreenPrefabs.Instance.FileNameDialog.gameObject, GameScreenManager.Instance.GetParent(GameScreenManager.UIRenderTarget.ScreenSpaceOverlay));

            textDialog.name = title; //.displayName?

            //Transform titleTransform = textDialog.transform.Find("Panel")?.Find("Title_BG")?.Find("Title");
            //if (titleTransform != null && titleTransform.GetComponent<LocText>() != null)
            //    titleTransform.GetComponent<LocText>().text = title;

            return textDialog;
        }
    }
}
