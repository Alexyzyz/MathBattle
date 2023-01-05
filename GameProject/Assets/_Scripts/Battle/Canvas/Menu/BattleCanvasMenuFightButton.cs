using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCanvasMenuFightButton : BattleCanvasMenuListButtonBase
{

  // signals

  public override void Subscribe() {
    BattleCanvasMenuFightList.OnHoverOption += HandleOnHover;
  }

  public override void Unsubscribe() {
    BattleCanvasMenuFightList.OnHoverOption -= HandleOnHover;
  }

  // base methods

  private void OnDestroy() {
    Unsubscribe();
  }

}
