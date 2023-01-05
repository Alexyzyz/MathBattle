using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCanvasUnitOverheadAttackSignal : MonoBehaviour
{

  private CanvasGroup myCanvasGroup;
  private Action myUnitOnSignalEnd;

  // public methods

  public void Play(Action onSignalEnd) {
    myUnitOnSignalEnd = onSignalEnd;
    UtilCoroutine.Instance.StartCoroutine(this, ref animShow, AnimateShow(AnimState.Start, AnimState.Shown));
  }

  // init

  private void Init() {
    myCanvasGroup = GetComponent<CanvasGroup>();

    myCanvasGroup.alpha = AnimState.Start.alpha;
    transform.localScale = Vector3.one * AnimState.Start.scale;
  }

  // base methods

  private void Awake() {
    Init();
  }



  // animations

  private struct AnimStateData {
    public float alpha;
    public float scale;
  }

  private class AnimStateBase
  {
    public AnimStateData Start = new AnimStateData {
      alpha = 0,
      scale = 1.5f,
    };
    public AnimStateData Shown = new AnimStateData {
      alpha = 1,
      scale = 1,
    };
    public AnimStateData Hidden = new AnimStateData {
      alpha = 0,
      scale = 0.5f,
    };
  }
  private AnimStateBase AnimState = new AnimStateBase();

  private IEnumerator animShow;
  private IEnumerator animHide;

  private IEnumerator AnimateShow(AnimStateData prev, AnimStateData next) {
    float value = 0;
    while (value < 0.99f) {
      UpdateProps();
      value = Mathf.Lerp(value, 1, 0.6f);
      yield return null;
    }
    value = 1;
    UpdateProps();

    yield return new WaitForSeconds(1);
    UtilCoroutine.Instance.StartCoroutine(this, ref animHide, AnimateHide(AnimState.Shown, AnimState.Hidden));

    void UpdateProps() {
      myCanvasGroup.alpha = Mathf.Lerp(prev.alpha, next.alpha, value);
      transform.localScale = Vector3.one * Mathf.Lerp(prev.scale, next.scale, value);
    }
  }

  private IEnumerator AnimateHide(AnimStateData prev, AnimStateData next) {
    float value = 0;
    while (value < 0.99f) {
      UpdateProps();
      value = Mathf.Lerp(value, 1, 0.6f);
      yield return null;
    }
    value = 1;
    UpdateProps();

    myUnitOnSignalEnd();

    void UpdateProps() {
      myCanvasGroup.alpha = Mathf.Lerp(prev.alpha, next.alpha, value);
      transform.localScale = Vector3.one * Mathf.Lerp(prev.scale, next.scale, value);
    }
  }

}
