using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvasMenuMainContainer : Singleton<BattleCanvasMenuMainContainer>
{

  private RectTransform myRect;

  // public methods

  public void ToggleShow(bool show) {
    UtilCoroutine.Instance.StartCoroutine(this, ref animToggleShow, AnimateToggleShow(show));
  }

  // init

  private void Init() {
    myRect = GetComponent<RectTransform>();

    myRect.localScale = Vector3.zero;
  }

  private void InitCoroutines() {
    UtilCoroutine.Instance.StartCoroutine(this, ref animToggleShow, AnimateToggleShow(false));
  }

  // base methods

  protected override void Awake() {
    base.Awake();
    Init();
  }

  private void Start() {
    InitCoroutines();
  }

  // animations

  private IEnumerator animToggleShow;

  private IEnumerator AnimateToggleShow(bool show) {
    Vector3 currScale = myRect.localScale;
    Vector3 targetScale = show ? Vector3.one : Vector3.zero;

    float value = 0;
    while (value < 0.99f) {
      UpdateProps();
      value = Mathf.Lerp(value, 1, 0.6f);
      yield return null;
    }
    value = 1;
    UpdateProps();

    void UpdateProps() {
      myRect.localScale = Vector3.Lerp(currScale, targetScale, value);
    }
  }

}
