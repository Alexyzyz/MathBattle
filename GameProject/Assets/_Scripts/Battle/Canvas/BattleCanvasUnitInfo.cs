using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvasUnitInfo : Singleton<BattleCanvasUnitInfo>
{

  private struct UnitInfoData
  {
    public CanvasGroup container;
    public Text valueText;
    public Text maxValueText;

    public int value;
    public int maxValue;
  }

  //

  private RectTransform myRect;
  private UnitInfoData myHP, myAP;

  public bool IsShown { get; private set; }

  // public methods

  public void SetValues(UnitStats stats) {
    myHP.value = stats.HP;
    myHP.maxValue = stats.maxHP;
    myHP.valueText.text = "" + stats.HP;
    myHP.maxValueText.text = "/ " + stats.maxHP;

    myAP.value = stats.AP;
    myAP.maxValue = stats.maxAP;
    myAP.valueText.text = "" + stats.AP;
    myAP.maxValueText.text = "/ " + stats.maxAP;
  }

  public void SetHP(int hp) {
    UtilCoroutine.Instance.StartCoroutine(this, ref animHPRoll, AnimateHPRoll(myHP.value, hp));
    myHP.value = hp;
  }

  public void SetAP(int ap) {
    UtilCoroutine.Instance.StartCoroutine(this, ref animAPRoll, AnimateAPRoll(myAP.value, ap));
    myAP.value = ap;
  }

  public void SetAlphas(float hpAlpha, float apAlpha, float mpAlpha) {
    myHP.container.alpha = hpAlpha;
    myAP.container.alpha = apAlpha;
  }

  public void ToggleShow(bool state) {
    UtilCoroutine.Instance.StartCoroutine(this, ref animToggleShow, AnimateToggleShow(state));
  }

  // init

  private void Init() {
    myRect = GetComponent<RectTransform>();

    myHP.container = transform.Find("HP").GetComponent<CanvasGroup>();
    myHP.valueText = transform.Find("HP/Value").GetComponent<Text>();
    myHP.maxValueText = transform.Find("HP/MaxValue").GetComponent<Text>();

    myAP.container = transform.Find("AP").GetComponent<CanvasGroup>();
    myAP.valueText = transform.Find("AP/Value").GetComponent<Text>();
    myAP.maxValueText = transform.Find("AP/MaxValue").GetComponent<Text>();

    showPos = myRect.anchoredPosition;
    hidePos = showPos - new Vector2(0, 80);
    myRect.anchoredPosition = hidePos;
  }

  // base methods

  protected override void Awake() {
    base.Awake();
    Init();
  }



  // animations

  private Vector2 showPos, hidePos;

  private IEnumerator animHPRoll, animAPRoll, animMPRoll;
  private IEnumerator animToggleShow;
  
  private IEnumerator AnimateHPRoll(float currHP, float targetHP) {
    float val = 0;
    while (val < 0.99f) {
      UpdateProps();
      val = Mathf.Lerp(val, 1, 0.2f);
      yield return null;
    }
    val = 1;
    UpdateProps();

    void UpdateProps() {
      myHP.valueText.text = Mathf.Ceil(Mathf.Lerp(currHP, targetHP, val)).ToString();
    }
  }

  private IEnumerator AnimateAPRoll(float currAP, float targetAP) {
    float val = 0;
    while (val < 0.99f) {
      UpdateProps();
      val = Mathf.Lerp(val, 1, 0.2f);
      yield return null;
    }
    val = 1;
    UpdateProps();

    void UpdateProps() {
      myAP.valueText.text = Mathf.Ceil(Mathf.Lerp(currAP, targetAP, val)).ToString();
    }
  }

  private IEnumerator AnimateToggleShow(bool show) {
    Vector2 currPos = myRect.anchoredPosition;
    Vector2 targetPos = show ? showPos : hidePos;

    while (show && BattleCanvasIdleFlavorText.Instance.IsShown) yield return null;

    float value = 0;
    while (value < 0.99f) {
      UpdateProps();
      value = Mathf.Lerp(value, 1, 0.6f);
      yield return null;
    }
    value = 1;
    UpdateProps();

    IsShown = show;

    void UpdateProps() {
      myRect.anchoredPosition = Vector2.Lerp(currPos, targetPos, value);
    }
  }

}
