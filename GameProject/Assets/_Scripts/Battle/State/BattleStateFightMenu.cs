using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateFightMenu : BattleStateBase
{

  private const string RANDOM_DAMAGE_TEXT = "{0} — {1}";
  private const string AP_INSUFFICIENT_TEXT = "<color=#d3ff00>AP</color> tidak mencukupi!";

  public static List<SkillScriptable> SkillList { get; private set; }
  private List<MenuListButtonData<SkillScriptable>> skillButtonList;

  // state methods

  public override void EnterState<T>(T data) {
    SkillList = BattleUnitManager.Instance.SelectedHero.SkillList;
    skillButtonList = GenerateButtonDataList();

    BattleTileSelection.Instance.ToggleEnableSelectTile(false);

    BattleCanvasMenuFightList.Instance.Show(skillButtonList, false, "", OnScroll, OnSelect);
    BattleCanvasFightSkillDescription.Instance.Show();

    BattleCanvasUnitInfo.Instance.SetAlphas(0.2f, 1f, 1f);
  }

  public override void LeaveState(BattleState outgoingState) {
    BattleCanvasMenuFightList.Instance.Hide();
    BattleCanvasUnitInfo.Instance.SetAlphas(1f, 1f, 1f);
  }

  public override void UpdateState() {
    HandleBackToMainMenu();
  }

  // methods

  private List<MenuListButtonData<SkillScriptable>> GenerateButtonDataList() {
    List<MenuListButtonData<SkillScriptable>> buttonDataList = new List<MenuListButtonData<SkillScriptable>>();

    foreach (SkillScriptable item in SkillList) {
      MenuListButtonData<SkillScriptable> buttonData = new MenuListButtonData<SkillScriptable> {
        data = item,
        title = item.title,
      };
      buttonDataList.Add(buttonData);
    }

    return buttonDataList;
  }

  private void OnSelect(MenuListButtonData<SkillScriptable> hoveredSkill, int index) {
    if (SelectedHero.Stats.AP >= hoveredSkill.data.APCost) {
      BattleStateManager.Instance.SwitchState(BattleState.FightTarget, hoveredSkill.data);
    } else {
      BattleCanvasInfoBanner.Instance.Show(AP_INSUFFICIENT_TEXT, 3);
    }
  }

  private void OnScroll(MenuListButtonData<SkillScriptable> hoveredSkill) {
    SkillScriptable skill = hoveredSkill.data;

    if (!skill.damageIsRandom && skill.damage == 0) {
      // skill that deals no damage
      BattleCanvasFightSkillDescription.Instance.SetInfo(
        skill.APCost,
        skill.description);
    } else {
      // skill that deals damage
      string damageText = skill.damage.ToString();
      if (skill.damageIsRandom) {
        damageText = string.Format(RANDOM_DAMAGE_TEXT, skill.damageMin, skill.damageMax);
      }
      BattleCanvasFightSkillDescription.Instance.SetInfo(
        skill.APCost,
        damageText,
        skill.critBonus,
        skill.description);
    }
  }

  private void HandleBackToMainMenu() {
    if (!Input.GetKeyDown(GameKeyMapping.CancelKey)) return;

    StateManager.SwitchState(BattleState.MainMenu);
  }

  // signals

  public override void Subscribe() { }

  public override void Unsubscribe() { }

  // util

  private BattleUnitBase SelectedHero {
    get {
      return BattleUnitManager.Instance.SelectedHero;
    }
  }

  // init

  public override void Init(BattleStateManager stateManager) {
    StateManager = stateManager;
  }

}
