using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvasIdleFlavorText : Singleton<BattleCanvasIdleFlavorText>
{

  private RectTransform myRect;
  private Text myText;

  private Vector3 showPos = new Vector3(0, 20, 0);
  private Vector3 hidePos = new Vector3(0, -160, 0);

  public bool IsShown { get; private set; }

  // public methods

  public void SetText(string text) => myText.text = text;

  // public methods

  public void ToggleShow(bool state) {
    SetText(
      "Pertarungan dimulai!\n" +
      "Pilih unitmu untuk mulai beraksi dan\n" +
      "kalahkan sebanyak mungkin lawan!"
    );

    UtilCoroutine.Instance.StartCoroutine(this, ref animToggleShow, AnimateToggleShow(state));
  }

  // init

  private void Init() {
    myRect = GetComponent<RectTransform>();
    myText = transform.Find("Text").GetComponent<Text>();

    myRect.anchoredPosition = showPos;
  }

  // base methods

  protected override void Awake() {
    base.Awake();
    Init();
  }

  // animations

  private IEnumerator animToggleShow;

  private IEnumerator AnimateToggleShow(bool show) {
    Vector2 currPos = myRect.anchoredPosition;
    Vector2 targetPos = show ? showPos : hidePos;

    while (show && BattleCanvasUnitInfo.Instance.IsShown) yield return null;

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
