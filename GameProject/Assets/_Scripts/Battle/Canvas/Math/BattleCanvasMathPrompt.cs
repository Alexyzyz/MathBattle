using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvasMathPrompt : Singleton<BattleCanvasMathPrompt>
{

  private float TIMER_BAR_MAX_LENGTH = 400;

  private Color32 CORRECT_COLOR = new Color32(125, 217, 75, 255);
  private Color32 CRIT_COLOR = new Color32(252, 232, 3, 255);
  private Color32 INCORRECT_COLOR = new Color32(204, 61, 61, 255);
  private Color32 NEUTRAL_COLOR = new Color32(51, 50, 44, 255);

  [SerializeField] private Sprite resultIconCorrect;
  [SerializeField] private Sprite resultIconIncorrect;
  [SerializeField] private Sprite resultIconNeutral;

  private CanvasGroup myCanvasGroup;
  private CanvasGroup backCanvasGroup;
  private CanvasGroup otherCanvasGroup;

  private RectTransform questionBackRect;
  private Text questionText;
  private Text answerText;

  private Image resultIcon;
  private Animator resultIconAnimController;

  private RectTransform timerBarCritRect;
  private RectMask2D timerBarMask;

  // public methods

  public void SetAnswerCorrect(bool isCrit) {
    resultIcon.sprite = resultIconCorrect;
    resultIcon.color = isCrit ? CRIT_COLOR : CORRECT_COLOR;
    resultIconAnimController.SetTrigger("Swivel");
  }

  public void SetAnswerIncorrect() {
    resultIcon.sprite = resultIconIncorrect;
    resultIcon.color = INCORRECT_COLOR;
    resultIconAnimController.SetTrigger("Swivel");
  }

  public void SetQuestion(string text) => questionText.text = text;

  public void SetAnswer(string text) => answerText.text = text;

  public void ToggleShow(bool show) {
    if (show) {
      resultIcon.sprite = resultIconNeutral;
      resultIcon.color = NEUTRAL_COLOR;
    }

    UtilCoroutine.Instance.StartCoroutine(this, ref animToggleShow, AnimateToggleShow(show));
  }

  public void InitTimer(float critVal, float max) {
    Vector2 temp = timerBarCritRect.sizeDelta;
    temp.x = TIMER_BAR_MAX_LENGTH * (critVal / max);
    timerBarCritRect.sizeDelta = temp;
  }

  public void UpdateTimer(float val, float max) {
    timerBarMask.padding = new Vector4(0, 0, TIMER_BAR_MAX_LENGTH - TIMER_BAR_MAX_LENGTH * (val / max), 0);
  }

  // init

  private void Init() {
    myCanvasGroup = GetComponent<CanvasGroup>();
    backCanvasGroup = transform.Find("Back").GetComponent<CanvasGroup>();
    otherCanvasGroup = transform.Find("Other").GetComponent<CanvasGroup>();

    questionBackRect = transform.Find("Question").GetComponent<RectTransform>();
    questionText = transform.Find("Question/Text").GetComponent<Text>();

    resultIcon = transform.Find("Question/ResultIcon/Rig").GetComponent<Image>();
    resultIconAnimController = transform.Find("Question/ResultIcon/Rig").GetComponent<Animator>();

    timerBarMask = transform.Find("Question/Timer/Bar").GetComponent<RectMask2D>();
    timerBarCritRect = transform.Find("Question/Timer/Bar/Crit").GetComponent<RectTransform>();

    answerText = transform.Find("Other/Answer/Text").GetComponent<Text>();
  }

  // base methods

  protected override void Awake() {
    base.Awake();
    Init();
  }



  // animations

  private struct AnimStateData
  {
    public float questionBackHeight;
    public float otherAlpha;
  }

  private class AnimStateBase
  {
    public AnimStateData Hidden = new AnimStateData {
      questionBackHeight = 0,
      otherAlpha = 0,
    };
    public AnimStateData Shown = new AnimStateData {
      questionBackHeight = 130,
      otherAlpha = 1,
    };
  }
  private AnimStateBase AnimState = new AnimStateBase();

  private IEnumerator animToggleShow;

  private IEnumerator AnimateToggleShow(bool show) {
    myCanvasGroup.alpha = show ? 1 : 0;

    float target = show ? 1 : 0;
    float value = 1 - target;

    while (Mathf.Abs(value - target) > 0.01f) {
      value = Mathf.Lerp(value, target, 0.2f);
      UpdateProps();
      yield return null;
    }
    value = target;
    UpdateProps();

    void UpdateProps() {
      backCanvasGroup.alpha = Mathf.Lerp(AnimState.Hidden.otherAlpha, AnimState.Shown.otherAlpha, value);
      otherCanvasGroup.alpha = Mathf.Lerp(AnimState.Hidden.otherAlpha, AnimState.Shown.otherAlpha, value);

      float questionBackHeight = Mathf.Lerp(AnimState.Hidden.questionBackHeight, AnimState.Shown.questionBackHeight, value);
      Vector2 temp = questionBackRect.sizeDelta;
      temp.y = questionBackHeight;
      questionBackRect.sizeDelta = temp;
    }
  }

}
