using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCanvasMenuItemButton : BattleCanvasMenuListButtonBase
{

  // signals

  public override void Subscribe() {
    BattleCanvasMenuItemList.OnHoverOption += HandleOnHover;
  }

  public override void Unsubscribe() {
    BattleCanvasMenuItemList.OnHoverOption -= HandleOnHover;
  }

  // base methods

  private void OnDestroy() {
    Unsubscribe();
  }

}
