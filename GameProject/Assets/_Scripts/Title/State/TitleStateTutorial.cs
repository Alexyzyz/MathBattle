using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleStateTutorial : TitleStateBase
{

  // state methods

  public override void EnterState<T>(T data) {
    TitleCanvasTutorialList.Instance.Activate();
  }

  public override void LeaveState(TitleState outgoingState) {
    TitleCanvasTutorialList.Instance.Deactivate();

    TitleCanvasScreenContainer.Instance.SwitchScreen(TitleState.Tutorial, outgoingState);
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
