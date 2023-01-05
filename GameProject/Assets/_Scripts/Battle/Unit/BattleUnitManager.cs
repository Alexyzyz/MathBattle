using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitManager : Singleton<BattleUnitManager>
{

  [SerializeField] private BaseUnitScriptable testUnitData;

  [SerializeField] private BattleUnitHero heroUnitPrefab;
  [SerializeField] private BattleUnitEnemy enemyUnitPrefab;
  [SerializeField] private Transform unitParent;

  public BattleUnitBase SelectedHero { get; set; }

  public List<BattleUnitBase> UnitList {
    get {
      List<BattleUnitBase> unitList = new List<BattleUnitBase>();
      HeroUnitList.ForEach((item) => unitList.Add(item));
      EnemyUnitList.ForEach((item) => unitList.Add(item));
      return unitList;
    }
  }

  public List<BattleUnitHero> HeroUnitList { get; private set; }
  public List<BattleUnitEnemy> EnemyUnitList { get; private set; }

  private DelegateVoid damageHitBacklog { get; set; }

  // public methods

  public void SpawnNewEnemies() {
    while (EnemyUnitList.Count < BattleController.Instance.DiffData.minEnemyCount) {
      List<BattleTile> emptyTiles = BattleTileManager.Instance.TileList.FindAll((item) => item.Unit == null);
      BattleTile chosenTile = UtilGlobal.GetRandomItem(emptyTiles);
      SpawnEnemy(chosenTile.Coord, new UnitStats(BattleController.Instance.DiffData.enemyMaxHP, 4, 1));
    }
  }

  public void RemoveUnit(BattleUnitHero heroUnit) {
    BattleTile tile = heroUnit.Tile;
    tile.Unit = null;
    tile.UpdateState();

    HeroUnitList.Remove(heroUnit);
  }

  public void RemoveUnit(BattleUnitEnemy enemyUnit) {
    BattleTile tile = enemyUnit.Tile;
    tile.Unit = null;
    tile.UpdateState();

    EnemyUnitList.Remove(enemyUnit);
  }

  public void SetAllTargetUnitToDefend() => AttackData.targetList.ForEach((unit) => unit.SetToDefend());

  public void HideAllUnitOverhead() => UnitList.ForEach((unit) => unit.SetOverheadInfoAlpha(0));

  public void HandleTargetUnitDoneDefending() {
    if (AttackData.targetList.Count > 0) {
      AttackData.targetList.RemoveAt(0);
    }
    if (AttackData.targetList.Count > 0) return;
    BattleController.Instance.HandleEndAttack();
  }

  public void BacklogDamageHit(int damageMin, int damageMax, float bonus) {
    void BacklogDamageHitInstance() {
      foreach (BattleUnitBase unit in AttackData.targetList) {
        int damage = (int)(Random.Range(damageMin, damageMax + 1) * bonus);

        unit.Overhead.DamageQueue.CreateDamageText(damage);
        unit.TakeDamage(damage);
      }
    }
    damageHitBacklog += BacklogDamageHitInstance;
  }

  public void CallAllDamageHit() {
    damageHitBacklog?.Invoke();
    damageHitBacklog = null;
  }

  public void SpawnHero(Vector2Int startCoord, UnitStats baseStats) {
    BattleUnitHero spawnedUnit = Instantiate(
      heroUnitPrefab,
      Vector3.zero,
      heroUnitPrefab.transform.rotation,
      unitParent);

    spawnedUnit.MoveToCoord(startCoord, false);
    spawnedUnit.SetStats(baseStats);

    spawnedUnit.SkillList = testUnitData.startingSkills;
    spawnedUnit.ItemList = CopyStartingItemList(testUnitData.startingItems);

    HeroUnitList.Add(spawnedUnit);
  }

  public void SpawnEnemy(Vector2Int startCoord, UnitStats baseStats) {
    BattleUnitEnemy spawnedUnit = Instantiate(
      enemyUnitPrefab,
      Vector3.zero,
      enemyUnitPrefab.transform.rotation,
      unitParent);

    spawnedUnit.MoveToCoord(startCoord, false);
    spawnedUnit.SetStats(baseStats);

    spawnedUnit.SkillList = testUnitData.startingSkills;
    spawnedUnit.ItemList = CopyStartingItemList(testUnitData.startingItems);

    EnemyUnitList.Add(spawnedUnit);
  }

  // util

  private List<Item> CopyStartingItemList(List<Item> from) {
    List<Item> to = new List<Item>();
    from.ForEach((item) => to.Add(
      new Item {
        data = item.data,
        qty = item.qty,
      }
    ));
    return to;
  }

  private AttackData AttackData {
    get {
      return BattleController.Instance.AttackData;
    }
  }

  // base methods

  protected override void Awake() {
    base.Awake();

    HeroUnitList = new List<BattleUnitHero>();
    EnemyUnitList = new List<BattleUnitEnemy>();
  }

}
