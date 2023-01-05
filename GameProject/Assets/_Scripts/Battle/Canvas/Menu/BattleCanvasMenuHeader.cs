using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvasMenuHeader : Singleton<BattleCanvasMenuHeader>
{

  private RectTransform myRect;
  private CanvasGroup myCanvasGroup;
  private Text myText;

  // public methods

  public void SetText(string text) => myText.text = text;

  public void Show() {
    myCanvasGroup.alpha = 1;
  }
  public void Hide() {
    myCanvasGroup.alpha = 0;
  }

  public void Shift() {
    UtilCoroutine.Instance.StartCoroutine(this, ref animToggleShift, AnimateToggleShift(true));
  }

  public void Unshift() {
    UtilCoroutine.Instance.StartCoroutine(this, ref animToggleShift, AnimateToggleShift(false));
  }

  // init

  private void Init() {
    myRect = GetComponent<RectTransform>();
    myCanvasGroup = GetComponent<CanvasGroup>();
    myText = transform.Find("Text").GetComponent<Text>();

    AnimState.Shifted.pos = myRect.anchoredPosition;
    AnimState.Unshifted.pos = AnimState.Shifted.pos - new Vector2(85, 0);

    myRect.anchoredPosition = AnimState.Unshifted.pos;
    myCanvasGroup.alpha = 0;
  }

  // base methods

  protected override void Awake() {
    base.Awake();
    Init();
  }



  // animations

  private struct AnimStateData
  {
    public Vector2 pos;
  }

  private class AnimStateBase
  {
    public AnimStateData Unshifted = new AnimStateData {
      pos = Vector2.zero,
    };
    public AnimStateData Shifted = new AnimStateData {
      pos = Vector2.zero,
    };
  }
  private AnimStateBase AnimState = new AnimStateBase();

  private IEnumerator animToggleShift;

  private IEnumerator AnimateToggleShift(bool shift) {
    float target = shift ? 1 : 0;
    float value = 1 - target;

    while (Mathf.Abs(target - value) > 0.01f) {
      UpdateProps();
      value = Mathf.Lerp(value, target, 0.2f);
      yield return null;
    }
    value = target;
    UpdateProps();

    void UpdateProps() {
      myRect.anchoredPosition = Vector2.Lerp(AnimState.Unshifted.pos, AnimState.Shifted.pos, value);
    }
  }

}
