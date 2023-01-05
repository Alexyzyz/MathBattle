using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCanvasMenuItemList : BattleCanvasMenuListBase<Item>
{

  // methods

  protected override bool IsGrayedOut(Item data) {
    return false;
  }

  // singleton

  public static BattleCanvasMenuItemList Instance { get; private set; }

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
