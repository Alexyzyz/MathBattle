using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateMainMenu : BattleStateBase
{

  private Dictionary<BattleState, string> menuHeader = new Dictionary<BattleState, string> {
    { BattleState.FightMenu, "Serang lawan." },
    { BattleState.ItemMenu, "Gunakan item." },
    { BattleState.MoveMenu, "Pindah ke tile lain." },
    { BattleState.EndMenu, "Akhiri giliranmu." },
  };

  private BattleState hoveredOption = BattleState.FightMenu;

  public static DelegateVoid OnBackToMainMenu { get; set; }
  public static DelegateBattleState OnSelectMainMenuOption { get; set; }
  public static DelegateBattleState OnHoverMainMenuOption { get; set; }

  // state methods

  public override void EnterState<T>(T data) {
    BattleCanvasMenuMainContainer.Instance.ToggleShow(true);
    BattleTileSelection.Instance.ToggleEnableSelectTile(true);

    BattleCanvasMenuHeader.Instance.SetText(menuHeader[hoveredOption]);
    BattleCanvasMenuHeader.Instance.Unshift();
    BattleCanvasMenuHeader.Instance.Show();

    BattleCanvasMenuBattleInfo.Instance.Show();

    BattleCanvasUnitInfo.Instance.SetValues(BattleUnitManager.Instance.SelectedHero.Stats);
    BattleCanvasUnitInfo.Instance.ToggleShow(true);

    BattleCanvasFightSkillDescription.Instance.Hide();

    OnBackToMainMenu?.Invoke();
    OnHoverMainMenuOption?.Invoke(hoveredOption);
  }

  public override void LeaveState(BattleState outgoingState) {
    BattleCanvasMenuMainContainer.Instance.ToggleShow(false);

    BattleCanvasMenuBattleInfo.Instance.Hide();

    if (outgoingState == BattleState.Idle) {
      BattleCanvasMenuHeader.Instance.Hide();
      hoveredOption = BattleState.FightMenu;
    } else {
      BattleCanvasMenuHeader.Instance.Shift();
    }
  }

  public override void UpdateState() {
    HandleMenu();
  }

  // methods

  private void HandleMenu() {
    HandleSelectMenuOption();

    HandleHoverMenuOption(GameKeyMapping.UpKey, BattleState.FightMenu);
    HandleHoverMenuOption(GameKeyMapping.LeftKey, BattleState.ItemMenu);
    HandleHoverMenuOption(GameKeyMapping.RightKey, BattleState.MoveMenu);
    HandleHoverMenuOption(GameKeyMapping.DownKey, BattleState.EndMenu);
  }

  private void HandleSelectMenuOption() {
    if (!Input.GetKeyDown(GameKeyMapping.ConfirmKey)) return;

    OnSelectMainMenuOption?.Invoke(hoveredOption);

    StateManager.SwitchState(hoveredOption);
  }

  private void HandleHoverMenuOption(KeyCode input, BattleState option) {
    if (Input.GetKeyDown(input)) {
      BattleCanvasMenuHeader.Instance.SetText(menuHeader[option]);

      hoveredOption = option;
      OnHoverMainMenuOption?.Invoke(option);
    }
  }

  // signal methods

  private void CheckTileHasHero(BattleTile selectedTile) {
    if (!IsActive) return;

    BattleUnitBase selectedUnit = selectedTile && selectedTile.Unit ? selectedTile.Unit : null;

    if (!selectedUnit || !selectedUnit.IsHero) {
      StateManager.SwitchState(BattleState.Idle);
      return;
    }

    if (selectedUnit.IsHero) {
      BattleUnitManager.Instance.SelectedHero = selectedUnit;
    }

    BattleUnitBase selectedHero = BattleTileSelection.Instance.SelectedTile.Unit;
    BattleCanvasUnitInfo.Instance.SetValues(selectedHero.Stats);
  }

  // signals

  public override void Subscribe() {
    BattleTileSelection.OnSelectTile += CheckTileHasHero;
  }

  public override void Unsubscribe() {
    BattleTileSelection.OnSelectTile -= CheckTileHasHero;
  }

  // base methods

  public override void Init(BattleStateManager stateManager) {
    StateManager = stateManager;
    Subscribe();
  }

}
