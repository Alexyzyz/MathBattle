using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCanvasEndOptionDescription : BattleCanvasSideDescription
{

  // singleton

  public static BattleCanvasEndOptionDescription Instance { get; private set; }

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
