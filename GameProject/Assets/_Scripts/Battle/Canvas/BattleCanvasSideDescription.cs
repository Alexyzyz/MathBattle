using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvasSideDescription : MonoBehaviour
{

  private const string AP_COST_TEXT = "<color=#d3ff00>Biaya AP</color>  {0}";
  private const string DAMAGE_TEXT = "<color=#bfbfbf>DMG</color>  {0}";
  private const string CRIT_BONUS_TEXT = "<color=#bfbfbf>CRIT Bonus</color>  {0}%";

  private const float DESC_SINGLE_POS_Y = -39.5f;
  private const float DESC_DEFAULT_POS_Y = -52;


  private RectTransform myRect;

  private Text apCostText;
  private Text damageText;
  private Text critBonusText;
  private Text descText;

  // public methods

  public void Show() => PlayAnim(AnimState.Shown);
  public void Hide() => PlayAnim(AnimState.Hidden);
  public void Shrink() => PlayAnim(AnimState.Shrunk);

  public void SetInfo(string desc) {
    apCostText.text = "";
    damageText.text = "";
    critBonusText.text = "";
    descText.text = desc;

    Vector2 descTextPos = descText.rectTransform.anchoredPosition;
    descTextPos.y = DESC_SINGLE_POS_Y;
    descText.rectTransform.anchoredPosition = descTextPos;
  }

  public void SetInfo(int apCost, string desc) {
    apCostText.text = string.Format(AP_COST_TEXT, apCost.ToString());
    damageText.text = "";
    critBonusText.text = "";
    descText.text = desc;

    Vector2 descTextPos = descText.rectTransform.anchoredPosition;
    descTextPos.y = DESC_DEFAULT_POS_Y;
    descText.rectTransform.anchoredPosition = descTextPos;
  }

  public void SetInfo(int apCost, string damage, float critBonus, string desc) {
    apCostText.text = string.Format(AP_COST_TEXT, apCost.ToString());
    damageText.text = string.Format(DAMAGE_TEXT, damage.ToString());
    critBonusText.text = string.Format(CRIT_BONUS_TEXT, Mathf.Floor(critBonus * 100));
    descText.text = desc;

    Vector2 descTextPos = descText.rectTransform.anchoredPosition;
    descTextPos.y = DESC_DEFAULT_POS_Y;
    descText.rectTransform.anchoredPosition = descTextPos;
  }

  // methods

  private void PlayAnim(AnimStateData state) => UtilCoroutine.Instance.StartCoroutine(this, ref animToggleShow, AnimateToggleShow(state));

  // init

  private void Init() {
    myRect = GetComponent<RectTransform>();
    apCostText = transform.Find("Cost").GetComponent<Text>();
    damageText = transform.Find("Damage").GetComponent<Text>();
    critBonusText = transform.Find("CritBonus").GetComponent<Text>();
    descText = transform.Find("Desc").GetComponent<Text>();

    myRect.anchoredPosition = AnimState.Hidden.pos;
  }

  // base methods

  protected virtual void Awake() {
    Init();
  }



  // animations

  private struct AnimStateData
  {
    public Vector2 pos;
    public Vector2 scale;
  }

  private class AnimStateBase
  {
    public AnimStateData Hidden = new AnimStateData {
      pos = new Vector2(580.5f, 80),
      scale = Vector2.one,
    };
    public AnimStateData Shown = new AnimStateData {
      pos = new Vector2(0, 80),
      scale = Vector2.one,
    };
    public AnimStateData Shrunk = new AnimStateData {
      pos = new Vector2(0, 80),
      scale = 0.7f * Vector2.one,
    };
  }
  private AnimStateBase AnimState = new AnimStateBase();

  private IEnumerator animToggleShow;

  private IEnumerator AnimateToggleShow(AnimStateData targetAnimState) {
    Vector2 currPos = myRect.anchoredPosition;
    Vector2 currScale = myRect.localScale;

    float value = 0;
    while (value < 0.99f) {
      UpdateProps();
      value = Mathf.Lerp(value, 1, 0.2f);
      yield return null;
    }
    value = 1;
    UpdateProps();

    void UpdateProps() {
      myRect.anchoredPosition = Vector2.Lerp(currPos, targetAnimState.pos, value);
      myRect.localScale = Vector2.Lerp(currScale, targetAnimState.scale, value);
    }
  }

}
