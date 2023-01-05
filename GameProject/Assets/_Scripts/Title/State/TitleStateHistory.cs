using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleStateHistory : TitleStateBase
{

  private SaveData saveData = new SaveData();

  // state methods

  public override void EnterState<T>(T data) {
    saveData = UtilSave.LoadGame();
    TitleCanvasHistoryList.Instance.Activate(saveData.battleRecordList);
  }

  public override void LeaveState(TitleState outgoingState) {
    TitleCanvasHistoryList.Instance.Deactivate();

    TitleCanvasScreenContainer.Instance.SwitchScreen(TitleState.History, outgoingState);
  }

  public override void UpdateState() {
    HandleBackToMain();
  }

  // methods

  private void HandleBackToMain() {
    if (!Input.GetKeyDown(GameKeyMapping.CancelKey)) return;

    StateManager.SwitchState(TitleState.Main);
  }

  // signals

  public override void Subscribe() { }

  public override void Unsubscribe() { }

  // init

  public override void Init(TitleStateManager stateManager) {
    StateManager = stateManager;
    Subscribe();
  }

}
