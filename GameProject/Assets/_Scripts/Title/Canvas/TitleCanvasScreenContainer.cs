using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCanvasScreenContainer : Singleton<TitleCanvasScreenContainer>
{

  private Dictionary<TitleState, Vector2> posDict = new Dictionary<TitleState, Vector2> {
    { TitleState.History, new Vector2(1400, 0) },
    { TitleState.Main, Vector2.zero },
    { TitleState.Tutorial, new Vector2(-1400, 0) },
  };

  private RectTransform myRect;

  // public methods

  public void SwitchScreen(TitleState from, TitleState to) {
    UtilCoroutine.Instance.StartCoroutine(this, ref animSwitchScreen, AnimateSwitchScreen(from, to));
  }

  // init

  private void Init() {
    myRect = GetComponent<RectTransform>();
  }

  // base methods

  protected override void Awake() {
    base.Awake();
    Init();
  }



  // animations

  private IEnumerator animSwitchScreen;

  private IEnumerator AnimateSwitchScreen(TitleState from, TitleState to) {
    Vector2 startPos = posDict[from];
    Vector2 endPos = posDict[to];

    float val = 0;
    while (val < 0.999f) {
      UpdateProps();
      val = Mathf.Lerp(val, 1, 0.2f);
      yield return null;
    }
    val = 1;
    UpdateProps();

    void UpdateProps() {
      myRect.anchoredPosition = Vector2.Lerp(startPos, endPos, val);
    }
  }

}
