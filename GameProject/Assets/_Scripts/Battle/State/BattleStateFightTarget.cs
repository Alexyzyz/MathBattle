using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateFightTarget : BattleStateBase, SubscriberInterface
{

  private SkillScriptable selectedSkill;
  private List<BattleUnitBase> targetList;

  // state methods

  public override void EnterState<T>(T data) {
    selectedSkill = data as SkillScriptable;

    BattleTileSelection.Instance.SetPendingSkill(selectedSkill);

    SetHoverableTiles();

    BattleTileSelection.Instance.ToggleEnableSelectTile(true);
    BattleTileSelection.Instance.SelectedTile = null;

    BattleCanvasMenuFightList.Instance.Hide();
    BattleCanvasFightSkillDescription.Instance.Shrink();

    BattleCanvasFightTargetTilePrompt.Instance.Show();
    BattleCanvasFightTargetTilePrompt.Instance.SetSkillCost(selectedSkill.APCost);
  }

  public override void LeaveState(BattleState outgoingState) {
    BattleTileSelection.Instance.SetPendingSkill(null);
    BattleTileSelection.Instance.ClearTargetSelectedTiles();
    BattleTileSelection.Instance.ClearTargetHoveredTiles();

    BattleTileSelection.Instance.ToggleEnableSelectTile(false);
    BattleTileSelection.Instance.SetAllTilesHoverable();
    BattleTileSelection.Instance.SelectedTile = BattleUnitManager.Instance.SelectedHero.Tile;

    BattleCanvasFightTargetTilePrompt.Instance.Hide();
  }

  public override void UpdateState() {
    HandleSelectTarget();
    HandleBackToSkillList();
  }

  // methods

  private void SetHoverableTiles() {
    BattleTileSelection.Instance.SetHoverableTiles(
      BattleTileManager.Instance.GetSkillHoverableTiles(selectedSkill.id));
  }

  private void HandleSelectTarget() {
    if (!Input.GetKeyDown(GameKeyMapping.ConfirmKey)) return;

    // check if selected tiles have at least one target
    
    if (targetList.Count == 0) return;
    SelectedHero.AP -= selectedSkill.APCost;

    AttackData attackData = new AttackData {
      attacker = SelectedHero,
      targetList = targetList,
      attackerSkill = selectedSkill,
    };
    BattleController.Instance.SetAttackData(attackData);
    BattleController.Instance.SetMathData(selectedSkill);

    BattleStateManager.Instance.SwitchState(BattleState.Math);
  }

  private List<BattleUnitBase> HandleSkillTargetSelection() {
    List<BattleUnitBase> targetUnitList = new List<BattleUnitBase>();

    // TODO: dirty failsafe since selectedSkill assignment can lag behind this being called
    if (selectedSkill == null) return targetUnitList;

    List<BattleTile> targetTileList = BattleTileManager.Instance.GetSkillScopeTiles(selectedSkill.id, SelectedTile);

    foreach (BattleTile targetTile in targetTileList) {
      if (targetTile && targetTile.Unit && !targetTile.Unit.IsHero) {
        targetUnitList.Add(targetTile.Unit);
      }
    }
    return targetUnitList;
  }

  private void HandleBackToSkillList() {
    if (!Input.GetKeyDown(GameKeyMapping.CancelKey)) return;

    StateManager.SwitchState(BattleState.FightMenu);
  }

  // signal methods

  private void HandleGetTargetList(BattleTile tile) {
    targetList = HandleSkillTargetSelection();
    BattleCanvasFightTargetTilePrompt.Instance.ToggleSelectPromptAlpha(targetList.Count > 0);
  }

  // signals

  public override void Subscribe() {
    BattleTileSelection.OnSelectTile += HandleGetTargetList;
  }

  public override void Unsubscribe() {
    BattleTileSelection.OnSelectTile -= HandleGetTargetList;
  }

  // init

  public override void Init(BattleStateManager stateManager) {
    StateManager = stateManager;
    Subscribe();
  }

  // util

  private BattleTile SelectedTile {
    get {
      return BattleTileSelection.Instance.SelectedTile;
    }
  }

  private BattleUnitBase SelectedHero {
    get {
      return BattleUnitManager.Instance.SelectedHero;
    }
  }

}
