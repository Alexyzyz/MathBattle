using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateGameOver : BattleStateBase
{

  public static DelegateVoid OnGameOver { get; set; }

  // state methods

  public override void EnterState<T>(T data) {
    GameSoundManager.Instance.PlaySound(GameSound.BattleFinish);

    BattleTileSelection.Instance.ToggleEnableSelectTile(false);

    BattleCanvasMenuHeader.Instance.Hide();
    BattleCanvasMenuMainContainer.Instance.ToggleShow(false);
    BattleCanvasUnitInfo.Instance.ToggleShow(false);

    OnGameOver?.Invoke();

    BattleCanvasOver.Instance.Show();

    HandleSaveGame();
  }

  public override void LeaveState(BattleState outgoingState) {
  }

  public override void UpdateState() {
    HandleBackToTitle();
  }

  // methods

  private void HandleSaveGame() {
    SaveData saveData = UtilSave.LoadGame();
    saveData.battleRecordList.Add(BattleController.Instance.BattleRecord);
    UtilSave.SaveGame(saveData);
  }

  private void HandleBackToTitle() {
    if (!Input.GetKeyDown(GameKeyMapping.ConfirmKey)) return;
    CanvasSceneTransition.Instance.TransitionToScene(CoverTransition.Right, CoverTransition.Center, "TitleScene");
  }

  // signals

  public override void Subscribe() { }

  public override void Unsubscribe() { }

  // base methods

  public override void Init(BattleStateManager stateManager) {
    StateManager = stateManager;
    Subscribe();
  }

}
