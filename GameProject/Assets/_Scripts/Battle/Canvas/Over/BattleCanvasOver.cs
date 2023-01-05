using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvasOver : Singleton<BattleCanvasOver>
{

  private const string ACCURACY_TEXT = "{0:0.0}%";
  private const string ANSWER_COUNT_TEXT = "{0} / {1} / {2}";
  private const string ANSWER_TIME_TEXT = "{0:0.000}s";

  private CanvasGroup myGroup;
  private CanvasGroup myBackGroup;

  private RectTransform myBorderTop;
  private RectTransform myBorderBottom;

  private RectTransform myResultScreenRect;
  private CanvasGroup myResultScreenGroup;

  private Text resultTurnCountText;
  private Text resultDefeatCountText;

  private Text resultAccuracyText;
  private Text resultAnswerCountText;
  private Text resultAnswerTimeText;

  // public methods

  public void Show() {
    int correct = BattleController.Instance.CorrectAnswers;
    int total = BattleController.Instance.TotalAnswers;
    float perc = total == 0 ? 0 : 100 * (float)correct / total;

    resultTurnCountText.text = BattleController.Instance.Turn.ToString();
    resultDefeatCountText.text = BattleController.Instance.DefeatCount.ToString();
    
    resultAccuracyText.text = total > 0 ? string.Format(ACCURACY_TEXT, perc) : "—";
    resultAnswerCountText.text = string.Format(ANSWER_COUNT_TEXT, correct, total - correct, total);
    resultAnswerTimeText.text = total > 0 ? string.Format(ANSWER_TIME_TEXT, BattleController.Instance.AvgAnswerTime) : "—";

    StartCoroutine(AnimateShow());
  }

  // init

  private void Init() {
    myGroup = GetComponent<CanvasGroup>();
    myBackGroup = transform.Find("Back").GetComponent<CanvasGroup>();

    myBorderTop = transform.Find("BorderTop").GetComponent<RectTransform>();
    myBorderBottom = transform.Find("BorderBottom").GetComponent<RectTransform>();

    myResultScreenRect = transform.Find("ResultScreen").GetComponent<RectTransform>();
    myResultScreenGroup = transform.Find("ResultScreen").GetComponent<CanvasGroup>();

    resultTurnCountText = transform.Find("ResultScreen/ResultContainer/TurnCount/Right").GetComponent<Text>();
    resultDefeatCountText = transform.Find("ResultScreen/ResultContainer/DefeatCount/Right").GetComponent<Text>();

    resultAccuracyText = transform.Find("ResultScreen/ResultContainer/Accuracy/Right").GetComponent<Text>();
    resultAnswerCountText = transform.Find("ResultScreen/ResultContainer/AnswerCount/Right").GetComponent<Text>();
    resultAnswerTimeText = transform.Find("ResultScreen/ResultContainer/AnswerTime/Right").GetComponent<Text>();

    myBackGroup.alpha = 0;
    myResultScreenGroup.alpha = 0;
    myResultScreenRect.anchoredPosition = new Vector2(0, -70);
  }

  // base methods

  protected override void Awake() {
    base.Awake();
    Init();
  }

  // animations

  private IEnumerator AnimateShow() {
    myGroup.alpha = 1;

    float borderX = myBorderTop.anchoredPosition.x;

    float val = 0;

    while (val < 0.99f) {
      UpdateProps();
      val = Mathf.Lerp(val, 1, 0.4f);
      yield return null;
    }
    val = 1;
    UpdateProps();

    void UpdateProps() {
      myBackGroup.alpha = Mathf.Lerp(0, 1, val);

      myBorderTop.anchoredPosition = Vector2.Lerp(new Vector2(borderX, 434), new Vector2(borderX, 334), val);
      myBorderBottom.anchoredPosition = Vector2.Lerp(new Vector2(borderX, -434), new Vector2(borderX, -334), val);

      myResultScreenGroup.alpha = Mathf.Lerp(0, 1, val);
      myResultScreenRect.anchoredPosition = Vector2.Lerp(new Vector2(0, -70), Vector2.zero, val);
    }
  }

}
