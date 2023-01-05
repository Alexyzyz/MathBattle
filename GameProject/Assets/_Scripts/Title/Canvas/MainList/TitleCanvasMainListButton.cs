using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleCanvasMainListButton : MonoBehaviour, SubscriberInterface
{

  private RectTransform myRect;

  private Image myBack;
  private Text myText;

  private int index;
  private bool isShown;

  // public methods

  public void SetData(TitleMainListButtonData data) {
    index = data.index;
    myText.text = data.text;
  }

  // signal methods

  protected void HandleOnHover(int hoveredIndex) {
    bool show = index == hoveredIndex;
    if (show == isShown) return;
    isShown = show;
    UtilCoroutine.Instance.StartCoroutine(this, ref animToggleHover, AnimateToggleHover(show));
  }

  // signals

  public void Subscribe() {
    TitleCanvasMainList.OnHoverOption += HandleOnHover;
  }

  public void Unsubscribe() {
    TitleCanvasMainList.OnHoverOption -= HandleOnHover;
  }

  // init

  private void Init() {
    myRect = GetComponent<RectTransform>();
    myBack = transform.Find("Back").GetComponent<Image>();
    myText = transform.Find("Text").GetComponent<Text>();

    myRect.localScale = AnimState.Unhovered.scale;
    myBack.color = AnimState.Unhovered.backColor;
    myText.color = AnimState.Unhovered.textColor;
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

  private class AnimStateData
  {
    public Vector2 scale;
    public Color backColor;
    public Color textColor;
  }

  private class AnimStateBase
  {
    public AnimStateData Hovered = new AnimStateData {
      scale = 1.1f * Vector2.one,
      backColor = Color.white,
      textColor = Color.black,
    };
    public AnimStateData Unhovered = new AnimStateData {
      scale = Vector2.one,
      backColor = Color.black,
      textColor = Color.white,
    };
  }
  private AnimStateBase AnimState = new AnimStateBase();

  private IEnumerator animToggleHover;

  private IEnumerator AnimateToggleHover(bool hover) {
    float target = hover ? 1 : 0;
    float val = 1 - target;

    while (Mathf.Abs(target - val) > 0.01f) {
      UpdateProps();
      val = Mathf.Lerp(val, target, 0.2f);
      yield return null;
    }
    val = target;
    UpdateProps();

    void UpdateProps() {
      myRect.localScale = Vector2.Lerp(AnimState.Unhovered.scale, AnimState.Hovered.scale, val);
      myBack.color = Color.Lerp(AnimState.Unhovered.backColor, AnimState.Hovered.backColor, val);
      myText.color = Color.Lerp(AnimState.Unhovered.textColor, AnimState.Hovered.textColor, val);
    }
  }

}
