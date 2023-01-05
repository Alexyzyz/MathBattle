using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvasUnitOverheadHint : MonoBehaviour
{

  private CanvasGroup myCanvasGroup;
  private Text myText;
  private RectTransform myArrowRect;

  private Vector2 myArrowRootPos;

  // public methods

  public void Show(string text) {
    UtilCoroutine.Instance.StartCoroutine(this, ref animArrowSway, AnimateArrowSway());
    myText.text = text;
    myCanvasGroup.alpha = 1;
  }

  public void Hide() {
    UtilCoroutine.Instance.StopCoroutine(this, ref animArrowSway);
    myCanvasGroup.alpha = 0;
  }

  // init

  private void Init() {
    myCanvasGroup = GetComponent<CanvasGroup>();
    myText = transform.Find("Text").GetComponent<Text>();
    myArrowRect = transform.Find("Arrow").GetComponent<RectTransform>();

    myArrowRootPos = myArrowRect.anchoredPosition;

    myCanvasGroup.alpha = 0;
  }

  // base methods

  private void Awake() {
    Init();
  }

  // animation

  private IEnumerator animArrowSway;

  private IEnumerator AnimateArrowSway() {
    float val = 0;
    while (true) {
      val = (val + Time.deltaTime) % 360;
      UpdateProps();
      yield return null;
    }

    void UpdateProps() {
      float yy = 2.5f * Mathf.Sin(10 * val);
      Vector2 vect = myArrowRootPos;
      vect.y += yy;
      myArrowRect.anchoredPosition = vect;
    }
  }

}
