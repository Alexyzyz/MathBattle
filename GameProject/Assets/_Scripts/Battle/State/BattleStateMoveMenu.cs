using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateMoveMenu : BattleStateBase
{

  // state methods

  public override void EnterState<T>(T data) {
    BattleTileSelection.Instance.SetHoverableTiles(SelectedTile.GetNeighbors(), true);

    BattleTileSelection.Instance.ToggleEnableSelectTile(true);
    BattleTileSelection.Instance.SelectedTile = null;

    BattleCanvasUnitInfo.Instance.SetAlphas(0.2f, 1f, 0.2f);

    BattleCanvasMoveTilePrompt.Instance.Show();
  }

  public override void LeaveState(BattleState outgoingState) {
    BattleTileSelection.Instance.ToggleEnableSelectTile(false);
    BattleTileSelection.Instance.SetAllTilesHoverable();
    BattleTileSelection.Instance.SelectedTile = BattleUnitManager.Instance.SelectedHero.Tile;

    BattleCanvasUnitInfo.Instance.SetAlphas(1f, 1f, 1f);

    BattleCanvasMoveTilePrompt.Instance.Hide();
  }

  public override void UpdateState() {
    HandleMoveUnit();
    HandleBackToMainMenu();
  }

  // public methods



  // methods

  private void HandleMoveUnit() {
    if (!Input.GetKeyDown(GameKeyMapping.ConfirmKey)) return;

    if (!SelectedTile) return;
    if (SelectedHero.AP < UtilData.BATTLE_BASE_MOVE_COST) return;

    SelectedHero.AP -= UtilData.BATTLE_BASE_MOVE_COST;
    SelectedHero.MoveToCoord(SelectedTile.Coord);

    BattleTileSelection.Instance.SetHoverableTiles(SelectedTile.GetNeighbors(), true);
    BattleTileSelection.Instance.SelectedTile = null;
  }

  private void HandleBackToMainMenu() {
    if (!Input.GetKeyDown(GameKeyMapping.CancelKey)) return;

    StateManager.SwitchState(BattleState.MainMenu);
  }

  // signal methods

  private void SetPromptAlpha(BattleTile tile) {
    BattleCanvasMoveTilePrompt.Instance.ToggleSelectPromptAlpha(tile);
  }

  // signals

  public override void Subscribe() {
    BattleTileSelection.OnSelectTile += SetPromptAlpha;
  }

  public override void Unsubscribe() {
    BattleTileSelection.OnSelectTile -= SetPromptAlpha;
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
