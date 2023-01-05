using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCanvasUnitOverheadDamageQueue : MonoBehaviour
{

  [SerializeField] private BattleCanvasDamageText damageTextPrefab;

  private RectTransform myRect;

  // methods

  public void CreateDamageText(int damage) {
    BattleCanvasDamageText damageText = Instantiate(
      damageTextPrefab,
      Vector3.zero,
      Quaternion.identity,
      myRect);
    damageText.SetDamageText(damage);
  }

  // init

  private void Init() {
    myRect = GetComponent<RectTransform>();
  }

  // base methods

  private void Awake() {
    Init();
  }

}
