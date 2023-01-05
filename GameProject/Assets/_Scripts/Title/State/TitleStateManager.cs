using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TitleState
{
  Start,

  Main,
  Tutorial,
  History,
  
  Quit
}

public class TitleStateManager : Singleton<TitleStateManager>
{

  public TitleState CurrentState { get; private set; }
  private TitleStateBase currentState;

  private Dictionary<TitleState, TitleStateBase> States = new Dictionary<TitleState, TitleStateBase> {
    { TitleState.Main, new TitleStateMain() },
    { TitleState.Tutorial, new TitleStateTutorial() },
    { TitleState.History, new TitleStateHistory() },
  };

  // public methods

  public void SwitchState(TitleState newState) {
    HandleSwitchState(newState);
    currentState.EnterState((object)null);
  }

  public void SwitchState<T>(TitleState newState, T data) {
    HandleSwitchState(newState);
    currentState.EnterState(data);
  }

  // methods

  private void HandleSwitchState(TitleState newState) {
    currentState.IsActive = false;
    currentState.LeaveState(newState);

    CurrentState = newState;
    currentState = States[newState];
    currentState.IsActive = true;
  }

  private void UnsubscribeAll() {
    foreach (KeyValuePair<TitleState, TitleStateBase> entry in States) {

    }
  }

  // base methods

  void Start() {
    foreach (KeyValuePair<TitleState, TitleStateBase> entry in States) {
      entry.Value.Init(this);
    }

    CurrentState = TitleState.Main;
    currentState = States[TitleState.Main];
    currentState.IsActive = true;
    currentState.EnterState((object)null);
  }

  // base methods

  void Update() {
    currentState.UpdateState();
  }

  private void OnDestroy() {
    UnsubscribeAll();
  }

}
