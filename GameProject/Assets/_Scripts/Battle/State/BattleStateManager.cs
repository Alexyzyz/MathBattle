using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
  Idle,
  MainMenu, FightMenu, FightTarget, ItemMenu, MoveMenu, EndMenu,
  EnemyTurn,
  Math, GameOver
}

public class BattleStateManager : Singleton<BattleStateManager>
{

  public BattleState CurrentState { get; private set; }
  private BattleStateBase currentState;

  private Dictionary<BattleState, BattleStateBase> States = new Dictionary<BattleState, BattleStateBase> {
    { BattleState.Idle, new BattleStateIdle() },

    { BattleState.MainMenu, new BattleStateMainMenu() },
    { BattleState.FightMenu, new BattleStateFightMenu() },
    { BattleState.FightTarget, new BattleStateFightTarget() },
    { BattleState.ItemMenu, new BattleStateItemMenu() },
    { BattleState.MoveMenu, new BattleStateMoveMenu() },
    { BattleState.EndMenu, new BattleStateEndMenu() },
    { BattleState.EnemyTurn, new BattleStateEnemyTurn() },

    { BattleState.Math, new BattleStateMath() },

    { BattleState.GameOver, new BattleStateGameOver() },
  };

  // public methods

  public void SwitchState(BattleState newState) {
    HandleSwitchState(newState);
    currentState.EnterState((object)null);
  }

  public void SwitchState<T>(BattleState newState, T data) {
    HandleSwitchState(newState);
    currentState.EnterState(data);
  }

  // methods

  private void HandleSwitchState(BattleState newState) {
    currentState.IsActive = false;
    currentState.LeaveState(newState);

    CurrentState = newState;
    currentState = States[newState];
    currentState.IsActive = true;
  }

  private void UnsubscribeAll() {
    foreach (KeyValuePair<BattleState, BattleStateBase> entry in States) {
      
    }
  }

  // base methods

  void Start() {
    foreach (KeyValuePair<BattleState, BattleStateBase> entry in States) {
      entry.Value.Init(this);
    }

    CurrentState = BattleState.Idle;
    currentState = States[BattleState.Idle];
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
