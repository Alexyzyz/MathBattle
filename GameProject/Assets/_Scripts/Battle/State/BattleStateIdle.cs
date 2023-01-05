using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateIdle : BattleStateBase
{

  private const string HERO_HINT = "Pilih unit kamu.";

  private bool heroHintWasShown;

  // state methods

  public override void EnterState<T>(T data) {
    if (BattleUnitManager.Instance.SelectedHero) {
      BattleUnitManager.Instance.SelectedHero.Tile.HasSelectedHero = false;
      BattleUnitManager.Instance.SelectedHero = null;
    }

    BattleCanvasIdleFlavorText.Instance.ToggleShow(true);
    BattleCanvasUnitInfo.Instance.ToggleShow(false);

    BattleTileSelection.Instance.ToggleEnableSelectTile(true);

    ShowHeroHint();
  }

  public override void LeaveState(BattleState outgoingState) {
    // set selected hero
    BattleUnitBase selectedHero = BattleTileSelection.Instance.SelectedTile.Unit;
    BattleUnitManager.Instance.SelectedHero = selectedHero;
    BattleTileSelection.Instance.SelectedTile.HasSelectedHero = true;

    BattleCanvasIdleFlavorText.Instance.ToggleShow(false);

    HideHeroHint();
  }

  public override void UpdateState() {}

  // methods

  private void ShowHeroHint() {
    if (heroHintWasShown) return;
    BattleUnitManager.Instance.HeroUnitList.ForEach((heroUnit) => heroUnit.Overhead.Hint.Show(HERO_HINT));
    heroHintWasShown = true;
  }
  private void HideHeroHint() {
    BattleUnitManager.Instance.HeroUnitList.ForEach((heroUnit) => heroUnit.Overhead.Hint.Hide());
    heroHintWasShown = false;
  }

  // signal methods

  private void HandleSelectTile(BattleTile selectedTile) {
    if (!IsActive) return;

    bool tileHasHero = selectedTile && selectedTile.Unit && selectedTile.Unit.IsHero;
    if (tileHasHero) {
      StateManager.SwitchState(BattleState.MainMenu);
    }
    HandleHoverTile(selectedTile);
  }

  private void HandleHoverTile(BattleTile hoveredTile) {
    if (!IsActive) return;

    bool tileHasHero = hoveredTile && hoveredTile.Unit && hoveredTile.Unit.IsHero;
    if (tileHasHero) {
      HideHeroHint();
    }
    else {
      ShowHeroHint();
    }
  }

  // signals

  public override void Subscribe() {
    BattleTileSelection.OnSelectTile += HandleSelectTile;
    BattleTileSelection.OnHoverTile += HandleHoverTile;
  }

  public override void Unsubscribe() { }

  // base methods

  public override void Init(BattleStateManager stateManager) {
    StateManager = stateManager;

    CanvasSceneTransition.Instance.PlayTransition(CoverTransition.Center, CoverTransition.Left);

    Subscribe();
  }

}
