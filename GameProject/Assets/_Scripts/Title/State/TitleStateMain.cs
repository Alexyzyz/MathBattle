using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleStateMain : TitleStateBase
{

  // state methods

  public override void EnterState<T>(T data) {
    TitleCanvasMainList.Instance.IsActive = true;
  }

  public override void LeaveState(TitleState outgoingState) {
    TitleCanvasMainList.Instance.IsActive = false;

    TitleCanvasScreenContainer.Instance.SwitchScreen(TitleState.Main, outgoingState);
  }

  public override void UpdateState() { }

  // signals

  public override void Subscribe() { }

  public override void Unsubscribe() { }

  // init

  public override void Init(TitleStateManager stateManager) {
    StateManager = stateManager;

    CanvasSceneTransition.Instance.PlayTransition(CoverTransition.Center, CoverTransition.Left);

    Subscribe();
  }

}
