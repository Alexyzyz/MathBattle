using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvasUnitOverheadInfo : MonoBehaviour
{

  private const float HP_BAR_MAX_LENGTH = 200;

  private BattleUnitBase myUnit;

  private CanvasGroup myCanvasGroup;
  private Text myUnitNameText;
  private Text myUnitHPText;
  private RectTransform myHPBarRect;
  private RectTransform myHPLagBarRect;

  private int hp, hpMax;
  private float hpLag;

  // public methods

  public void SetAlpha(float alpha) => myCanvasGroup.alpha = alpha;

  public void SetHP(int hp, int hpMax) {
    this.hp = hp;
    this.hpMax = hpMax;

    SetAlpha(1);

    myUnitHPText.text = string.Format("HP {0} / {1}", hp, hpMax);

    Vector2 hpBarSize = myHPBarRect.sizeDelta;
    hpBarSize.x = (float)hp / hpMax * HP_BAR_MAX_LENGTH;
    myHPBarRect.sizeDelta = hpBarSize;

    UtilCoroutine.Instance.StartCoroutine(this, ref animHPLagBarAdjust, AnimateHPLagBarAdjust());
  }

  public void SetUnit(BattleUnitBase unit) {
    myUnit = unit;
    myUnitNameText.text = unit.IsHero ? "Kamu" : "Lawan";

    hp = unit.Stats.HP;
    hpMax = unit.Stats.maxHP;
    hpLag = hp;

    // init bar

    myUnitHPText.text = string.Format("HP {0} / {1}", hp, hpMax);

    Vector2 hpBarSize = myHPBarRect.sizeDelta;
    hpBarSize.x = (float)hp / hpMax * HP_BAR_MAX_LENGTH;

    myHPBarRect.sizeDelta = hpBarSize;
    myHPLagBarRect.sizeDelta = hpBarSize;
  }

  // init

  private void Init() {
    myCanvasGroup = GetComponent<CanvasGroup>();
    myUnitNameText = transform.Find("UnitName").GetComponent<Text>();
    myUnitHPText = transform.Find("HPText").GetComponent<Text>();
    myHPBarRect = transform.Find("HPBar/RemainingBar").GetComponent<RectTransform>();
    myHPLagBarRect = transform.Find("HPBar/RemainingLagBar").GetComponent<RectTransform>();
  }

  // base methods

  private void Awake() {
    Init();
  }

  // animations

  private IEnumerator animHPLagBarAdjust;

  private IEnumerator AnimateHPLagBarAdjust() {
    float lagTick = hpLag - hp;
    while (hpLag - hp > 0) {
      hpLag -= lagTick * Time.deltaTime;
      UpdateProps();
      yield return null;
    }
    hpLag = hp;
    UpdateProps();

    yield return new WaitForSeconds(1);
    
    SetAlpha(0);
    
    myUnit.IsCurrentlyBeingAttacked = false;

    if (hp == 0) {
      // play defeated animation
      myUnit.StartDefeatedAnimation();
    }

    void UpdateProps() {
      Vector2 hpLagBarSize = myHPLagBarRect.sizeDelta;
      hpLagBarSize.x = hpLag / hpMax * HP_BAR_MAX_LENGTH;
      myHPLagBarRect.sizeDelta = hpLagBarSize;
    }
  }

}
