using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateEnemyTurnParam
{
  public bool isContinuation;
}

public class BattleStateEnemyTurn : BattleStateBase
{

  private List<BattleUnitEnemy> enemyQueue = new List<BattleUnitEnemy>();

  public static DelegateVoid OnEnemyTurn { get; set; }

  // state methods

  public override void EnterState<T>(T data) {
    BattleStateEnemyTurnParam param = data as BattleStateEnemyTurnParam;

    BattleController.Instance.IsEnemyTurn = true;

    // don't reset the list if it's a continuation of a previous enemy turn state
    if (param != null && param.isContinuation) {
      MoveEnemyUnit();
      return;
    }

    enemyQueue = new List<BattleUnitEnemy>(BattleUnitManager.Instance.EnemyUnitList);

    BattleTileSelection.Instance.ToggleEnableSelectTile(false);

    BattleCanvasMenuHeader.Instance.Hide();
    BattleCanvasMenuMainContainer.Instance.ToggleShow(false);
    BattleCanvasUnitInfo.Instance.ToggleShow(false);

    BattleCanvasInfoBanner.Instance.Show("Saatnya giliran lawan!", 2);

    // this is to force the meanu header to reset its parent
    OnEnemyTurn?.Invoke();

    UtilCoroutine.Instance.StartUtilCoroutine(HandleDelayBeforeMovingEnemyUnits());
    
  }

  public override void LeaveState(BattleState outgoingState) {
    
  }

  public override void UpdateState() {
    
  }

  // public methods

  public void MoveEnemyUnit() {
    if (enemyQueue.Count < 1) {
      BattleController.Instance.EndTurn();
      return;
    }
    BattleUnitEnemy enemy = enemyQueue[0];
    enemyQueue.RemoveAt(0);
    enemy.HandleMove();
  }

  // methods

  private IEnumerator HandleDelayBeforeMovingEnemyUnits() {
    yield return new WaitForSeconds(2.5f);
    MoveEnemyUnit();
  }

  // signals

  public override void Subscribe() {

  }

  public override void Unsubscribe() {

  }

  // init

  public override void Init(BattleStateManager stateManager) {

  }

}
