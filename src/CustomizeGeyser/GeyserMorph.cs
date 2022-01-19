using Common;
using HarmonyLib;
using KSerialization;
using System;
using System.Linq;
using TUNING;
using UnityEngine;

namespace CustomizeGeyser
{
    [HarmonyPatch(typeof(GeyserGenericConfig), "CreateGeyser")]
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
        [Serialize]
        private string currentGeyserSelection;
        [Serialize]
        private Tag currentGeyserSelectionTag;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenu);

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
            this.SetWorkTime( Math.Max(CustomizeGeyserState.StateManager.State.GeyserMorphWorktime, 3) );
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            if (!currentGeyserSelectionTag.IsValid)
                currentGeyserSelectionTag = "geyser_" + this.gameObject.name.Replace("GeyserGeneric_", "");
            this.UpdateButton();
            //this.morphedIndicator = new MeterController(this.GetComponent<KBatchedAnimController>(), this.meterTrackerSymbol, this.meterAnim, Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[1] { this.meterTrackerSymbol });
        }

        private void OnRefreshUserMenu(object data)
        {
            if (!this.showCmd)
                return;

            Game.Instance.userMenu.AddButton(this.gameObject, this.chore != null ?
                  new KIconButtonMenu.ButtonInfo("action_morph_geyser", Strings.Get("CustomizeGeyser.LOCSTRINGS.Cancel_morphing"), new System.Action(this.ToggleChore), tooltipText: Strings.Get("CustomizeGeyser.LOCSTRINGS.Cancel_morphing_tooltip"))
                : new KIconButtonMenu.ButtonInfo("action_morph_geyser", Strings.Get("CustomizeGeyser.LOCSTRINGS.Morph_into"), new System.Action(this.ToggleChore), tooltipText: Strings.Get("CustomizeGeyser.LOCSTRINGS.Morph_into_tooltip")), 1f);

            Game.Instance.userMenu.AddButton(this.gameObject, new KIconButtonMenu.ButtonInfo("action_morph_type", buttonText, new System.Action(this.NextGeyser), tooltipText: "Click for next geyser type."), 2f);

        }

        public void NextGeyser()
        {
            string selection = this.currentGeyserSelectionTag.Name.Substring(7);
            int index = GeyserInfo.Config.FindIndex(x => x.id == selection) + 1;
            if (index >= GeyserInfo.Config.Count) index = 0;

            this.currentGeyserSelectionTag = "geyser_" + GeyserInfo.Config[index].id.Replace("GeyserGeneric_", "");
            this.UpdateButton();
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
                    this.OnCompleteWork(null);
                }
                else
                {
                    this.chore = new WorkChore<GeyserMorph>(chore_type: Db.Get().ChoreTypes.Research, target: this, only_when_operational: false);
                    this.statusItemGuid = this.GetComponent<KSelectable>().AddStatusItem(this.workerStatusItem);
                }
            }
            else
            {
                this.CancelChore();
            }
            //this.morphedIndicator.gameObject.SetActive(false);  //is this right?
        }

        public void CancelChore()
        {
            Helpers.PrintDebug("DEBUG CancelChore");
            if (this.chore == null)
                return;
            this.chore.Cancel("GeyserMorph.CancelChore");
            this.chore = null;
            this.statusItemGuid = this.GetComponent<KSelectable>().RemoveStatusItem(this.workerStatusItem, false);
        }

        protected override void OnCompleteWork(Worker worker)
        {
            Helpers.PrintDebug("DEBUG OnCompleteWork");
            this.CancelChore();
            //this.TriggerTextDialog();
            ChangeGeyserElement(this.gameObject, currentGeyserSelection);
        }

		public static bool ChangeGeyserElement(GameObject go, string new_id = null)
        {
            if (new_id == null)
                new_id = "GeyserGeneric";

            try
            {
                GameUtil.KInstantiate(Assets.GetPrefab((Tag)new_id), go.transform.GetPosition(), Grid.SceneLayer.BuildingBack, (string)null, 0).SetActive(true);
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
            textDialog.onConfirm = (System.Action<string>)(name =>
            {
                if (name != null)
                {
                    name = name.Substring(0, Math.Max(0, name.Length - 4));

                    GeyserConfigurator.GeyserType type = GeyserConfigurator.FindType((HashedString)name);

                    ChangeGeyserElement(this.gameObject, type?.id);
                }
            });

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