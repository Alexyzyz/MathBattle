using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CoverTransition
{
  Left,
  Center,
  Right,
}

public class CanvasSceneTransition : Singleton<CanvasSceneTransition>
{

  private RectTransform myCover;

  public bool IsTransitioning { get; private set; }

  // public methods

  public void PlayTransition(CoverTransition from, CoverTransition to) {
    StartCoroutine(AnimateCover(from, to));
  }

  public void TransitionToScene(CoverTransition from, CoverTransition to, string sceneName) {
    StartCoroutine(AnimateCover(from, to, sceneName));
  }

  // init

  private void Init() {
    myCover = transform.Find("Cover").GetComponent<RectTransform>();
  }

  // base methods

  protected override void Awake() {
    base.Awake();
    Init();
  }



  // animations

  private Dictionary<CoverTransition, Vector2> CoverPos = new Dictionary<CoverTransition, Vector2> {
    { CoverTransition.Left, new Vector2(-2000, 0) },
    { CoverTransition.Center, new Vector2(0, 0) },
    { CoverTransition.Right, new Vector2(2000, 0) },
  };

  private IEnumerator AnimateCover(CoverTransition from, CoverTransition to, string sceneName = null) {
    IsTransitioning = true;

    Vector2 startPos = CoverPos[from];
    Vector2 endPos = CoverPos[to];

    float val = 0;

    UpdateProps();
    while (val < 0.99f) {
      UpdateProps();
      val = Mathf.Lerp(val, 1, 0.2f);
      yield return null;
    }
    val = 1;
    UpdateProps();

    if (sceneName != null) {
      SceneManager.LoadScene(sceneName);
    }
    IsTransitioning = false;

    void UpdateProps() {
      myCover.anchoredPosition = Vector2.Lerp(startPos, endPos, val);
    }
  }

}
