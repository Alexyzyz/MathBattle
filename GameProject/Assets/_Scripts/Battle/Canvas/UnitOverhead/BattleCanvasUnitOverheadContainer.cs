using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCanvasUnitOverheadContainer : Singleton<BattleCanvasUnitOverheadContainer>
{

  private List<BattleCanvasUnitOverhead> children = new List<BattleCanvasUnitOverhead>();

  // public methods

  public void Add(BattleCanvasUnitOverhead child) {
    children.Add(child);
  }

  public void Remove(BattleCanvasUnitOverhead child) {
    children.Remove(child);
  }

  // methods

  private void SortChildren() {
    children.Sort((a, b) => a.transform.position.z <= b.transform.position.z ? 1 : -1);
    for (int i = 0; i < children.Count - 1; i++) {
      children[i].transform.SetSiblingIndex(i);
    }
  }

  // base methods

  private void Update() {
    SortChildren();
  }

}
