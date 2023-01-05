using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCanvasMenuFightList : BattleCanvasMenuListBase<SkillScriptable>, SubscriberInterface
{

  // methods

  protected override bool IsGrayedOut(SkillScriptable data) {
    return SelectedHero.AP < data.APCost;
  }

  // util

  private BattleUnitBase SelectedHero {
    get {
      return BattleUnitManager.Instance.SelectedHero;
    }
  }

  // singleton

  public static BattleCanvasMenuFightList Instance { get; private set; }

  protected virtual void OnApplicationQuit() {
    Instance = null;
    Destroy(gameObject);
  }

  protected override void Awake() {
    if (Instance != null) {
      Destroy(gameObject);
      return;
    }
    Instance = this;
    base.Awake();
  }

}
