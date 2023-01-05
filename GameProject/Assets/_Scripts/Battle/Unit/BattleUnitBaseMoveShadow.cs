using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitBaseMoveShadow : MonoBehaviour
{

  private SpriteRenderer mySpriteRenderer;

  // public methods

  public void SetData(Sprite sprite, Color color) {
    mySpriteRenderer.sprite = sprite;
    mySpriteRenderer.color = color;
  }

  // init

  private void Init() {
    mySpriteRenderer = GetComponent<SpriteRenderer>();
  }

  private void InitCoroutines() {
    StartCoroutine(AnimateFade());
  }

  // base methods

  private void Awake() {
    Init();
  }

  private void Start() {
    InitCoroutines();
  }

  // animations

  private IEnumerator AnimateFade() {
    float valStart = 0.4f;
    float val = valStart;
    while (val > 0) {
      UpdateProps();
      val -= Time.deltaTime;
      yield return null;
    }
    Destroy(gameObject);

    void UpdateProps() {
      mySpriteRenderer.color = UtilColor.SetAlpha(mySpriteRenderer.color, Mathf.Lerp(0, 0.2f, val / valStart));
    }
  }

}
