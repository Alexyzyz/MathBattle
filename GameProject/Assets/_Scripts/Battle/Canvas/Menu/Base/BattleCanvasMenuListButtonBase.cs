using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvasMenuListButtonBase : MonoBehaviour, SubscriberInterface
{

  private const string QTY_TEXT = "×{0}";

  private RectTransform myArrowRect;
  private RectTransform myTextRect;

  private Image myBack;
  private Image myArrow;
  private Text myText;
  private Text myQtyText;

  private int index;
  private int listItemCount;
  private int listMaxItemsShown;

  private bool isGrayedOut;

  // public methods

  public void SetData(int index, string title, bool showQty, int qty, bool isGrayedOut, int listItemCount, int listMaxItemsShown) {
    myText.text = title;
    myQtyText.text = showQty ? string.Format(QTY_TEXT, qty) : "";

    this.index = index;
    this.isGrayedOut = isGrayedOut;
    this.listItemCount = listItemCount;
    this.listMaxItemsShown = listMaxItemsShown;
  }

  // signal methods

  protected void HandleOnHover(int hoveredIndex) {
    int viewScrollIndex = Mathf.Clamp(
      hoveredIndex,
      listMaxItemsShown / 2,
      listItemCount - 1 - listMaxItemsShown / 2
    );
    int viewScrollIndexDistance = Mathf.Abs(index - viewScrollIndex);

    float textAlpha = 0;
    float backAlpha = 0;
    if (viewScrollIndexDistance <= listMaxItemsShown / 2) {
      textAlpha = isGrayedOut ? 0.5f : 1;
      backAlpha = 1;
    }

    UtilCoroutine.Instance.StartCoroutine(this, ref animScrollFade, AnimateScrollFade(textAlpha, backAlpha));
    UtilCoroutine.Instance.StartCoroutine(this, ref animToggleHover, AnimateToggleHover(index == hoveredIndex));
  }

  // signals

  public virtual void Subscribe() { }

  public virtual void Unsubscribe() { }

  // init

  private void Init() {
    myBack = transform.Find("Back").GetComponent<Image>();

    GameObject myArrowObj = transform.Find("Arrow").gameObject;
    myArrowRect = myArrowObj.GetComponent<RectTransform>();
    myArrow = myArrowObj.GetComponent<Image>();

    GameObject myTextObj = transform.Find("Text").gameObject;
    myTextRect = myTextObj.GetComponent<RectTransform>();
    myText = myTextObj.GetComponent<Text>();

    myQtyText = transform.Find("QtyText").GetComponent<Text>();

    unhoveredPos = myTextRect.anchoredPosition;
    hoveredPos = unhoveredPos + new Vector2(30, 0);

    myBack.color = UtilColor.SetAlpha(myBack.color, 0);
    myText.color = UtilColor.SetAlpha(myText.color, 0);
    myArrow.color = UtilColor.SetAlpha(myArrow.color, 0);

    StartCoroutine(AnimateArrowSway());
  }

  // base methods

  private void Awake() {
    Init();
    Subscribe();
  }

  private void OnDestroy() {
    Unsubscribe();
  }



  // animations

  private Vector2 unhoveredPos, hoveredPos;

  private IEnumerator animToggleHover;
  private IEnumerator animScrollFade;

  private IEnumerator AnimateToggleHover(bool state) {
    Vector3 currPos = myTextRect.anchoredPosition;
    Vector3 targetPos = state ? hoveredPos : unhoveredPos;

    Color currArrowColor = myArrow.color;
    Color targetArrowColor = UtilColor.SetAlpha(myArrow.color, state ? 1 : 0);

    float value = 0;
    while (value < 0.99f) {
      UpdateProps();
      value = Mathf.Lerp(value, 1, 0.2f);
      yield return null;
    }
    value = 1;
    UpdateProps();

    void UpdateProps() {
      myTextRect.anchoredPosition = Vector3.Lerp(currPos, targetPos, value);
      myArrow.color = Color.Lerp(currArrowColor, targetArrowColor, value);
    }
  }

  private IEnumerator AnimateScrollFade(float targetTextAlpha, float targetBackAlpha) {
    Color currTextColor = myText.color;
    Color targetTextColor = UtilColor.SetAlpha(myText.color, targetTextAlpha);

    Color currBackColor = myBack.color;
    Color targetBackColor = UtilColor.SetAlpha(myBack.color, targetBackAlpha);

    float value = 0;
    while (value < 0.99f) {
      UpdateProps();
      value = Mathf.Lerp(value, 1, 0.2f);
      yield return null;
    }
    value = 1;
    UpdateProps();

    void UpdateProps() {
      Color lerpedColor = Color.Lerp(currTextColor, targetTextColor, value);

      myBack.color = Color.Lerp(currBackColor, targetBackColor, value);
      myText.color = lerpedColor;
      myQtyText.color = lerpedColor;
    }
  }

  private IEnumerator AnimateArrowSway() {
    Vector3 rootPos = myArrowRect.anchoredPosition;
    float swayTimer = 0f;

    while (true) {
      float xx = 5f * Mathf.Sin(swayTimer);
      myArrowRect.anchoredPosition = rootPos + new Vector3(xx, 0, 0);
      swayTimer = (swayTimer + 5f * Time.deltaTime) % 360f;
      yield return null;
    }
  }

}
