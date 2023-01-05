using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitBaseRig : MonoBehaviour
{

  private BattleUnitBase myUnit;

  // animation events

  private void OnDamageDealt() {
    GameSoundManager.Instance.PlaySound(GameSound.BattleAttack);
    BattleUnitManager.Instance.CallAllDamageHit();
    BattleUnitManager.Instance.SetAllTargetUnitToDefend();
  }

  private void OnDamageTaken() {
    HandleDamageTakenSkillEvent();
  }

  private void OnFinishAttack() {
    OnFinishAnimation();
  }

  private void OnFinishDefend() {
    BattleUnitManager.Instance.HandleTargetUnitDoneDefending();
    OnFinishAnimation();
  }

  private void OnFinishDefeated() {
    myUnit.OnDefeatedAnimationFinish();
    OnFinishAnimation();
  }

  private void OnFinishAnimation() {
    myUnit.AlwaysFaceCamera = true;
  }

  // methods

  private void HandleDamageTakenSkillEvent() {
    if (!AttackData.isSuccessful) return;

    SkillScriptable skill = AttackData.attackerSkill;
    if (skill == null) return;

    string skillId = skill.id;
    BattleUnitBase attacker = AttackData.attacker;

    if (skillId == "forward single knockback") {

      // get the neighbor index for the knockback direction
      int knockbackTileIndex = attacker.Tile.GetNeighborIndex(myUnit.Tile.Coord);
      if (knockbackTileIndex == -1) return;

      int KNOCKBACK_DISTANCE = 3;
      BattleTile prevKnockbackTile = myUnit.Tile;

      int i = KNOCKBACK_DISTANCE;
      while (i > 0) {
        BattleTile potentialNextKnockbackTile = prevKnockbackTile.GetNeighbor(knockbackTileIndex);
        if (potentialNextKnockbackTile == null || potentialNextKnockbackTile.Unit != null) break;
        prevKnockbackTile = potentialNextKnockbackTile;
        i--;
      }

      if (i < KNOCKBACK_DISTANCE) {
        myUnit.MoveToCoord(prevKnockbackTile.Coord);
      }
    } else
    if (skillId == "switcheroo") {
      // switch places with the attacker
      BattleTileSelection.Instance.SelectedTile = myUnit.Tile;
      myUnit.SwitchToCoord(attacker.Coord);
    } else
    if (skillId == "steal") {
      // randomly get an item
      ItemScriptable stolenItem = UtilGlobal.GetRandomItem(BattleController.Instance.StealableItemList);

      if (!AttackData.isCrit && Random.Range(0, 3) == 0) return;

      BattleCanvasInfoBanner.Instance.Show("Kamu berhasil mengambil sesuatu dari lawan!", 3);

      Item matchingItem = attacker.ItemList.Find((item) => item.data.idName == stolenItem.idName);
      if (matchingItem == null) {
        // new item
        attacker.ItemList.Add(new Item {
          data = stolenItem,
          qty = 1,
        });
      } else {
        // increase item qty
        matchingItem.qty++;
      }
    }

  }

  // init

  private void Init() {
    myUnit = transform.parent.GetComponent<BattleUnitBase>();
  }

  // util

  private AttackData AttackData {
    get {
      return BattleController.Instance.AttackData;
    }
  }

  // base method

  private void Awake() {
    Init();
  }

}
