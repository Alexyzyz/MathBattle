using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCanvasMenuEndOptionButton : BattleCanvasMenuListButtonBase
{

  // signals

  public override void Subscribe() {
    BattleCanvasMenuEndOptionList.OnHoverOption += HandleOnHover;
  }

  public override void Unsubscribe() {
    BattleCanvasMenuEndOptionList.OnHoverOption -= HandleOnHover;
  }

  // base methods

  private void OnDestroy() {
    Unsubscribe();
  }

}
