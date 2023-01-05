using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvasMenuBattleInfo : Singleton<BattleCanvasMenuBattleInfo>
{

  private const string ACCURACY_PERCENTAGE_TEXT = "{0:0.0}%";
  private const string ACCURACY_RATIO_TEXT = "{0} / {1}  jawaban benar";

  private RectTransform myRect;
  private Text myAccuracyPercText;
  private Text myAccuracyRatioText;
  private Text myTurnText;
  private Text myDefeatCountText;

  // public methods

  public void SetAccuracy(int correct, int total) {
    float perc = 100 * (float)correct / total;
    myAccuracyRatioText.text = string.Format(ACCURACY_RATIO_TEXT, correct, total);
    myAccuracyPercText.text = string.Format(ACCURACY_PERCENTAGE_TEXT, perc);
  }

  public void SetTurn(int turn) {
    myTurnText.text = turn.ToString();
  }

  public void SetDefeatedCount(int count) {
    myDefeatCountText.text = count.ToString();
  }

  public void Show() => ToggleShow(true);

  public void Hide() => ToggleShow(false);

  // methods

  private void ToggleShow(bool show) => UtilCoroutine.Instance.StartCoroutine(this, ref animToggleShow, AnimateToggleShown(show));

  // init

  private void Init() {
    myRect = GetComponent<RectTransform>();
    myAccuracyPercText = transform.Find("AccuracyContainer/Info/Percent").GetComponent<Text>();
    myAccuracyRatioText = transform.Find("AccuracyContainer/ExtraInfo/Text").GetComponent<Text>();
    myTurnText = transform.Find("OtherContainer/TurnInfo/Count").GetComponent<Text>();
    myDefeatCountText = transform.Find("OtherContainer/DefeatInfo/Count").GetComponent<Text>();

    myRect.anchoredPosition = AnimState.Hidden.pos;
  }

  // base methods

  protected override void Awake() {
    base.Awake();
    Init();
  }

  void Update() {
    
  }



  // animations

  private struct AnimStateData
  {
    public Vector2 pos;
  }

  private class AnimStateBase
  {
    public AnimStateData Shown = new AnimStateData {
      pos = new Vector2(0, 50),
    };
    public AnimStateData Hidden = new AnimStateData {
      pos = new Vector2(500, 50),
    };
  }
  private AnimStateBase AnimState = new AnimStateBase();

  private IEnumerator animToggleShow;

  private IEnumerator AnimateToggleShown(bool show) {
    float target = show ? 1 : 0;
    float val = 1 - target;

    while (Mathf.Abs(target - val) > 0.01f) {
      UpdateProps();
      val = Mathf.Lerp(val, target, 0.2f);
      yield return null;
    }
    val = target;
    UpdateProps();

    void UpdateProps() {
      myRect.anchoredPosition = Vector2.Lerp(AnimState.Hidden.pos, AnimState.Shown.pos, val);
    }
  }

}
