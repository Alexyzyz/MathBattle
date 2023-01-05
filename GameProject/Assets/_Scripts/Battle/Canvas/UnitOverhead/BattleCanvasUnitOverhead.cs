using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvasUnitOverhead : MonoBehaviour
{

  private BattleUnitBase myUnit;

  [HideInInspector] public BattleCanvasUnitOverheadInfo Info;
  [HideInInspector] public BattleCanvasUnitOverheadDamageQueue DamageQueue;
  [HideInInspector] public BattleCanvasUnitOverheadHint Hint;
  [HideInInspector] public BattleCanvasUnitOverheadAttackSignal AttackSignal;

  private float UNIT_Y_OFFSET = 2.5f;

  // public methods

  public void SetUnit(BattleUnitBase unit) {
    myUnit = unit;
    Info.SetUnit(unit);
  }

  // methods

  private void UpdateOverheadPos() {
    Vector3 unitOverheadPos = Camera.main.WorldToScreenPoint(
      myUnit.WorldPos - new Vector3(0, -UNIT_Y_OFFSET, 0));
    transform.position = unitOverheadPos;
  }

  // init

  private void Init() {
    Info = transform.Find("Info").GetComponent<BattleCanvasUnitOverheadInfo>();
    DamageQueue = transform.Find("DamageQueue").GetComponent<BattleCanvasUnitOverheadDamageQueue>();
    Hint = transform.Find("Hint").GetComponent<BattleCanvasUnitOverheadHint>();
    AttackSignal = transform.Find("AttackSignal").GetComponent<BattleCanvasUnitOverheadAttackSignal>();
  }

  // base methods

  private void Awake() {
    Init();
  }

  private void Update() {
    UpdateOverheadPos();
  }

}
