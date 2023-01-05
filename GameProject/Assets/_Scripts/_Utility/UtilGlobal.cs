using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilGlobal
{
  
  /// <summary>
  /// Gets a random item from a list.
  /// </summary>
  public static T GetRandomItem<T>(List<T> list) {
    int index = UnityEngine.Random.Range(0, list.Count);
    return list[index];
  }

}

// global delegates

public delegate void DelegateVoid();
public delegate void DelegateBool(bool param);
public delegate void DelegateInt(int param);

public delegate void DelegateBattleState(BattleState param);
public delegate void DelegateTile(BattleTile param);

// global structs

[Serializable]
public class Item {
  public ItemScriptable data;
  public int qty;
}
