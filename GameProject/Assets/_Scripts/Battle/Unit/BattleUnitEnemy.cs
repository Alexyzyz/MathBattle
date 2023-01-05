using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleUnitEnemy : BattleUnitBase
{

  private struct TileDistanceData
  {
    public BattleTile tile;
    public float distToPlayer;
  }

  private class DijkstraNode
  {
    public BattleTile tile;
    public DijkstraNode prev;
    public float dist;
  }

  private BattleUnitBase targetedHero;

  public override bool IsHero { get; } = false;

  // public method

  public override void HandleDefeat() {
    BattleUnitManager.Instance.RemoveUnit(this);
    BattleCanvasUnitOverheadContainer.Instance.Remove(Overhead);

    BattleController.Instance.HandleEnemyDefeat();
  }

  public void HandleMove() {
    List<Vector2Int> stepList;

    // look for heroes in the field
    List<BattleUnitHero> heroList = BattleUnitManager.Instance.HeroUnitList;

    BattleUnitBase closestHero = heroList[0];
    float closestHeroDist = Vector3.Distance(closestHero.WorldPos, WorldPos);

    foreach (BattleUnitBase hero in heroList) {
      float dist = Vector3.Distance(hero.WorldPos, WorldPos);
      if (dist < closestHeroDist) {
        closestHero = hero;
        closestHeroDist = dist;
      }
    }
    targetedHero = closestHero;

    stepList = GetDijkstraRoute((item) => item.tile == targetedHero.Tile);

    if (stepList.Count > 1) {
      // target hero is reachable
      StartCoroutine(EnemyTurnMoveTo(stepList));
      return;
    }

    // move to a random tile instead

    BattleTile randomTile = BattleTileManager.Instance.TileList.Find((item) => item.Unit == null);

    stepList = GetDijkstraRoute((item) => item.tile == randomTile);
    StartCoroutine(EnemyTurnMoveTo(stepList));
  }

  // methods

  private IEnumerator EnemyTurnMoveTo(List<Vector2Int> stepList) {
    Overhead.Hint.Show("Giliran lawan ini.");

    if (targetedHero != null && targetedHero.Tile.GetNeighbors().Contains(Tile)) {
      // attack immediately if hero is right next to this unit
      Overhead.Hint.Hide();
      Overhead.AttackSignal.Play(AttackHero);
      yield break;
    }

    while (stepList.Count > 0 && AP > 0) {
      int lastIndex = stepList.Count - 1;
      Vector2Int nextCoord = stepList[lastIndex];
      stepList.RemoveAt(lastIndex);

      if (MoveToCoord(nextCoord)) {
        AP--;
      }
      yield return new WaitForSeconds(0.4f);
    }

    Overhead.Hint.Hide();

    // check if a hero is attackable

    if (targetedHero != null && targetedHero.Tile.GetNeighbors().Contains(Tile)) {
      Overhead.AttackSignal.Play(AttackHero);
    }
    else {
      BattleController.Instance.HandleEndAttack();
    }
  }

  private void AttackHero() {
    List<BattleUnitBase> targetList = new List<BattleUnitBase>();
    targetList.Add(targetedHero);

    AttackData attackData = new AttackData {
      attacker = this,
      targetList = targetList,
      attackerSkill = null,
    };
    BattleController.Instance.SetAttackData(attackData);
    BattleController.Instance.SetMathData();

    BattleStateManager.Instance.SwitchState(BattleState.Math);
  }

  // dijkstra wall of code

  private List<Vector2Int> GetDijkstraRoute(System.Predicate<DijkstraNode> match) {
    List<BattleTile> tileList = BattleTileManager.Instance.TileList;
    List<DijkstraNode> queue = new List<DijkstraNode>();

    foreach (BattleTile tile in tileList) {
      float initialDist = tile == Tile ? 0 : Mathf.Infinity;
      DijkstraNode newNode = new DijkstraNode {
        tile = tile,
        prev = null,
        dist = initialDist,
      };
      queue.Add(newNode);
    }

    DijkstraNode target = queue.Find(match);

    while (queue.Count > 0) {
      DijkstraNode leastDistNode = queue.Aggregate((a, b) => a.dist < b.dist ? a : b);
      queue.Remove(leastDistNode);

      List<BattleTile> tileNeighbors = leastDistNode.tile.GetNeighbors();
      tileNeighbors.RemoveAll((item) => item.Unit != null && !item.Unit.IsHero);
      List<DijkstraNode> neighborsStillInQueue = queue.Where((item) => tileNeighbors.Contains(item.tile)).ToList();

      foreach (DijkstraNode neighbor in neighborsStillInQueue) {
        float alt = leastDistNode.dist + 1;
        if (alt < neighbor.dist) {
          neighbor.dist = alt;
          neighbor.prev = leastDistNode;
        }
      }
    }

    List<Vector2Int> stepList = new List<Vector2Int>();
    DijkstraNode temp = target;
    while (temp != null) {
      BattleTile tempTile = temp.tile;
      if (!tempTile.Unit || !tempTile.Unit.IsHero) {
        stepList.Add(tempTile.Coord);
      }
      temp = temp.prev;
    }

    return stepList;
  }

}
