using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitHero : BattleUnitBase
{

  public override bool IsHero { get; } = true;

  // public methods

  public override void HandleDefeat() {
    BattleUnitManager.Instance.RemoveUnit(this);
    BattleCanvasUnitOverheadContainer.Instance.Remove(Overhead);
  }

  // get set

  public override int HP {
    get { return Stats.HP; }
    set {
      int newValue = Mathf.Clamp(value, 0, Stats.maxHP);
      Stats.HP = newValue;
      BattleCanvasUnitInfo.Instance.SetHP(newValue);
    }
  }

  public override int AP {
    get { return Stats.AP; }
    set {
      int newValue = Mathf.Clamp(value, 0, Stats.maxAP);
      Stats.AP = newValue;
      BattleCanvasUnitInfo.Instance.SetAP(newValue);
    }
  }

}
